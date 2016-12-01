using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

namespace CrackerBarrel
{
    public static class GameBoardGenerator
    {

        public static GameBoard CreateTriangleGame(int edgeLength)
        {
            var gameBoard = new GameBoard();

            // Build the cells on the board
            CellPosition startPosition = new CellPosition() { X = 0, Y = edgeLength - 2 };
            for (int y = 0; y < edgeLength; y++)
            {
                for (int x = 0; x < (edgeLength - y); x++)
                {
                    bool isCorner = (x == 0 || y == 0 || x == edgeLength - 1 || y == edgeLength - 1);
                    var cell = gameBoard.AddCell(x, y, isCorner);
                    cell.HasPeg = cell.Position != startPosition;
                }
            }

            gameBoard.SetStartPosition(startPosition);

            return gameBoard;
        }

        public static GameBoard CreateFromRawPositions(CellPosition startPosition, IEnumerable<CellPosition> cellPositions)
        {
            var gameBoard = new GameBoard();
            foreach (var cellPosition in cellPositions)
            {
                var cell = gameBoard.AddCell(cellPosition);
                cell.HasPeg = cell.Position != startPosition;
            }

            gameBoard.SetStartPosition(startPosition);

            return gameBoard;
        }
    }
}