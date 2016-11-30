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
        
        public void NewTriangleGame(int edgeLength)
        {
            StartTime = Time.timeSinceLevelLoad; // capture start time
            GameBoard = new GameBoard();

            // build the cells on the board
            CellPosition startPosition = new CellPosition() { X = 1, Y = edgeLength - 2 };
            for (int y = 0; y < edgeLength; y++)
            {
                for (int x = 0; x < (edgeLength - y); x++)
                {
                    var cell = GameBoard.AddCell(x, y, false);
                    cell.HasPeg = cell.Position != startPosition;
                    cell.IsCornerCell = (x == 0 || y == 0 || x == edgeLength - 1 || y == edgeLength - 1);
                }
            }
        }

        // TODO: move Jump to GameBoard class?
        public void Jump(CellPosition fromPosition, CellPosition toPosition)
        {
            var fromCell = GameBoard.GetCellAtPosition(fromPosition);
            var toCell = GameBoard.GetCellAtPosition(toPosition);

            var jump = GameBoard.ExecuteJump(fromCell, toCell, ElapsedTime);

            // add move to history
            MoveHistory.Moves.Add(jump);

            // Trigger win/loss if this jump is the last possible.
            CheckForWinLose();
        }

        private void CheckForWinLose()
        {

        }
    }
}