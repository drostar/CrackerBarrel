using UnityEngine;
using System.Collections;
using System.Linq;
using Foundation.Databinding;

namespace CrackerBarrel
{
    public class CellViewModel : ObservableBehaviour
    {
        private Cell _cell;
        public Cell Cell { 
            get { return _cell; }
            set {
                if (value == _cell)
                    return;

                _cell = value;
                RaiseBindingUpdate(nameof(Cell), _cell);
            }
        }

        private bool _isHighlighted;
        public bool IsHighlighted {
            get { return _isHighlighted; }
            set {
                if (value == _isHighlighted)
                    return;
                _isHighlighted = value;
                RaiseBindingUpdate(nameof(IsHighlighted), _isHighlighted);
            }
        }
    }

}