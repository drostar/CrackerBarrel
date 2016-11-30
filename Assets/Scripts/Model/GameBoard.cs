using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrackerBarrel
{
    public class GameBoard
    {
        // TODO: make accessor for this instead of accessing directly... at a likely cost of garbage though....
        public List<HexCell> HexCells { get; private set; } = new List<HexCell>();

        public static GameBoard Load(string filePath)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds a cell to the game board and returns a reference to the newly added cell
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="isCorner"></param>
        /// <returns></returns>
        public HexCell AddCell(int x, int y, bool isCorner)
        {
            var cellPosition = new CellPosition(x, y);
            // make sure we don't have a cell at this position already
            if (HexCells.Any(c => c.Position == cellPosition))
                throw new InvalidCellPositionException($"{cellPosition} already exists in the game board");

            HexCell newCell = new HexCell(cellPosition);
            HexCells.Add(newCell);
            return newCell;
        }


        public HexCell GetCellAtPosition(CellPosition position)
        {
            var result = HexCells.FirstOrDefault(o => o.Position == position);
            if (result == null)
                throw new InvalidCellPositionException(position);
            return result;
        }

        /// <summary>
        /// Returns the valid positions on the board adjacent to the give <paramref name="position"/>
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public CellPosition[] GetValidNeighbourPositions(CellPosition position)
        {
            throw new NotImplementedException();
        }
    }
}