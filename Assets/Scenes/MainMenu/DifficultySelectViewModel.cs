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
        public LoadDialogViewModel LoadDialogView;

        protected override void Awake()
        {
            base.Awake();
            if (LoadDialogView == null)
                Debug.LogError($"Inspector field {nameof(LoadDialogView)} not set.");
        }

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
            // Ask the user what board they want to load.
            LoadDialogView.Open(EditorBoardStorage.ListSavedBoards(), result =>
            {
                if (!result.Canceled)
                {
                    // Load the board and play it.
                    var gameBoard = EditorBoardStorage.LoadBoard(result.Name);
                    GameBoardSceneParameters.SetParameters(new GameBoardSceneParameters()
                    {
                        GameBoard = gameBoard,
                        TimeLimit = (60f * 3f), // 3 minutes
                    });

                    SceneManager.LoadScene("GameBoard");
                }
            });


        }

        #endregion
    }
}