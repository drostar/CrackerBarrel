using UnityEngine;
using System.Collections;
using Foundation.Databinding;
using System;
using System.IO;

namespace CrackerBarrel
{
    public class EditorUIViewModel : ObservableBehaviour
    {
        public HexBoardEditorViewModel HexBoardView;
        public LoadDialogViewModel LoadDialogView;
        public SaveDialogViewModel SaveDialogView;

        protected override void Awake()
        {
            base.Awake();
            if (HexBoardView == null)
                Debug.LogError($"Inspector field {nameof(HexBoardView)} was not set");
            if (LoadDialogView == null)
                Debug.LogError($"Inspector field {nameof(LoadDialogView)} was not set");
            if (SaveDialogView == null)
                Debug.LogError($"Inspector field {nameof(SaveDialogView)} was not set");
        }

        #region Commands

        public void New()
        {
            HexBoardView.NewBoard();
        }

        public void Save()
        {
            SaveDialogView.Open((result) =>
            {
                if (!result.Canceled)
                {
                    GameBoard gameBoard = HexBoardView.GetCurrentBoard(result.Name);
                    EditorBoardStorage.SaveBoard(gameBoard, result.Name);
                }
            });
        }

        public void Load()
        {
            LoadDialogView.Open(EditorBoardStorage.ListSavedBoards(), (result) =>
            {
                if (!result.Canceled)
                {
                    GameBoard gameBoard = EditorBoardStorage.LoadBoard(result.Name);
                    HexBoardView.LoadBoard(gameBoard);
                }
            });
        }

        #endregion
    }
}
