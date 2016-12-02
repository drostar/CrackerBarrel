using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Foundation.Databinding;
using UnityEngine;

namespace CrackerBarrel
{
    public class ReplayManager : ObservableBehaviour
    {
        public bool CanMoveForward { get { return CurrentStepIndex < (ReplayHistory?.Moves?.Count ?? 0); } }
        public bool CanMoveBackward { get { return (CurrentStepIndex - 1) >= 0; } }

        /// <summary>
        /// Indicates the next move index to be used on "MoveForward"
        /// </summary>
        public int CurrentStepIndex { get; set; }

        /// <summary>
        /// The replay time of the last executed step.
        /// </summary>
        public float CurrentTime { get; set; }

        public GameMoveHistory ReplayHistory { get; set; }
        public GameBoard GameBoard { get; set; }

        public void InitializeWithReplay(GameMoveHistory replayHistory)
        {
            ReplayHistory = replayHistory;
            GameBoard = GameBoardData.GetGameBoard(replayHistory.GameBoardData);
            GameBoard.UpdateAvailableMoves();

            CurrentStepIndex = 0;
            notifyPropertiesChangedForMove();
        }

        public Jump StepForward()
        {
            if (!CanMoveForward)
                return null;

            var jump = replayMove(CurrentStepIndex);
            CurrentStepIndex++;
            notifyPropertiesChangedForMove();

            return jump;
        }

        public Jump StepBackward()
        {
            if (!CanMoveBackward)
                return null;
            
            var jump = undoMove(CurrentStepIndex - 1);
            CurrentStepIndex--;
            notifyPropertiesChangedForMove();

            return jump;
        }

        private void notifyPropertiesChangedForMove()
        {
            RaiseBindingUpdate(nameof(CanMoveForward), CanMoveForward);
            RaiseBindingUpdate(nameof(CanMoveBackward), CanMoveBackward);
            RaiseBindingUpdate(nameof(CurrentTime), CurrentTime);
            RaiseBindingUpdate(nameof(CurrentStepIndex), CurrentStepIndex);
        }

        private Jump replayMove(int moveIndex)
        {
            Jump move = ReplayHistory.Moves[moveIndex];

            var fromCell = GameBoard.GetCellAtPosition(move.FromPosition);
            var toCell = GameBoard.GetCellAtPosition(move.ToPosition);

            GameBoard.ExecuteJump(fromCell, toCell, move.TimeOffset);
            GameBoard.UpdateAvailableMoves();

            CurrentTime = move.TimeOffset;

            return move;
        }

        private Jump undoMove(int lastExecutedMoveIndex)
        {
            Jump reversedMove = ReplayHistory.Moves[lastExecutedMoveIndex];

            GameBoard.UndoJump(reversedMove);
            GameBoard.UpdateAvailableMoves();

            // update the sim time to the time of the step *before* the step be just undid.
            CurrentTime = lastExecutedMoveIndex >= 1 ? ReplayHistory.Moves[lastExecutedMoveIndex - 1].TimeOffset : 0f;

            return reversedMove;
        }

        #region Static - Load and Save
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
        #endregion
    }
}
