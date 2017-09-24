using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Btr
{
    public class BaseSettings
    {
        public double Delta { get; set;}
        public double GGap { get; set; } // 
        public TimeSpan Tbase { get; set; } 
        public TimeSpan T1 { get; set; }
    }
}
