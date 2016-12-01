﻿using UnityEngine;
using System.Collections;
using System.Linq;
using Foundation.Databinding;
using Zenject;
using System;

namespace CrackerBarrel
{
    public class GameUIViewModel : ObservableBehaviour
    {
        [Inject]
        private GameController gameController { get; set; }

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

        void Start()
        {
            gameController.OnGameEnded += GameController_OnGameEnded;
        }

        void Update()
        {
            updateTimeLeft();
        }

        private void updateTimeLeft()
        {
            int secondsLeft = Mathf.CeilToInt(gameController.TimeLeft);
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
                    IsGameWon = true;
                    break;
                case GameController.GameStates.LOST_OUTOFTIME:
                    IsGameOutOfTime = true;
                    break;
                case GameController.GameStates.LOST_OUTOFMOVES:
                    IsGameOutOfMoves = true;
                    break;
                default:
                    break;
            }
        }
    }

}