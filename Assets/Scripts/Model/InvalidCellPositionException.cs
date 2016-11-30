using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrackerBarrel
{
    [Serializable]
    public class InvalidCellPositionException : Exception
    {
        public InvalidCellPositionException() { }
        public InvalidCellPositionException(string message) : base(message) { }
        public InvalidCellPositionException(string message, Exception inner) : base(message, inner) { }
        public InvalidCellPositionException(CellPosition position) : base($"Invalid Position {position}.") { }
        public InvalidCellPositionException(CellPosition position, string message) : base($"Invalid Position {position}. {message}") { }
    }
}
