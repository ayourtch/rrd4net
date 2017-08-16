using System;
using System.Collections.Generic;
using System.Text;
using rrd4n.Core;

namespace RRDConfigTool.Data
{
   public class DatabaseData
   {
      public RrdDef Definition { get; set; }
      public double LastValue { get; set; }
      public DateTime LastUpdated { get; set; }
      public bool Saved { get; set; }
      public string SourceDatabasePath { get; set; }
   }
}
