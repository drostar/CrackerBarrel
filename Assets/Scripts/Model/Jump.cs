using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrackerBarrel
{
    /// <summary>
    /// Records A Move Made During The Game
    /// </summary>
    [Serializable]
    public class Jump
    {
        /// <summary>
        /// The original position of the target peg
        /// </summary>
        public CellPosition FromPosition;

        /// <summary>
        /// The position that the target peg was moved into
        /// </summary>
        public CellPosition ToPosition;

        /// <summary>
        /// The position of the peg that was jumped (and removed)
        /// </summary>
        public CellPosition JumpedPosition;

        /// <summary>
        /// Time elapsed since the beginning of the game that this move was made (in seconds).
        /// </summary>
        public float TimeOffset;

        public Jump()
        {

        }

        public Jump(CellPosition fromPosition, CellPosition toPosition, CellPosition jumpedPosition, float timeOffset)
        {
            this.FromPosition = fromPosition;
            this.ToPosition = toPosition;
            this.JumpedPosition = jumpedPosition;
            this.TimeOffset = timeOffset;
        }
    }
}