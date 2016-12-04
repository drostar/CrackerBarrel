using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Zenject;
using System;

namespace CrackerBarrel
{
    public class HexBoardEditorViewModel : MonoBehaviour
    {
        [Inject]
        InputManager inputManager;

        GameBoard gameBoard = new GameBoard();

        #region Unity Inspector Fields

        public CellViewModel CellPrefab;

        [Tooltip("Adjust cell spacing to match the size of the cell prefab")]
        public Vector2 CellSpacing = new Vector2(1f, 1f);

        #endregion

        void Start()
        {
            inputManager.OnSelectObject += InputManager_OnSelectObject;
            inputManager.OnObjectHighlightChanged += InputManager_OnObjectHighlightChanged;
            NewBoard();
        }

        void InputManager_OnSelectObject(GameObject obj)
        {
            if (obj == null)
                return;

            CellViewModel vm = obj.GetComponent<CellViewModel>();
            // If this is an expander cell, convert this cell to a real cell and add expanders around it.
            if (vm.IsExpanderCell)
            {
                vm.IsExpanderCell = false;
                AddExpanderCellsAround(vm);
            }
        }

        void InputManager_OnObjectHighlightChanged(bool arg1, GameObject arg2)
        {

        }

        public void ClearChildren()
        {
            var count = transform.childCount;
            for (int i = count - 1; i >= 0; i--)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }

        public void NewBoard()
        {
            ClearChildren();
            gameBoard = new GameBoard();
            var initialCell = AddCell(new CellPosition(0, 0));
            initialCell.Cell.HasPeg = false;
            AddExpanderCellsAround(initialCell);
        }

        public void LoadBoard(GameBoard gameBoard)
        {
            ClearChildren();
            this.gameBoard = gameBoard;

            // Make a copy of the list so we can modify the current gameBoard as we add expanders.
            var gameBoardCells = gameBoard.HexCells.ToArray();

            // Add cell views for actual cells
            foreach (var cell in gameBoardCells)
            {
                var cellVM = AddCellVM(cell);

                // Add expanders
                AddExpanderCellsAround(cellVM);
            }

            this.gameBoard.GetCellAtPosition(gameBoard.StartPosition).HasPeg = false;
        }

        public GameBoard GetCurrentBoard(string name)
        {
            var usedCells = transform.GetComponentsInChildren<CellViewModel>().Where(x => !x.IsExpanderCell).Select(x => x.Cell);

            // Whichever cell has no peg will be the starter cell.
            var startCell = usedCells.Where(x => !x.HasPeg).FirstOrDefault();
            // Or if none have no peg, then we'll just make the initial cell the starter cell.
            if (startCell == null)
                startCell = usedCells.First();

            // Rebuild the gameboard based on the used cells.
            GameBoard gb = new GameBoard();
            foreach (var cell in usedCells)
            {
                gb.AddCell(cell);
            }
            gb.SetStartPosition(startCell.Position);

            return gb;
        }

        public CellViewModel AddCell(CellPosition cellPosition)
        {
            var cell = new Cell(cellPosition);
            var cellView = AddCellVM(cell);
            gameBoard.AddCell(cellView.Cell);

            return cellView;
        }

        public CellViewModel AddCellVM(Cell cell)
        {
            var cellView = Instantiate(CellPrefab.gameObject).GetComponent<CellViewModel>();
            cellView.transform.SetParent(transform, worldPositionStays: false);
            cellView.transform.localPosition = cellPositionToViewportPosition(cell.Position);
            cellView.gameObject.name = $"C_{cell.Position.X}_{cell.Position.Y}";

            cellView.Cell = cell;
            cellView.Cell.HasPeg = true;
            cellView.Initialize();

            return cellView;
        }

        public List<CellViewModel> AddExpanderCellsAround(CellViewModel edgeCellVM)
        {
            var possibleExpanderPositions = gameBoard.GetPossibleNeighbourPositions(edgeCellVM.Cell.Position);
            var newExpanders = possibleExpanderPositions.Except(gameBoard.HexCells.Select(x => x.Position));

            List<CellViewModel> newCells = new List<CellViewModel>();
            foreach (var item in newExpanders)
            {
                var newCell = AddCell(item);
                newCell.IsExpanderCell = true;
                newCells.Add(newCell);
            }

            return newCells;
        }

        Vector2 cellPositionToViewportPosition(CellPosition cellPosition)
        {
            // The coordinates in model are such that visual x moves a half cell to the right on each move up one row.
            // So add the row offset.
            float rowOffset = cellPosition.Y * (0.5f * CellSpacing.x);
            float x = cellPosition.X * CellSpacing.x + rowOffset;
            float y = cellPosition.Y * CellSpacing.y;

            return new Vector2(x, y);
        }
    }
}
