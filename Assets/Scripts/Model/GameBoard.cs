using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace CrackerBarrel
{
    public class GameBoard
    {
        // TODO: make accessor for getting all HexCells instead of accessing directly... at a likely cost of garbage though....
        // TODO: optimize this cell lookup by CellPosition.
        public List<Cell> HexCells { get; private set; } = new List<Cell>();

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
        public Cell AddCell(int x, int y, bool isCorner)
        {
            var cellPosition = new CellPosition(x, y);
            // make sure we don't have a cell at this position already
            if (HexCells.Any(c => c.Position == cellPosition))
                throw new InvalidCellPositionException($"{cellPosition} already exists in the game board");

            Cell newCell = new Cell(cellPosition);
            HexCells.Add(newCell);
            return newCell;
        }


        public bool IsValidCellPosition(CellPosition position)
        {
            return HexCells.Any(o => o.Position == position);
        }
        public Cell GetCellAtPosition(CellPosition position)
        {
            var result = HexCells.FirstOrDefault(o => o.Position == position);
            if (result == null)
                throw new InvalidCellPositionException(position);
            return result;
        }
        public bool TryGetCellAtPosition(CellPosition position, out Cell cell)
        {
            cell = HexCells.FirstOrDefault(o => o.Position == position);
            return cell != null;
        }

        public Cell GetCellAtPosition(int x, int y)
        {
            return GetCellAtPosition(new CellPosition(x, y));
        }

        /// <summary>
        /// Returns the valid target cells if player were to attempt to move the given <paramref name="fromCell"/>
        /// </summary>
        /// <param name="fromCell"></param>
        /// <returns></returns>
        public Cell[] GetValidMovesFrom(Cell fromCell)
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Returns the valid positions on the board adjacent to the give <paramref name="position"/>
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public IEnumerable<CellPosition> GetValidNeighbourPositions(CellPosition position)
        {
            CellPosition[] possibleNeighbours = new[]
            {
                new CellPosition(position.X - 1, position.Y),
                new CellPosition(position.X + 1, position.Y),
                new CellPosition(position.X - 1, position.Y + 1),
                new CellPosition(position.X, position.Y + 1),
                new CellPosition(position.X, position.Y - 1),
                new CellPosition(position.X + 1, position.Y - 1),
            };

            var validNeighbours = possibleNeighbours.Intersect(HexCells.Select(x => x.Position));
            return validNeighbours;
        }

        public Jump ExecuteJump(Cell fromCell, Cell toCell, float timestamp)
        {
            var fromPosition = fromCell.Position;
            var toPosition = toCell.Position;

            // validate move
            if (!fromCell.HasPeg)
                throw new InvalidMoveException($"From position {fromPosition} has no peg.");
            if (toCell.HasPeg)
                throw new InvalidMoveException($"To position {toPosition} already has a peg.");

            // update the game board for the move
            var jumpedPosition = GetValidNeighbourPositions(fromPosition).Intersect(GetValidNeighbourPositions(toPosition)).Single(); // will throw if jump is invalid
            var jumpedCell = GetCellAtPosition(jumpedPosition);

            // apply the jump to the board
            var jump = new Jump(fromPosition, toPosition) { TimeOffset = timestamp, JumpedPosition = jumpedPosition };
            fromCell.HasPeg = false;
            jumpedCell.HasPeg = false;
            toCell.HasPeg = true;

            return jump;
        }
    }
}