using UnityEngine;
using System.Collections;
using System.Linq;
using Zenject;

namespace CrackerBarrel
{
    public class HexBoardView : MonoBehaviour
    {
        [Inject]
        GameController gameController;



        #region Unity Inspector Fields

        public CellView CellPrefab;
        public Vector2 CellSpacing = new Vector2(1f, 1f); // temporarilty have fixed sizes until we get the basic visuals working

        #endregion

        #region Unity Message Handlers

        void Start()
        {
            InitializeGame();
        }

        #endregion

        void InitializeGame()
        {
            gameController.NewTriangleGame();

            BuildBoardView(gameController.GameBoard);
        }

        void BuildBoardView(GameBoard board)
        {
            // draw visuals based on the state of the game board.
            foreach (var cell in board.HexCells)
            {
                var cellView = Instantiate(CellPrefab.gameObject).GetComponent<CellView>();
                cellView.transform.SetParent(transform, worldPositionStays: false);
                initializeCellViewWithCell(cellView, cell);
            }
        }

        /// <summary>
        /// Links a <see cref="GameBoard"/> <see cref="HexCell"/> to a visual component (ie, <see cref="CellView"/>)
        /// </summary>
        /// <param name="cellView"></param>
        /// <param name="cell"></param>
        void initializeCellViewWithCell(CellView cellView, HexCell cell)
        {
            cellView.transform.localPosition = cellPositionToViewportPosition(cell.Position);
            cellView.gameObject.name = $"C_{cell.Position.X}_{cell.Position.Y}";
        }

        Vector2 cellPositionToViewportPosition(CellPosition cellPosition)
        {
            // the coordinates in the model are such that visual x moves a half cell to the right on each move up one row. So add the row offset.
            float rowOffset = cellPosition.Y * (0.5f * CellSpacing.x);
            float x = cellPosition.X * CellSpacing.x + rowOffset;
            float y = cellPosition.Y * CellSpacing.y;

            return new Vector2(x, y);
        }
    } 
}
