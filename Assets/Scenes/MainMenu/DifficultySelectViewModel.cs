using UnityEngine;
using System.Collections;
using System.Linq;
using Foundation.Databinding;
using UnityEngine.SceneManagement;
using System;
using System.IO;

namespace CrackerBarrel
{
    public class DifficultySelectViewModel : ObservableBehaviour
    {
        public void Open()
        {
            gameObject.SetActive(true);
        }
        public void Close()
        {
            gameObject.SetActive(false);
        }

        #region Commands

        public void PlayEasyGame()
        {
            GameBoardSceneParameters.SetParameters(new GameBoardSceneParameters()
            {
                GameBoard = GameBoardGenerator.CreateTriangleGame(4),
                TimeLimit = (60f * 3f), // 3 minutes
            });
            
            SceneManager.LoadScene("GameBoard");
        }

        public void PlayNormalGame()
        {
            GameBoardSceneParameters.SetParameters(new GameBoardSceneParameters()
            {
                GameBoard = GameBoardGenerator.CreateTriangleGame(5),
                TimeLimit = (60f * 3f), // 3 minutes
            });
            SceneManager.LoadScene("GameBoard");
        }

        public void PlayHardGame()
        {
            GameBoardSceneParameters.SetParameters(new GameBoardSceneParameters()
            {
                GameBoard = GameBoardGenerator.CreateTriangleGame(6),
                TimeLimit = (60f * 3f), // 3 minutes
            });
            SceneManager.LoadScene("GameBoard");
        }

        public void LoadGame()
        {
            // Load from streaming assets for demonstration purposes.
            string filePath = Path.Combine(Application.streamingAssetsPath, "diamondBoard.json");
            string json = File.ReadAllText(filePath);
            var gameBoard = GameBoardData.DeserializeGameboard(json);

            GameBoardSceneParameters.SetParameters(new GameBoardSceneParameters()
            {
                GameBoard = gameBoard,
                TimeLimit = (60f * 3f), // 3 minutes
            });

            SceneManager.LoadScene("GameBoard");
        }

        #endregion
    }
}