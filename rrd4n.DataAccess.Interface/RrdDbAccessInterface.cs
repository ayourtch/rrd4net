using System;
using System.Collections.Generic;
using System.Text;
using rrd4n.DataAccess.Data;

namespace rrd4n.DataAccess.Interface
{
   public interface RrdDbAccessInterface
   {
      FetchData GetData(FetchRequest request);
      void StoreData(rrd4n.DataAccess.Data.Sample sample);
   }
}
