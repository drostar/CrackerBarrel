using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using Foundation.Databinding;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;


namespace CrackerBarrel
{
    public class ReplaySelectViewModel : ObservableBehaviour
    {
        public ObservableCollection<LoadReplayButtonViewModel> SavedReplays { get; set; } = new ObservableCollection<LoadReplayButtonViewModel>();

        public void Open()
        {
            SavedReplays.Clear();
            foreach (var saveName in ReplayManager.ListSavedReplays())
            {
                SavedReplays.Add(new LoadReplayButtonViewModel() { SaveName = saveName });
            }
            gameObject.SetActive(true);
        }
        public void Close()
        {
            gameObject.SetActive(false);
        }

        public void LoadReplay(Text saveButtonText)
        {
            string saveName = saveButtonText.text;
            GameMoveHistory replay = ReplayManager.LoadReplay(saveName);

            GameBoardSceneParameters.SetParameters(new GameBoardSceneParameters()
            {
                GameBoard = GameBoardData.GetGameBoard(replay.GameBoardData),
                TimeLimit = replay.TimeLimit,
                ReplayHistory = replay,
            });
            SceneManager.LoadScene("GameBoard");
        }
    }
}