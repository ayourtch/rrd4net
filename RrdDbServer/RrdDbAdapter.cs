using System;
using System.Collections.Specialized;
using System.Configuration;
using rrd4n.ServerAccess.Data;
using rrd4n.Core;
using rrd4n.DataAccess.Data;

namespace RrdDbServer
{
   public class RrdDbAdapter : MarshalByRefObject, rrd4n.ServerAccess.Interface.RrdServerInterface
   {
      private static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
      private static readonly bool debugEnabled = log.IsDebugEnabled;

      #region RrdDbInterface Members

      public RemoteFetchData FetchData(string databaseName, long startTimeTick, long endTimeTick, string consolFunctionName, long resolution)
      {
         RrdDb database = null;
         try
         {
            log.DebugFormat("Read data from {0}", databaseName);

            var nameValueCollection = (NameValueCollection)ConfigurationManager.GetSection("rrdbfileserver");
            string dataBasePath = nameValueCollection["databasepath"];

            log.DebugFormat("Database path:{0}",dataBasePath + databaseName);
            database = new RrdDb(dataBasePath + databaseName, true);
            FetchRequest fetchRequest = new FetchRequest(null, consolFunctionName, startTimeTick, endTimeTick, resolution);
            FetchData data = database.fetchData(fetchRequest);
            database.close();

            RemoteFetchData remoteData = new RemoteFetchData();
            remoteData.Timestamps = data.getTimestamps();
            remoteData.Values = data.getValues();
            remoteData.ArchiveEndTimeTicks = data.getArcEndTime();
            remoteData.ArchiveSteps = data.getArcStep();
            remoteData.DatasourceNames = data.getDsNames();
            if (debugEnabled)
               log.DebugFormat("Read data from {0} to {1}.", rrd4n.Common.Util.getDate(startTimeTick), rrd4n.Common.Util.getDate(endTimeTick));
            return remoteData;
         }
         catch (Exception ex)
         {
            log.Error("Fetchdata exception", ex);
            throw;
         }
         finally
         {
            if (database != null)
               database.close();
         }
      }

      public void StoreData(string databaseName, long timeStamp, double[] newValues)
      {
         RrdDb database = null;
         try
         {
            database = new RrdDb(databaseName, false);
            database.store(timeStamp, newValues);
         }
         catch (Exception ex)
         {
            log.Error("Store data exception", ex);
            throw;
         }
         finally
         {
            if (database != null)
               database.close();
         }
      }

      #endregion
   }
}
