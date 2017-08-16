using System;
using System.Collections.Generic;
using System.Text;
using rrd4n.Common;
using rrd4n.Core;
using rrd4n.DataAccess.Data;
using rrd4n.DataAccess.Interface;

namespace rrd4n.DataAccess.LocalFile
{
   public class FileAccessor : RrdDbAccessInterface
   {
      public string DataPath { get; set; }

      #region RrdDbAccessInterface Members

      public rrd4n.DataAccess.Data.FetchData GetData(rrd4n.DataAccess.Data.FetchRequest request)
      {
         RrdDb rrdDb = null;
         try
         {
            string dataPath;
            if (DataPath.Contains("${APPDATA}"))
            {
               dataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
               dataPath += DataPath.Substring(10);
            }
            else
               dataPath = DataPath;


            rrdDb = new RrdDb(dataPath + request.DatabasePath);
            FetchRequest coreRequest = new FetchRequest(rrdDb.getPath(), request.ConsolidateFunctionName, request.FetchStart, request.FetchEnd, request.Resolution);
            FetchData coreFetchData = rrdDb.fetchData(coreRequest);
            
            return new rrd4n.DataAccess.Data.FetchData(coreFetchData.getArcStep(), coreFetchData.getArcEndTime(), coreFetchData.getDsNames())
                                                           {
                                                              Values = coreFetchData.getValues(),
                                                              Timestamps = coreFetchData.getTimestamps()
                                                           };
         }
         finally
         {
            if (rrdDb != null)
               rrdDb.close();
         }
      }

      public void StoreData(rrd4n.DataAccess.Data.Sample sample)
      {
         RrdDb rrdDb = null;
         try
         {
            rrdDb = new RrdDb(DataPath + sample.DatabasePath, false);
            Sample coreSample = new Sample(rrdDb.getPath(),rrdDb.getDsNames(), sample.Time);
            coreSample.setValues(sample.Values);
            rrdDb.store(coreSample);
         }
         finally
         {
            if (rrdDb != null)
               rrdDb.close();
         }
      }

      #endregion
   }
}
