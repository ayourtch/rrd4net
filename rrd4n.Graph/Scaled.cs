using System;
using System.Collections.Generic;
using System.Text;

namespace rrd4n.Graph
{
   public class Scaled
    {
        public  double value;
        public String unit;

        public Scaled(double value, String unit)
        {
            this.value = value;
            this.unit = unit;
        }

        void dump()
        {
            //System.out.println("[" + value + unit + "]");
        }
    }
}
