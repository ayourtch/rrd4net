using System;
using System.Collections.Generic;
using System.Text;
using rrd4n.Common;

namespace rrd4n.Data
{
   public class DataShift : DataSource
   {
      private readonly long shiftOffset;
      private readonly long startTime;
      private readonly long endTime;
      private readonly long step;

      public DataShift(string variableName, long shiftOffset)
         :base(variableName)
      {
         this.shiftOffset = shiftOffset;
      }

      public void TimeShiftData(long[] timeStamps)
      {
         //long[] timeStamps = dataSource.getRrdTimestamps();

         for (var i = 0; i < timeStamps.Length; i++ )
         {
            timeStamps[i] += shiftOffset;
         }
      }
   }
}
