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

        public GameMoveHistory MoveHistory { get; set; } = new GameMoveHistory();
        public GameBoard GameBoard { get; set; }

        private CellViewModel _selectedCell;
        public CellViewModel SelectedCell {
            get { return _selectedCell; }
            set
            {
                if (_selectedCell == value)
                    return;
                _selectedCell = value;
                RaiseBindingUpdate(nameof(SelectedCell), _selectedCell);
            }
        }

        #region Inpsector Fields

        public Transform PegHoldPosition;

        #endregion

        #region Unity Message Handlers

        void Start()
        {
            inputManager.OnSelectObject += InputManager_OnSelectObject;
            inputManager.OnObjectHighlightChanged += InputManager_OnObjectHighlightChanged;
        }

        #endregion

        #region Input Manager Event Handlers

        void InputManager_OnSelectObject(GameObject obj)
        {
            CellViewModel cellVM = obj == null ? null : obj.GetComponent<CellViewModel>();

            // cellVM will be null to signal something other than a GameObject was clicked
            if (cellVM == null)
            {
                clearSelectedCell();
            }
            else if (SelectedCell == null) 
            {
                // If no current selection, select the activated object if it has valid moves
                if (cellVM.Cell.CanPegMove)
                {
                    selectCell(cellVM);
                }
                else
                {
                    clearSelectedCell();
                }
            }
            else
            {
                // If there is a current selection then move the peg to the activated cell (if possible)
                if (GameBoard.IsValidMove(SelectedCell.Cell, cellVM.Cell))
                {
                    jump(SelectedCell, cellVM);
                }
                else
                {
                    clearSelectedCell();
                }
            }
        }

        void InputManager_OnObjectHighlightChanged(bool isHighlighted, GameObject obj)
        {
            CellViewModel cellVM = obj.GetComponent<CellViewModel>();

            // If no current selection, highlight cells with pegs that can move
            if (SelectedCell == null)
            {
                cellVM.IsHighlighted = isHighlighted && cellVM.Cell.CanPegMove;
            }
            // With a current selection, highlight cell where the selected peg can move to
            else
            {
                // OPTIMIZE: Cache this since it only needs to be calculated once after the cell is selected.
                cellVM.IsHighlighted = isHighlighted && GameBoard.IsValidMove(SelectedCell.Cell, cellVM.Cell);
            }
        }

        #endregion

        #region Selection

        private void selectCell(CellViewModel cellVM)
        {
            // Deselect old cell first
            if (SelectedCell != null && SelectedCell != cellVM)
            {
                SelectedCell.DeselectCell();
            }
            // Don't reselect the same cell. Treat it as a deselect instead and bail out.
            if (SelectedCell == cellVM)
            {
                clearSelectedCell();
                return;
            }

            // Note the select and signal the cell's viewmodel that it make the cell/peg look selected.
            SelectedCell = cellVM;
            var holdPosition = PegHoldPosition.position;
            cellVM.SelectCell(holdPosition);
        }

        private void clearSelectedCell()
        {
            if (SelectedCell != null)
            {
                SelectedCell.DeselectCell();
                SelectedCell = null;
            }
        }

        #endregion

        public void NewTriangleGame(int edgeLength)
        {
            StartTime = Time.timeSinceLevelLoad; // capture start time
            GameBoard = new GameBoard();

            // build the cells on the board
            CellPosition startPosition = new CellPosition() { X = 0, Y = edgeLength - 2 };
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

        private void jump(CellViewModel fromCellVM, CellViewModel toCellVM)
        {
            var fromCell = fromCellVM.Cell;
            var toCell = toCellVM.Cell;

            inputManager.DisableInput = true;

            // Animate selected peg to 'to' cell
            // TODO: Make this not a callback but either a coroutine or async function
            fromCellVM.JumpPegTo(toCellVM, () =>
            {
                // Update the game board with the Jump
                var jump = GameBoard.ExecuteJump(fromCell, toCell, ElapsedTime);

                // Return the 'from' cell's peg to its own cell
                fromCellVM.ResetPeg();

                // Add move to history
                MoveHistory.Moves.Add(jump);

                // Update cell valid move states after this move
                updateAvailableMoves();

                inputManager.DisableInput = false;
                clearSelectedCell();

                // Trigger win/loss if this jump is the last possible.
                checkForWinLose();
            });
        }

        private void checkForWinLose()
        {
            int pegsLeft = GameBoard.HexCells.Count(x => x.HasPeg);
            // A win is when there is only 1 peg left.
            if (pegsLeft == 1)
            {
                triggerWin();
            }
            // A loss is when there is more than 1 peg left and there are no more valid moves
            else if (pegsLeft > 1 && !GameBoard.HexCells.Any(x => x.CanPegMove))
            {
                triggerLoss();
            }
        }

        private void updateAvailableMoves()
        {
            foreach (var cell in GameBoard.HexCells)
            {
                cell.CanPegMove = GameBoard.HasValidMovesFrom(cell);
            }
        }

        private void triggerWin()
        {
            Debug.Log("WIN");
        }

        private void triggerLoss()
        {
            Debug.Log("LOSS");
        }

    }
}
