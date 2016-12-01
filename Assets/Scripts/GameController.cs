using Foundation.Databinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Zenject;

namespace CrackerBarrel
{
    public class GameController : ObservableBehaviour
    {
        #region Injected Components

        [Inject]
        InputManager inputManager; 

        #endregion

        public float StartTime { get; private set; }
        public float ElapsedTime { get { return Time.timeSinceLevelLoad - StartTime; } }

        public GameMoveHistory MoveHistory { get; set; }
        public GameBoard GameBoard { get; set; }

		#region Unity Message Handlers

		void Start()
		{
			inputManager.OnActivateObject += InputManager_OnActivateObject;
			inputManager.OnHighlightObject += InputManager_OnHighlightObject;
		}

		#endregion

		#region Input Manager Event Handlers

		void InputManager_OnActivateObject(GameObject obj)
		{
			CellViewModel cellVM = obj.GetComponent<CellViewModel>();

		}

		void InputManager_OnHighlightObject(bool isHighlighted, GameObject obj)
		{
			CellViewModel cellVM = obj.GetComponent<CellViewModel>();
			cellVM.IsHighlighted = isHighlighted && cellVM.Cell.CanPegMove;
		}

		#endregion

		public void NewTriangleGame(int edgeLength)
        {
            StartTime = Time.timeSinceLevelLoad; // capture start time
            GameBoard = new GameBoard();

            // build the cells on the board
            CellPosition startPosition = new CellPosition() { X = 1, Y = edgeLength - 3 };
            for (int y = 0; y < edgeLength; y++)
            {
                for (int x = 0; x < (edgeLength - y); x++)
                {
                    var cell = GameBoard.AddCell(x, y, false);
                    cell.HasPeg = cell.Position != startPosition;
					cell.IsCornerCell = (x == 0 || y == 0 || x == edgeLength - 1 || y == edgeLength - 1);
                }
            }
			updateAvailableMoves();
        }

        // TODO: move Jump to GameBoard class?
        public void Jump(CellPosition fromPosition, CellPosition toPosition)
        {
            var fromCell = GameBoard.GetCellAtPosition(fromPosition);
            var toCell = GameBoard.GetCellAtPosition(toPosition);

            var jump = GameBoard.ExecuteJump(fromCell, toCell, ElapsedTime);

            // add move to history
            MoveHistory.Moves.Add(jump);

			// update cell states after move
			updateAvailableMoves();

            // Trigger win/loss if this jump is the last possible.
            checkForWinLose();
        }

		private void checkForWinLose()
        {

        }

		private void updateAvailableMoves()
		{
			foreach (var cell in GameBoard.HexCells)
			{
				// a cell can be part of a valid move if it has a peg 
				// AND has another peg as a neighbour
				// AND the the cell beyond that neighbour exists and is empty.

				if (!cell.HasPeg)
				{
					cell.CanPegMove = false;
					continue;
				}

				// TODO: extract this to GameBoard.GetValidMovesFrom(...)
				bool hasAtLeastOneJumpableNeighbour = false;
				var neighbourCells = GameBoard.GetValidNeighbourPositions(cell.Position).Select(x => GameBoard.GetCellAtPosition(x));
				foreach (var n in neighbourCells)
				{
					if (n.HasPeg)
					{
						// check what's beyond this peg
						// add the difference to find the potential 'to' position of the peg
						var dx = n.Position.X - cell.Position.X;
						var dy = n.Position.Y - cell.Position.Y;
						var toPosition = new CellPosition(n.Position.X + dx, n.Position.Y + dy);

						Cell toCell = null;
						if (GameBoard.TryGetCellAtPosition(toPosition, out toCell) && !toCell.HasPeg)
						{
							hasAtLeastOneJumpableNeighbour = true;
							break;
						}
					}
				}
				cell.CanPegMove = hasAtLeastOneJumpableNeighbour;
			}
		}
    }
}