using UnityEngine;
using System.Collections;
using System.Linq;

namespace CrackerBarrel
{
    public static class GameBoardGenerator
    {

        public static GameBoard CreateTriangleGame(int edgeLength)
        {
            var gameBoard = new GameBoard();

            // build the cells on the board
            CellPosition startPosition = new CellPosition() { X = 0, Y = edgeLength - 2 };
            for (int y = 0; y < edgeLength; y++)
            {
                for (int x = 0; x < (edgeLength - y); x++)
                {
                    var cell = gameBoard.AddCell(x, y, false);
                    cell.HasPeg = cell.Position != startPosition;
                    cell.IsCornerCell = (x == 0 || y == 0 || x == edgeLength - 1 || y == edgeLength - 1);
                }
            }

            return gameBoard;
        }

    }
}