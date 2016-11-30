using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrackerBarrel
{
    public class Cell
    {
        public bool HasPeg { get; set; }
        public bool CanPegMove { get; set; }
        public bool IsCornerCell { get; set; }
        public CellPosition Position { get; set; }

        public Cell()
        {

        }

        public Cell(CellPosition position)
        {
            this.Position = position;
        }
    }
}