using Foundation.Databinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CrackerBarrel
{
    public class GameController : ObservableBehaviour
    {
        public float StartTime { get; private set; }
        public float ElapsedTime { get { return Time.timeSinceLevelLoad - StartTime; } }

        public GameMoveHistory MoveHistory { get; set; }

        public GameBoard GameBoard { get; set; }

        public InputManager InputManager { get; set; }

        public void NewTriangleGame()
        {
            StartTime = Time.timeSinceLevelLoad; // capture start time
            GameBoard = new GameBoard();

            // build the cells on the board
            int edgeLength = 4;
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

        public void Jump(CellPosition fromPosition, CellPosition toPosition)
        {
            // update the game board for the move
            var fromCell = GameBoard.GetCellAtPosition(fromPosition);
            var toCell = GameBoard.GetCellAtPosition(toPosition);
            var jumpedPosition = GameBoard.GetValidNeighbourPositions(fromPosition).Intersect(GameBoard.GetValidNeighbourPositions(toPosition)).Single();
            var jumpedCell = GameBoard.GetCellAtPosition(jumpedPosition);

            // apply the jump to the board
            var jump = new Jump(fromPosition, toPosition) { TimeOffset = ElapsedTime, JumpedPosition = jumpedPosition };
            jumpedCell.HasPeg = false;

            // add move to history
            MoveHistory.Moves.Add(new Jump(fromPosition, toPosition));

            // Trigger win/loss if this jump is the last possible.
            CheckForWinLose();
        }

        private void CheckForWinLose()
        {

        }
    }
}