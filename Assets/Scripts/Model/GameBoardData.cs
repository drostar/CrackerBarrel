using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrackerBarrel
{
    [Serializable]
    public class GameBoardData
    {
        public List<CellPosition> CellPositions = new List<CellPosition>();
        public CellPosition StartPosition;

        public static string SerializeGameBoard(GameBoard gameBoard)
        {
            GameBoardData data = new GameBoardData();
            foreach (var cell in gameBoard.HexCells)
            {
                data.CellPositions.Add(cell.Position);
            }
            data.StartPosition = gameBoard.StartPosition;

            string json = UnityEngine.JsonUtility.ToJson(data, prettyPrint:true);
            return json;
        }

        public static GameBoard DeserializeGameboard(string serializedGameBoard)
        {
            GameBoardData data = UnityEngine.JsonUtility.FromJson<GameBoardData>(serializedGameBoard);

            var gameBoard = GameBoardGenerator.CreateFromRawPositions(data.StartPosition, data.CellPositions);
            return gameBoard;
        }
    }
}
