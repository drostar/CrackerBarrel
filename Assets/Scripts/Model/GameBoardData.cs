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


        public static GameBoardData GetGameBoardData(GameBoard gameBoard)
        {
            GameBoardData data = new GameBoardData();
            foreach (var cell in gameBoard.HexCells)
            {
                data.CellPositions.Add(cell.Position);
            }
            data.StartPosition = gameBoard.StartPosition;
            return data;
        }

        public static string SerializeGameBoard(GameBoard gameBoard)
        {
            var data = GetGameBoardData(gameBoard);
            string json = UnityEngine.JsonUtility.ToJson(data, prettyPrint:true);
            return json;
        }

        public static GameBoard GetGameBoard(GameBoardData data)
        {
            return GameBoardGenerator.CreateFromRawPositions(data.StartPosition, data.CellPositions);
        }

        public static GameBoard DeserializeGameboard(string serializedGameBoard)
        {
            GameBoardData data = UnityEngine.JsonUtility.FromJson<GameBoardData>(serializedGameBoard);
            var gameBoard = GetGameBoard(data);
            return gameBoard;
        }
    }
}
