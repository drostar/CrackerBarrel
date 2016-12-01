using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrackerBarrel
{
    [Serializable]
    public class GameMoveHistory
    {
        /// <summary>
        /// The moves that a player has executed since the beginning of the game.
        /// </summary>
        public List<Jump> Moves = new List<Jump>();


        /// <summary>
        /// The game board data that the moves history is relevenat to.
        /// This allows a save file to load an arbitrary game board with the moves history.
        /// </summary>
        public GameBoardData GameBoardData;


        /// <summary>
        /// The original time limit (in seconds);
        /// </summary>
        public float TimeLimit;
    }
}