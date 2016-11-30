using UnityEngine;
using System.Collections;
using System.Linq;
using Zenject;

namespace CrackerBarrel
{
    public class HexBoardViewModel : MonoBehaviour
    {
        [Inject]
        GameController gameController;

        #region Unity Inspector Fields

        public CellViewModel CellPrefab;
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
            //gameController.NewTriangleGame(4);
            //gameController.NewTriangleGame(5);
            gameController.NewTriangleGame(6);

            BuildBoardView(gameController.GameBoard);
        }

        void BuildBoardView(GameBoard board)
        {
            int minX = 0, minY = 0, maxX = 0, maxY = 0;

            // draw visuals based on the state of the game board.
            foreach (var cell in board.HexCells)
            {
                var cellView = Instantiate(CellPrefab.gameObject).GetComponent<CellViewModel>();
                cellView.transform.SetParent(transform, worldPositionStays: false);
                initializeCellViewWithCell(cellView, cell);

                minX = Mathf.Min(minX, cell.Position.X);
                minY = Mathf.Min(minY, cell.Position.Y);
                maxX = Mathf.Max(maxX, cell.Position.X);
                maxY = Mathf.Max(maxY, cell.Position.Y);
            }

            // position hex board such that the entire board is centered around (0,0)
            var width = CellSpacing.x * (maxX - minX);
            var height = CellSpacing.y * (maxY - minY);
            centerBoard(width, height);
        }

        void centerBoard(float boardWidth, float boardHeight)
        {
            var boardX = -(boardWidth / 2f);
            var boardY = -(boardHeight / 2f);

            transform.localPosition = new Vector3(boardX, boardY, transform.localPosition.z);
        }

        /// <summary>
        /// Links a <see cref="GameBoard"/> <see cref="Cell"/> to a visual component (ie, <see cref="CellViewModel"/>)
        /// </summary>
        /// <param name="cellView"></param>
        /// <param name="cell"></param>
        void initializeCellViewWithCell(CellViewModel cellView, Cell cell)
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
