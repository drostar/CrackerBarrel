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
            inputManager.OnActivateObject += InputManager_OnActivateObject;
            inputManager.OnHighlightObject += InputManager_OnHighlightObject;
        }

        #endregion

        #region Input Manager Event Handlers

        void InputManager_OnActivateObject(GameObject obj)
        {
            CellViewModel cellVM = obj == null ? null : obj.GetComponent<CellViewModel>();

            // Select the object if it's a valid movable piece
            if (cellVM != null && GameBoard.HasValidMovesFrom(cellVM.Cell))
            {
                selectCell(cellVM);
            }
            else
            {
                clearSelectedCell();
            }
        }

        void InputManager_OnHighlightObject(bool isHighlighted, GameObject obj)
        {
            CellViewModel cellVM = obj.GetComponent<CellViewModel>();
            cellVM.IsHighlighted = isHighlighted && cellVM.Cell.CanPegMove;
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

        public void Jump(CellPosition fromPosition, CellPosition toPosition)
        {
            var fromCell = GameBoard.GetCellAtPosition(fromPosition);
            var toCell = GameBoard.GetCellAtPosition(toPosition);

            var jump = GameBoard.ExecuteJump(fromCell, toCell, ElapsedTime);

            // Add move to history
            MoveHistory.Moves.Add(jump);

            // Update cell states after move
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
                cell.CanPegMove = GameBoard.HasValidMovesFrom(cell);
            }
        }
    }
}
