using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CrackerBarrel
{
    public static class ReplayManager
    {
        private static string baseDirectory { get { return Application.persistentDataPath; } }

        public static void SaveReplay(GameMoveHistory moveHistory, string saveName)
        {
            var filePath = saveNameToFilePath(saveName);
            var json = JsonUtility.ToJson(moveHistory, prettyPrint: true);
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            File.WriteAllText(filePath, json);
            Debug.Log("File written to " + filePath);
        }

        public static GameMoveHistory LoadReplay(string saveName)
        {
            var filePath = saveNameToFilePath(saveName);
            var json = File.ReadAllText(filePath);
            var moveHistory = JsonUtility.FromJson<GameMoveHistory>(json);
            return moveHistory;
        }

        public static string[] ListSavedReplays()
        {
            var files = Directory.GetFiles(baseDirectory);
            return files.Select(x => filePathToSaveName(x)).ToArray();
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
