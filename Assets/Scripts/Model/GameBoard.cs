using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrackerBarrel
{
    public class GameBoard
    {
        protected List<HexCell> HexCells { get; set; } = new List<HexCell>();

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
            throw new NotImplementedException();
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