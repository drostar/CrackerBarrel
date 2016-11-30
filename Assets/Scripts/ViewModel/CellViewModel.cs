using UnityEngine;
using System.Collections;
using System.Linq;
using Foundation.Databinding;

namespace CrackerBarrel
{
    public class CellViewModel : ObservableBehaviour
    {
        public Cell Cell { get; set; }

        public CellViewModel()
        {
            
        }
    }

}