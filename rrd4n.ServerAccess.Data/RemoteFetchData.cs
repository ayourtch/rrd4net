using System;
using System.Collections.Generic;
using System.Text;

namespace rrd4n.ServerAccess.Data
{
   [Serializable]
   public class RemoteFetchData
   {
      public long[] Timestamps { get; set; }
      public double[][] Values { get; set; }
      public long ArchiveSteps { get; set; }
      public long ArchiveEndTimeTicks { get; set; }
      public string[] DatasourceNames { get; set; }
   }
}
