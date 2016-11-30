using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrackerBarrel
{

    [Serializable]
    public class InvalidMoveException : Exception
    {
        public InvalidMoveException() { }
        public InvalidMoveException(string message) : base(message) { }
        public InvalidMoveException(string message, Exception inner) : base(message, inner) { }
    }
}
