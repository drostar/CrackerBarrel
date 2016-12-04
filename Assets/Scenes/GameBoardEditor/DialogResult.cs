using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrackerBarrel
{
    public class DialogResult
    {
        public bool Canceled {
            get { return Name == null; }
            set { if (value) Name = null; }
        }
        public string Name { get; set; }
    }
}
