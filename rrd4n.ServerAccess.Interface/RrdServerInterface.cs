using rrd4n.ServerAccess.Data;

namespace rrd4n.ServerAccess.Interface
{
   public interface RrdServerInterface
   {
      RemoteFetchData FetchData(string databaseName, long startTimeTick, long endTimeTick, string consolFunctionName, long resolution);
      void StoreData(string databaseName, long timeStamp, double[] newValues);
   }
}
