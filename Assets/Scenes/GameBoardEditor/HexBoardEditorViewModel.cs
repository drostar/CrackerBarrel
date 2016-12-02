using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Zenject;

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
            AddExpanderCellsAround(initialCell);
        }

        public CellViewModel AddCell(CellPosition cellPosition)
        {
            var cellView = Instantiate(CellPrefab.gameObject).GetComponent<CellViewModel>();
            cellView.transform.SetParent(transform, worldPositionStays: false);
            cellView.transform.localPosition = cellPositionToViewportPosition(cellPosition);
            cellView.gameObject.name = $"C_{cellPosition.X}_{cellPosition.Y}";
            cellView.Cell = new Cell(cellPosition);
            cellView.Cell.HasPeg = true;
            cellView.Initialize();

            gameBoard.AddCell(cellView.Cell);

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
