using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Zenject;

namespace CrackerBarrel
{
    public class HexBoardViewModel : MonoBehaviour
    {
        [Inject]
        GameController gameController;

        [Inject]
        ReplayManager replayManager;

        #region Unity Inspector Fields

        public CellViewModel CellPrefab;

        // Ideally CellSpacing would be calculated based on the CellPrefab but for now it must be manually specified.
        [Tooltip("Adjust cell spacing to match the size of the cell prefab")]
        public Vector2 CellSpacing = new Vector2(1f, 1f);

        #endregion

        #region Unity Message Handlers

        void Start()
        {
            InitializeGame();
        }

        #endregion

        void InitializeGame()
        {
            var sceneParameters = GameBoardSceneParameters.GetParameters();

            // If null, start with a default board for testing when directly starting from GameBoard scene.
            if (sceneParameters == null)
            {
                sceneParameters = new GameBoardSceneParameters()
                {
                    GameBoard = GameBoardGenerator.CreateTriangleGame(4),
                    TimeLimit = 180f
                };
            }

            if (sceneParameters.IsReplay)
            {
                replayManager.InitializeWithReplay(sceneParameters.ReplayHistory);
                BuildBoardView(replayManager.GameBoard);
            }
            else
            {
                gameController.InitializeWithBoard(sceneParameters.GameBoard, sceneParameters.TimeLimit);
                BuildBoardView(gameController.GameBoard);
            }

        }

        void BuildBoardView(GameBoard board)
        {
            int minX = 0, minY = 0, maxX = 0, maxY = 0;

            cachedCellViewModels.Clear();
            // Draw visuals based on the state of the game board.
            foreach (var cell in board.HexCells)
            {
                var cellView = Instantiate(CellPrefab.gameObject).GetComponent<CellViewModel>();
                cellView.transform.SetParent(transform, worldPositionStays: false);
                initializeCellViewWithCell(cellView, cell);
                cachedCellViewModels.Add(cell.Position, cellView);

                minX = Mathf.Min(minX, cell.Position.X);
                minY = Mathf.Min(minY, cell.Position.Y);
                maxX = Mathf.Max(maxX, cell.Position.X);
                maxY = Mathf.Max(maxY, cell.Position.Y);
            }

            // Position hex board such that the entire board is centered around (0,0)
            var width = CellSpacing.x * (maxX - minX);
            var height = CellSpacing.y * (maxY - minY);
            centerBoard(width, height);
        }

        Dictionary<CellPosition, CellViewModel> cachedCellViewModels = new Dictionary<CellPosition, CellViewModel>();

        public CellViewModel GetCellViewModelFor(Cell cell)
        {
            CellViewModel cellVM;
            if (cachedCellViewModels.TryGetValue(cell.Position, out cellVM))
                return cellVM;
            else
                return null;
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
            cellView.Cell = cell;
            cellView.Initialize();
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
