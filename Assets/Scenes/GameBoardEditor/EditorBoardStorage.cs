using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CrackerBarrel
{
    public static class EditorBoardStorage
    {
        private static string baseDirectory { get { return Path.Combine(Application.streamingAssetsPath, "Boards"); } }

        public static void SaveBoard(GameBoard gameBoard, string saveName)
        {
            var filePath = saveNameToFilePath(saveName);
            var json = GameBoardData.SerializeGameBoard(gameBoard);
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            File.WriteAllText(filePath, json);
            Debug.Log("File written to " + filePath);
        }

        public static GameBoard LoadBoard(string saveName)
        {
            var filePath = saveNameToFilePath(saveName);
            var json = File.ReadAllText(filePath);
            var gameBoard = GameBoardData.DeserializeGameboard(json);
            return gameBoard;
        }

        public static string[] ListSavedBoards()
        {
            if (Directory.Exists(baseDirectory))
            {
                var files = Directory.GetFiles(baseDirectory, "*.json");
                return files.Select(x => filePathToSaveName(x)).ToArray();
            }
            else
            {
                return new string[0];
            }
        }

        private static string saveNameToFilePath(string saveName)
        {
            string filename = saveName + ".json";
            string filePath = Path.Combine(baseDirectory, filename);
            return filePath;
        }

        private static string filePathToSaveName(string filePath)
        {
            return Path.GetFileNameWithoutExtension(filePath);
        }
    }
}
