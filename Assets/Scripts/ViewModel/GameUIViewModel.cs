using UnityEngine;
using System.Collections;
using System.Linq;
using Foundation.Databinding;
using Zenject;
using System;
using System.IO;
using UnityEngine.SceneManagement;
using DG.Tweening;

namespace CrackerBarrel
{
    public class GameUIViewModel : ObservableBehaviour
    {
        [Inject]
        private GameController gameController { get; set; }
        [Inject]
        ReplayManager replayManager;
        [Inject]
        private AudioManager audioManager { get; set; }

        #region Inspector Fields
        public HexBoardViewModel GameBoardViewModel;
        #endregion

        private bool _isGameOutOfMoves;
        public bool IsGameOutOfMoves {
            get { return _isGameOutOfMoves; }
            set {
                if (_isGameOutOfMoves == value)
                    return;
                _isGameOutOfMoves = value;
                RaiseBindingUpdate(nameof(IsGameOutOfMoves), _isGameOutOfMoves);
            }
        }

        private bool _isGameOutOfTime;
        public bool IsGameOutOfTime {
            get { return _isGameOutOfTime; }
            set {
                if (_isGameOutOfTime == value)
                    return;
                _isGameOutOfTime = value;
                RaiseBindingUpdate(nameof(IsGameOutOfTime), _isGameOutOfTime);
            }
        }

        private bool _isGameWon;
        public bool IsGameWon {
            get { return _isGameWon; }
            set {
                if (_isGameWon == value)
                    return;
                _isGameWon = value;
                RaiseBindingUpdate(nameof(IsGameWon), _isGameWon);
            }
        }

        public int TimeRemainingSeconds { get; private set; }
        public string TimeRemainingFormatted { get; private set; }

        private bool _isReplay;
        public bool IsReplay {
            get { return _isReplay; }
            set {
                if (_isReplay == value)
                    return;
                _isReplay = value;
                RaiseBindingUpdate(nameof(IsReplay), _isReplay);
            }
        }

        public bool CanReplayForward { get { return replayManager?.CanMoveForward ?? false; } }
        public bool CanReplayBackward { get { return replayManager?.CanMoveBackward ?? false; } }

        void Start()
        {
            gameController.OnGameEnded += GameController_OnGameEnded;
            IsReplay = GameBoardSceneParameters.GetParameters()?.IsReplay ?? false;
            if (IsReplay)
            {
                gameController.gameObject.SetActive(false);

                // Forward some replayManager events.
                replayManager.OnBindingUpdate += (ObservableMessage msg) => {
                    if (msg.Name == nameof(ReplayManager.CanMoveForward))
                        RaiseBindingUpdate(nameof(CanReplayForward), CanReplayForward);
                    else if (msg.Name == nameof(ReplayManager.CanMoveBackward))
                        RaiseBindingUpdate(nameof(CanReplayBackward), CanReplayBackward);
                };
            }
        }

        void Update()
        {
            if (!IsReplay)
            {
                int secondsLeft;
                if (gameController.GameState == GameController.GameStates.PLAYING)
                    secondsLeft = Mathf.CeilToInt(gameController.TimeLeft);
                else
                    secondsLeft = Mathf.CeilToInt(gameController.TimeLeftAtCompletion);

                updateTimeLeft(secondsLeft);    
            }
            else
            {
                int secondsLeft = Mathf.CeilToInt(replayManager.ReplayHistory.TimeLimit - replayManager.CurrentTime);
                updateTimeLeft(secondsLeft);
            }
        }

        private void updateTimeLeft(int secondsLeft)
        {
            // Reduce unnecessary overhead/garbage by only updating when the full seconds have actually changed.
            if (secondsLeft != TimeRemainingSeconds)
            {
                TimeRemainingSeconds = secondsLeft;
                RaiseBindingUpdate(nameof(TimeRemainingSeconds), TimeRemainingSeconds);

                var timeSpan = TimeSpan.FromSeconds(TimeRemainingSeconds);
                TimeRemainingFormatted = $"{timeSpan.Minutes}:{timeSpan.Seconds:00}";
                RaiseBindingUpdate(nameof(TimeRemainingFormatted), TimeRemainingFormatted);
            }
        }

        private void GameController_OnGameEnded(GameController.GameStates state)
        {
            switch (state)
            {
                case GameController.GameStates.WON:
                    audioManager.PlayWin();
                    IsGameWon = true;
                    break;
                case GameController.GameStates.LOST_OUTOFTIME:
                    audioManager.PlayLoss();
                    IsGameOutOfTime = true;
                    break;
                case GameController.GameStates.LOST_OUTOFMOVES:
                    audioManager.PlayLoss();
                    IsGameOutOfMoves = true;
                    break;
                default:
                    break;
            }
        }

        #region Commands

        public void LoadMainMenu()
        {
            SceneManager.LoadScene("MainMenu");
        }

        public void SaveReplay()
        {
            string saveName = DateTime.Now.ToString("yyyyMMddHHmmss");

            ReplayManager.SaveReplay(gameController.MoveHistory, saveName);

            LoadMainMenu();
        }

        public void MoveReplayForward()
        {
            var move = replayManager.ReplayHistory.Moves[replayManager.CurrentStepIndex];

            var fromCell = replayManager.GameBoard.GetCellAtPosition(move.FromPosition);
            var toCell = replayManager.GameBoard.GetCellAtPosition(move.ToPosition);
            var jumpCell = replayManager.GameBoard.GetCellAtPosition(move.JumpedPosition);

            var fromVM = GameBoardViewModel.GetCellViewModelFor(fromCell);
            var toVM = GameBoardViewModel.GetCellViewModelFor(toCell);
            var jumpVM = GameBoardViewModel.GetCellViewModelFor(jumpCell);

            fromVM.Peg.transform.DOMove(toVM.Peg.transform.position, 0.1f).SetAutoKill().OnKill(() => {
                fromVM.ResetPeg();
                var jump = replayManager.StepForward();
                replayManager.GameBoard.UpdateAvailableMoves();

                RaiseBindingUpdate(nameof(CanReplayForward), CanReplayForward);
                RaiseBindingUpdate(nameof(CanReplayBackward), CanReplayBackward);
            });
        }

        public void MoveReplayBackward()
        {
            var move = replayManager.ReplayHistory.Moves[replayManager.CurrentStepIndex - 1];

            var fromCell = replayManager.GameBoard.GetCellAtPosition(move.FromPosition);
            var toCell = replayManager.GameBoard.GetCellAtPosition(move.ToPosition);
            var jumpCell = replayManager.GameBoard.GetCellAtPosition(move.JumpedPosition);

            var fromVM = GameBoardViewModel.GetCellViewModelFor(fromCell);
            var toVM = GameBoardViewModel.GetCellViewModelFor(toCell);
            var jumpVM = GameBoardViewModel.GetCellViewModelFor(jumpCell);

            // Position peg in 'finished' position so we can rewind it by animation
            fromVM.Peg.transform.position = toVM.Peg.transform.position;
            var originalPos = fromVM.originalLocalPosition;

            // Update game board
            replayManager.StepBackward();

            RaiseBindingUpdate(nameof(CanReplayForward), CanReplayForward);
            RaiseBindingUpdate(nameof(CanReplayBackward), CanReplayBackward);

            // Move the peg back to original location.
            fromVM.Peg.transform.DOLocalMove(originalPos, 0.1f);
        }

        #endregion
    }

}