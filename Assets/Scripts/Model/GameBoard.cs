using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace CrackerBarrel
{
    public class GameBoard
    {
        // TODO: Optimize this cell lookup by CellPosition and add an accessor
        public List<Cell> HexCells { get; private set; } = new List<Cell>();

        public CellPosition StartPosition { get; private set; }

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
            return AddCell(cellPosition);
        }

        public Cell AddCell(CellPosition cellPosition)
        {
            // Make sure we don't have a cell at this position already
            if (HexCells.Any(c => c.Position == cellPosition))
                throw new InvalidCellPositionException($"{cellPosition} already exists in the game board");

            Cell newCell = new Cell(cellPosition);
            HexCells.Add(newCell);
            return newCell;
        }

        public void SetStartPosition(CellPosition startPosition)
        {
            StartPosition = startPosition;
            var cell = GetCellAtPosition(startPosition);
            cell.HasPeg = false;
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
        public IEnumerable<Cell> GetValidMovesFrom(Cell fromCell)
        {
            // A cell can be part of a valid move if it has a peg 
            // AND has another peg as a neighbour
            // AND the the cell beyond that neighbour exists and is empty.

            if (!fromCell.HasPeg)
            {
                yield break;
            }

            var neighbourCells = GetValidNeighbourPositions(fromCell.Position).Select(x => GetCellAtPosition(x));
            foreach (var n in neighbourCells)
            {
                if (n.HasPeg)
                {
                    // Check what's beyond this peg
                    // Add the difference to find the potential 'to' position of the peg
                    var dx = n.Position.X - fromCell.Position.X;
                    var dy = n.Position.Y - fromCell.Position.Y;
                    var toPosition = new CellPosition(n.Position.X + dx, n.Position.Y + dy);

                    Cell toCell = null;
                    if (TryGetCellAtPosition(toPosition, out toCell) && !toCell.HasPeg)
                    {
                        yield return toCell;
                    }
                }
            }
        }

        public bool HasValidMovesFrom(Cell fromCell)
        {
            return GetValidMovesFrom(fromCell).Any();
        }

        public bool IsValidMove(Cell fromCell, Cell toCell)
        {
            return GetValidMovesFrom(fromCell).Contains(toCell);
        }

        /// <summary>
        /// Updates the all cells' <see cref="Cell.CanPegMove"/> property with current valid moves.
        /// </summary>
        public void UpdateAvailableMoves()
        {
            foreach (var cell in HexCells)
            {
                cell.CanPegMove = HasValidMovesFrom(cell);
            }
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

            // Validate move
            if (!fromCell.HasPeg)
                throw new InvalidMoveException($"From position {fromPosition} has no peg.");
            if (toCell.HasPeg)
                throw new InvalidMoveException($"To position {toPosition} already has a peg.");

            // Update the game board for the move
            var jumpedPosition = GetValidNeighbourPositions(fromPosition)
                                .Intersect(GetValidNeighbourPositions(toPosition))
                                .Single(); // Will throw if jump is invalid
            var jumpedCell = GetCellAtPosition(jumpedPosition);

            // Apply the jump to the board
            var jump = new Jump(fromPosition, toPosition, jumpedPosition, timestamp);
            fromCell.HasPeg = false;
            jumpedCell.HasPeg = false;
            toCell.HasPeg = true;

            return jump;
        }
    }
}