using System;
using System.Collections.Generic;
using System.Text;
using rrd4n.Common;

namespace rrd4n.DataAccess.Data
{
   public class FetchRequest
   {
      public string DatabasePath { get; private set; }
      public string ConsolidateFunctionName { get; private set; }

      /**
       * Returns starting timestamp to be used for the fetch request.
       * @return Starting timstamp in seconds.
       */
      public long FetchStart { get; private set; }

      /**
       * Returns ending timestamp to be used for the fetch request.
       * @return Ending timestamp in seconds.
       */
      public long FetchEnd { get; private set; }

      /**
       * Returns fetch resolution to be used for the fetch request.
       * @return Fetch resolution in seconds.
       */
      public long Resolution { get; private set; }

      private String[] filter;

      public FetchRequest(string databasePath, string consolidateFunctionName, DateTime fetchStart, DateTime fetchEnd, long resolution)
         : this(databasePath, consolidateFunctionName, Util.getTimestamp(fetchStart), Util.getTimestamp(fetchEnd), resolution)
      {
      }

      public FetchRequest(string databasePath, string consolidateFunctionName, long fetchStart, long fetchEnd, long resolution)
      {
         if (string.IsNullOrEmpty(consolidateFunctionName)) throw new ArgumentException("Null consolidation function in fetch request");
         if (fetchStart < 0) throw new ArgumentException("Invalid start time in fetch request: " + fetchStart);
         if (fetchEnd < 0) throw new ArgumentException("Invalid end time in fetch request: " + fetchEnd);
         if (fetchStart > fetchEnd) throw new ArgumentException("Invalid start/end time in fetch request: " + fetchStart +
                    " > " + fetchEnd);
         if (resolution <= 0) throw new ArgumentException("Invalid resolution in fetch request: " + resolution);

         DatabasePath = databasePath;
         ConsolidateFunctionName = consolidateFunctionName;
         FetchStart = fetchStart;
         FetchEnd = fetchEnd;
         Resolution = resolution;
      }

      /**
       * Sets request filter in order to fetch data only for
       * the specified array of datasources (datasource names).
       * If not set (or set to null), fetched data will
       * containt values of all datasources defined in the corresponding RRD.
       * To fetch data only from selected
       * datasources, specify an array of datasource names as method argument.
       * @param filter Array of datsources (datsource names) to fetch data from.
       */
      public void setFilter(String[] Filter)
      {
         filter = Filter;
      }

      /**
       * Sets request filter in order to fetch data only for
       * the specified set of datasources (datasource names).
       * If the filter is not set (or set to null), fetched data will
       * containt values of all datasources defined in the corresponding RRD.
       * To fetch data only from selected
       * datasources, specify a set of datasource names as method argument.
       * @param filter Set of datsource names to fetch data for.
       */
      public void setFilter(List<String> Filter)
      {
         filter = Filter.ToArray();
      }

      /**
       * Sets request filter in order to fetch data only for
       * a single datasource (datasource name).
       * If not set (or set to null), fetched data will
       * containt values of all datasources defined in the corresponding RRD.
       * To fetch data for a single datasource only,
       * specify an array of datasource names as method argument.
       * @param filter Array of datsources (datsource names) to fetch data from.
       */
      public void setFilter(String Filter)
      {
         filter = (Filter == null) ? null : (new[] { Filter });
      }

      /**
       * Returns request filter. See {@link #setFilter(String[]) setFilter()} for
       * complete explanation.
       * @return Request filter (array of datasource names), null if not set.
       */
      public String[] getFilter()
      {
         return filter;
      }


      /**
       * Dumps the content of fetch request using the syntax of RRDTool's fetch command.
       * @return Fetch request dump.
       */
      public String dump()
      {
         return "fetch \"" + DatabasePath +
            "\" " + ConsolidateFunctionName + "\n --start " + FetchStart + "\n --end " + FetchEnd +
            (Resolution > 1 ? "\n --resolution " + Resolution : "");
      }
   }
}
