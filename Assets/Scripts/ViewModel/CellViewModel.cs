using UnityEngine;
using System.Collections;
using System.Linq;
using Foundation.Databinding;
using System;
using DG.Tweening;

namespace CrackerBarrel
{
    public class CellViewModel : ObservableBehaviour
    {
        public GameObject Peg;
        public float PegPullAnimationDuration = 0.2f;
        public float PegMoveAnimationDuration = 0.2f;

        private Cell _cell;
        public Cell Cell { 
            get { return _cell; }
            set {
                if (value == _cell)
                    return;

                _cell = value;
                RaiseBindingUpdate(nameof(Cell), _cell);
            }
        }

        private bool _isHighlighted;
        public bool IsHighlighted {
            get { return _isHighlighted; }
            set {
                if (value == _isHighlighted)
                    return;
                _isHighlighted = value;
                RaiseBindingUpdate(nameof(IsHighlighted), _isHighlighted);
            }
        }

        private bool _isSelected;
        public bool IsSelected {
            get { return _isSelected; }
            private set {
                if (value == _isSelected)
                    return;
                _isSelected = value;
                RaiseBindingUpdate(nameof(IsSelected), _isSelected);
            }
        }

        private bool _isExpanderCell;
        /// <summary>
        /// Used by editor to indicate this is a cell that can be added to the board by clicking it.
        /// </summary>
        public bool IsExpanderCell {
            get { return _isExpanderCell; }
            set {
                if (value == _isExpanderCell)
                    return;
                _isExpanderCell = value;
                RaiseBindingUpdate(nameof(IsExpanderCell), _isExpanderCell);
            }
        }

        private bool isOriginalPositionCaptured = false;
        public Vector3 originalLocalPosition { get; private set; }
        public Vector3 originalLocalScale { get; private set; }
        
        #region A bit of hackery because DOTween is a bit buggy :(
        private Sequence runningTween = null;
        private void clearRunningTween()
        {
            runningTween = null;
        }
        private void captureOriginalPositionIfNeeded()
        {
            if (!isOriginalPositionCaptured)
            {
                originalLocalPosition = Peg.transform.localPosition;
                originalLocalScale = Peg.transform.localScale;
                isOriginalPositionCaptured = true;
            }
        }
        private void restoreOriginalPostion()
        {
            clearRunningTween();
            if (isOriginalPositionCaptured)
            {
                Peg.transform.localPosition = originalLocalPosition;
                Peg.transform.localScale = originalLocalScale;
            }
        } 
        #endregion

        public void Initialize()
        {
            captureOriginalPositionIfNeeded();
        }

        public void SelectCell(Vector3 holdWorldPosition)
        {
            // bail out to prevent animation from running again
            if (IsSelected)
                return;

            IsSelected = true;
            IsHighlighted = false; // Highlighting isn't relevent if we're already selected.

            // trigger animation. There are better ways to animate, but this is good enough for now.
            captureOriginalPositionIfNeeded();
            var t = Peg.transform;
            runningTween?.Kill(false);
            runningTween = DOTween.Sequence()
                .Append(t.DOScale(1.5f * Vector3.one, PegPullAnimationDuration))
                .Append(t.DOMove(holdWorldPosition, PegMoveAnimationDuration))
                .OnComplete(clearRunningTween);
        }

        public void DeselectCell()
        {
            // bail out to prevent animation from running again
            if (!IsSelected)
                return;

            IsSelected = false;

            // trigger animation. There are better ways to animate, but this is good enough for now.
            var t = Peg.transform;
            runningTween?.Kill(false);
            DOTween.Sequence()
                .Append(t.DOLocalMove(originalLocalPosition, PegMoveAnimationDuration))
                .Append(t.DOScale(originalLocalScale, PegPullAnimationDuration))
                .OnComplete(restoreOriginalPostion);
        }

        public void JumpPegTo(CellViewModel selectedCell, Action callback)
        {
            var targetCell = selectedCell.Cell;
            var targetPeg = selectedCell.Peg;
            var targetWorldPosition = targetPeg.transform.position;

            // trigger animation. There are better ways to animate, but this is good enough for now.
            var t = Peg.transform;
            runningTween?.Kill(true);
            runningTween = DOTween.Sequence()
                // move 'from' peg over top of the 'to' peg
                .Append(t.DOMove(targetWorldPosition, PegMoveAnimationDuration))
                // shrink the 'from' peg
                .Append(t.DOScale(targetPeg.transform.localScale, PegPullAnimationDuration))
                .AppendCallback(() => callback?.Invoke())
                .OnComplete(clearRunningTween);
                
        }

        public void ResetPeg()
        {
            restoreOriginalPostion();
        }
    }

}