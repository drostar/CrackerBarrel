using Foundation.Databinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Zenject;

namespace CrackerBarrel
{
    public class GameController : ObservableBehaviour
    {
        #region Injected Components

        [Inject]
        InputManager inputManager;

        [Inject]
        AudioManager audioManager;

        #endregion

        /// <summary>
        /// The game engine time (seconds) at which the player has started the round.
        /// </summary>
        public float StartTime { get; private set; }
        /// <summary>
        /// The time limit (in seconds) the player has to win the game before automatically losing.
        /// </summary>
        public float TimeLimit { get; private set; }
        /// <summary>
        /// The time the player has played this round so far, in seconds.
        /// </summary>
        public float ElapsedTime { get { return Mathf.Min(Time.timeSinceLevelLoad - StartTime, TimeLimit); } }
        /// <summary>
        /// Time time (in seconds) at which the player won.
        /// </summary>
        public float TimeLeftAtCompletion { get; private set; }

        /// <summary>
        /// The time the player has left before they automatically lose, in seconds.
        /// </summary>
        public float TimeLeft { get { return Mathf.Max(TimeLimit - ElapsedTime, 0); } }

        public GameMoveHistory MoveHistory { get; set; } = new GameMoveHistory();
        public GameBoard GameBoard { get; set; }

        private CellViewModel _selectedCell;
        public CellViewModel SelectedCell {
            get { return _selectedCell; }
            set
            {
                if (_selectedCell == value)
                    return;
                _selectedCell = value;
                RaiseBindingUpdate(nameof(SelectedCell), _selectedCell);
            }
        }

        public enum GameStates { PLAYING, WON, LOST_OUTOFTIME, LOST_OUTOFMOVES }
        private GameStates _gameState;
        public GameStates GameState {
            get { return _gameState; }
            set {
                if (_gameState == value)
                    return;
                _gameState = value;
                RaiseBindingUpdate(nameof(GameState), _gameState);
            }
        }

        #region Inspector Fields

        public Transform PegHoldPosition;

        #endregion

        #region Events

        public event Action<GameStates> OnGameEnded;

        #endregion

        #region Unity Message Handlers

        void Start()
        {
            inputManager.OnSelectObject += InputManager_OnSelectObject;
            inputManager.OnObjectHighlightChanged += InputManager_OnObjectHighlightChanged;
        }

        void Update()
        {
            if (ElapsedTime >= TimeLimit)
                triggerLossByOutOfTime();
        }

        #endregion

        #region Input Manager Event Handlers

        void InputManager_OnSelectObject(GameObject obj)
        {
            CellViewModel cellVM = obj == null ? null : obj.GetComponent<CellViewModel>();

            // cellVM will be null to signal something other than a GameObject was clicked
            if (cellVM == null)
            {
                clearSelectedCell();
            }
            else if (SelectedCell == null) 
            {
                // If no current selection, select the activated object if it has valid moves
                if (cellVM.Cell.CanPegMove)
                {
                    selectCell(cellVM);
                }
                else
                {
                    clearSelectedCell();
                }
            }
            else
            {
                // If there is a current selection then move the peg to the activated cell (if possible)
                if (GameBoard.IsValidMove(SelectedCell.Cell, cellVM.Cell))
                {
                    jump(SelectedCell, cellVM);
                }
                else
                {
                    clearSelectedCell();
                }
            }
        }

        void InputManager_OnObjectHighlightChanged(bool isHighlighted, GameObject obj)
        {
            CellViewModel cellVM = obj.GetComponent<CellViewModel>();

            // If no current selection, highlight cells with pegs that can move
            if (SelectedCell == null)
            {
                cellVM.IsHighlighted = isHighlighted && cellVM.Cell.CanPegMove;
            }
            // With a current selection, highlight cell where the selected peg can move to
            else
            {
                // OPTIMIZE: Cache this since it only needs to be calculated once after the cell is selected.
                cellVM.IsHighlighted = isHighlighted && GameBoard.IsValidMove(SelectedCell.Cell, cellVM.Cell);
            }

            // Audio feedback of highlight
            if (cellVM.IsHighlighted)
            {
                audioManager.PlayHighlight();
            }
        }

        #endregion

        #region Selection

        private void selectCell(CellViewModel cellVM)
        {
            // Deselect old cell first
            if (SelectedCell != null && SelectedCell != cellVM)
            {
                SelectedCell.DeselectCell();
            }
            // Don't reselect the same cell. Treat it as a deselect instead and bail out.
            if (SelectedCell == cellVM)
            {
                clearSelectedCell();
                return;
            }

            // Note the select and signal the cell's viewmodel that it make the cell/peg look selected.
            SelectedCell = cellVM;
            var holdPosition = PegHoldPosition.position;
            cellVM.SelectCell(holdPosition);

            // Audio feedback of select
            audioManager.PlaySelect();
        }

        private void clearSelectedCell()
        {
            if (SelectedCell != null)
            {
                SelectedCell.DeselectCell();
                SelectedCell = null;
            }
        }

        #endregion

        public void InitializeWithBoard(GameBoard gameBoard, float timeLimitSeconds)
        {
            inputManager.DisableInput = false;
            StartTime = Time.timeSinceLevelLoad; // capture start time
            TimeLimit = timeLimitSeconds;
            GameBoard = gameBoard;
            GameBoard.UpdateAvailableMoves();
        }

        private void jump(CellViewModel fromCellVM, CellViewModel toCellVM)
        {
            var fromCell = fromCellVM.Cell;
            var toCell = toCellVM.Cell;

            inputManager.DisableInput = true;

            audioManager.PlayMove();

            // Animate selected peg to 'to' cell
            // TODO: Make this not a callback but either a coroutine or async function
            fromCellVM.JumpPegTo(toCellVM, () =>
            {
                // Update the game board with the Jump
                var jump = GameBoard.ExecuteJump(fromCell, toCell, ElapsedTime);

                // Return the 'from' cell's peg to its own cell
                fromCellVM.ResetPeg();

                // Add move to history
                MoveHistory.Moves.Add(jump);

                // Update cell valid move states after this move
                GameBoard.UpdateAvailableMoves();

                inputManager.DisableInput = false;
                clearSelectedCell();

                // Trigger win/loss if this jump is the last possible.
                checkForWinLose();
            });
        }

        private void checkForWinLose()
        {
            int pegsLeft = GameBoard.HexCells.Count(x => x.HasPeg);
            // A win is when there is only 1 peg left.
            if (pegsLeft == 1)
            {
                triggerWin();
            }
            // A loss is when there is more than 1 peg left and there are no more valid moves
            else if (pegsLeft > 1 && !GameBoard.HexCells.Any(x => x.CanPegMove))
            {
                triggerLossByOutOfMoves();
            }
        }
        
        private void triggerWin()
        {
            endGameCleanup();
            OnGameEnded?.Invoke(GameStates.WON);
        }

        private void triggerLossByOutOfMoves()
        {
            endGameCleanup();
            OnGameEnded?.Invoke(GameStates.LOST_OUTOFMOVES);
        }

        private void triggerLossByOutOfTime()
        {
            endGameCleanup();
            OnGameEnded?.Invoke(GameStates.LOST_OUTOFTIME);
        }

        private void endGameCleanup()
        {
            TimeLeftAtCompletion = TimeLeft;
            inputManager.DisableInput = true;

            // Save replay
            
        }

    }
}
