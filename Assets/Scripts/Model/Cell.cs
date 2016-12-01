using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CrackerBarrel
{
    public class Cell : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private bool _hasPeg;
        public bool HasPeg { 
            get { return _hasPeg; }
            set {
                if (value == _hasPeg)
                    return;
                _hasPeg = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasPeg)));
            }
        }

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