using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrackerBarrel
{
    [Serializable]
    public struct CellPosition
    {
        public int X;
        public int Y;

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            CellPosition tobj = (CellPosition)obj;

            return Equals(tobj);
        }

        public bool Equals(CellPosition p2)
        {
            return (p2.X == X && p2.Y == Y);
        }

        public override int GetHashCode()
        {
            // taken from http://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode/263416#263416
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                hash = hash * 23 + X.GetHashCode();
                hash = hash * 23 + Y.GetHashCode();
                return hash;
            }
        }

        public static bool operator ==(CellPosition p1, CellPosition p2)
        {
            return p1.Equals(p2);
        }
        public static bool operator !=(CellPosition p1, CellPosition p2)
        {
            return !p1.Equals(p2);
        }

        public override string ToString()
        {
            return $"({X},{Y})";
        }
    }
}