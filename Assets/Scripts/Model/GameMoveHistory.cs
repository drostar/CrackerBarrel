using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrackerBarrel
{
    [Serializable()]
    public class GameMoveHistory
    {
        /// <summary>
        /// The moves that a player has executed since the beginning of the game.
        /// </summary>
        public List<Jump> Moves { get; set; } = new List<Jump>();
    }
}