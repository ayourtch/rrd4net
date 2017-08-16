using System;
using rrd4n.Common;
using rrd4n.DataAccess.Data;

namespace rrd4n.Data
{
   class DataDef : DataSource
   {
      private readonly string path;
      private readonly string dsName;
      private readonly string backend;
      private readonly ConsolFun consolFun;
      public long StartTime { get; set; }
      public long EndTime { get; set; }
      public long Step { get; set; }
      public string ReduceCfName { get; set; }
      public FetchData FetchData { get; set; }

      public DataDef(String name, FetchData fetchData)
         : this(name, null, name, null, null)
      {
         setFetchData(fetchData);
      }

      public DataDef(String name, String path, String dsName, ConsolFun consolFunc)
         : this(name, path, dsName, consolFunc, null)
      {
      }

      public DataDef(String name, String path, String dsName, ConsolFun consolFunc, String backend)
         : base(name)
      {
         this.path = path;
         this.dsName = dsName;
         consolFun = consolFunc;
         this.backend = backend;
         StartTime = long.MinValue;
         EndTime = long.MinValue;
         Step = long.MinValue;
         ReduceCfName = string.Empty;
      }

      public DataDef(String name, String path, String dsName, ConsolFun consolFunc, long step, long startTime, long endTime, string reduceName)
         : this(name, path, dsName, consolFunc, null)
      {
         StartTime = startTime;
         EndTime = endTime;
         // Step resolution must be at last 1 (every value)
         Step = Math.Max(1,step);
         ReduceCfName = reduceName;
      }

      public String getPath()
      {
         return path;
      }

      public String getCanonicalPath()
      {
         return Util.getCanonicalPath(path);
      }

      public String getDsName()
      {
         return dsName;
      }

      public ConsolFun getConsolFun()
      {
         return consolFun;
      }

      public String getBackend()
      {
         return backend;
      }

      public bool isCompatibleWith(DataDef def)
      {
         return getCanonicalPath().CompareTo(def.getCanonicalPath()) == 0 
            && getConsolFun() == def.consolFun 
            && StartTime == def.StartTime 
            && EndTime == def.EndTime
            && ((backend == null && def.backend == null) 
               ||(backend != null && def.backend != null 
               && backend.CompareTo(def.backend) == 0));
      }

      public void setFetchData(FetchData fetchData)
      {
         FetchData = fetchData;
      }

      public long[] getRrdTimestamps()
      {
         return FetchData.getTimestamps();
      }

      public double[] getRrdValues()
      {
         return FetchData.getValues(dsName);
      }

      public long getArchiveEndTime()
      {
         return FetchData.getArcEndTime();
      }

      public long getFetchStep()
      {
         return FetchData.getStep();
      }

      public override void setTimestamps(long[] timestamps)
      {
         if (StartTime == long.MinValue)
         {
            base.setTimestamps(timestamps);
            return;
         }
         // ToDo: use graph time stamp until properl initialized
         Step = timestamps[1] - timestamps[0];
         long t1 = Util.normalize(StartTime, Step);
         long t2 = Util.normalize(EndTime, Step);
         if (t2 < EndTime)
         {
            t2 += Step;
         }
         int count = (int)(((t2 - t1) / Step) + 1);
         this.timestamps = new long[count];
         for (int i = 0; i < count; i++)
         {
            this.timestamps[i] = t1;
            t1 += Step;
         }
      }

      override public Aggregates getAggregates(long tStart, long tEnd)
      {
         long[] t = getRrdTimestamps();
         double[] v = getRrdValues();
         if (EndTime == long.MinValue)
            return new Aggregator(t, v).getAggregates(tStart, tEnd);

         return new Aggregator(t, v).getAggregates(StartTime, EndTime);
      }

      override public double getPercentile(long tStart, long tEnd, double percentile)
      {
         long[] t = getRrdTimestamps();
         double[] v = getRrdValues();
         return new Aggregator(t, v).getPercentile(tStart, tEnd, percentile);
      }

      public bool isLoaded()
      {
         return FetchData != null;
      }
   }
}
