using System;
using System.Collections.Generic;
using System.Text;

namespace RRDConfigTool.Data
{
   public class FetchedData
   {
      public DateTime TimeStamp { get; set; }
      public List<double> Values { get; set; }
   }
}
