using UnityEngine;
using System.Collections;
using System.Linq;
using System;

namespace CrackerBarrel
{
    public class GameBoardSceneParameters
    {
        private static GameBoardSceneParameters parameters;
        public static void SetParameters(GameBoardSceneParameters parameters)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));
            GameBoardSceneParameters.parameters = parameters;
        }
        public static GameBoardSceneParameters GetParameters()
        {
            return GameBoardSceneParameters.parameters;
        }

        public GameBoard GameBoard { get; set; }
        public float TimeLimit { get; set; }
    } 
}
