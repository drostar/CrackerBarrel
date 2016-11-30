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
        public CellPosition FromPosition { get; set; }

        /// <summary>
        /// The position that the target peg was moved into
        /// </summary>
        public CellPosition ToPosition { get; set; }

        /// <summary>
        /// The position of the peg that was jumped (and removed)
        /// </summary>
        public CellPosition JumpedPosition { get; set; }

        /// <summary>
        /// Time elapsed since the beginning of the game that this move was made (in seconds).
        /// </summary>
        public float TimeOffset { get; set; }

        public Jump()
        {

        }

        public Jump(CellPosition fromPosition, CellPosition toPosition)
        {
            throw new NotImplementedException();
        }
    }
}