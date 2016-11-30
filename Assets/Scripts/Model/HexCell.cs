using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrackerBarrel
{
    public class HexCell
    {
        public bool HasPeg { get; set; }
        public bool IsCornerCell { get; set; }
        public CellPosition Position { get; set; }

        public HexCell()
        {

        }

        public HexCell(CellPosition position)
        {
            this.Position = position;
        }
    }
}