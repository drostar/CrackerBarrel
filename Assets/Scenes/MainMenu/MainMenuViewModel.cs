using UnityEngine;
using System.Collections;
using Foundation.Databinding;
using System.Linq;
using UnityEngine.SceneManagement;

namespace CrackerBarrel
{
    public class MainMenuViewModel : ObservableBehaviour
    {
        public DifficultySelectViewModel DifficultySelectView;
        public ReplaySelectViewModel ReplaySelectView;

        #region Commands

        public void OpenDifficultySelect()
        {
            DifficultySelectView.Open();
        }

        public void OpenReplaySelect()
        {
            ReplaySelectView.Open();
        }

        #endregion
    }

}