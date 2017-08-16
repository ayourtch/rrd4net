using System;
using System.Collections.Generic;
using System.Text;
using rrd4n.Common;

namespace rrd4n.DataAccess.Data
{
   /**
 * Class used to represent data fetched from the RRD.
 * Object of this class is created when the method
 * {@link org.Rrd4n.core.FetchRequest#fetchData() fetchData()} is
 * called on a {@link org.Rrd4n.core.FetchRequest FetchRequest} object.<p>
 *
 * Data returned from the RRD is, simply, just one big table filled with
 * timestamps and corresponding datasource values.
 * Use {@link #getRowCount() getRowCount()} method to count the number
 * of returned timestamps (table rows).<p>
 *
 * The first table column is filled with timestamps. Time intervals
 * between consecutive timestamps are guaranteed to be equal. Use
 * {@link #getTimestamps() getTimestamps()} method to get an array of
 * timestamps returned.<p>
 *
 * Remaining columns are filled with datasource values for the whole timestamp range,
 * on a column-per-datasource basis. Use {@link #getColumnCount() getColumnCount()} to find
 * the number of datasources and {@link #getValues(int) getValues(i)} method to obtain
 * all values for the i-th datasource. Returned datasource values correspond to
 * the values returned with {@link #getTimestamps() getTimestamps()} method.<p>
 *
 */
   public class FetchData
   {
      public string[] DsNames { get; private set; }
      public long[] Timestamps { get; set; }
      public double [][] Values { get; set; }

      private readonly long arcStep;
      private readonly long arcEndTime;

      public FetchData(long arcStep, long arcEndTime, string[] dsNames)
      {
         this.arcStep = arcStep;
         this.arcEndTime = arcEndTime;
         DsNames = dsNames;
      }

      public void setTimestamps(long[] timestamps)
      {
         Timestamps = timestamps;
      }

      public void setValues(double[][] values)
      {
         Values = values;
      }

      /**
       * Returns the number of rows fetched from the corresponding RRD.
       * Each row represents datasource values for the specific timestamp.
       *
       * @return Number of rows.
       */
      public int getRowCount()
      {
         return Timestamps.Length;
      }

      /**
       * Returns the number of columns fetched from the corresponding RRD.
       * This number is always equal to the number of datasources defined
       * in the RRD. Each column represents values of a single datasource.
       *
       * @return Number of columns (datasources).
       */
      public int getColumnCount()
      {
         return DsNames.Length;
      }

      /**
       * Returns an array of timestamps covering the whole range specified in the
       * {@link FetchRequest FetchReguest} object.
       *
       * @return Array of equidistant timestamps.
       */
      public long[] getTimestamps()
      {
         return Timestamps;
      }
      public DateTime[] getDateTimestamps()
      {
         var dateTimestamps = new List<DateTime>(Timestamps.Length);

         for (var i = 0; i < Timestamps.Length; i++)
         {
            dateTimestamps.Add(Util.ConvertToDateTime(Timestamps[i]));
         }
         return dateTimestamps.ToArray();
      }

      /**
       * Returns the step with which this data was fetched.
       *
       * @return Step as long.
       */
      public long getStep()
      {
         return Timestamps[1] - Timestamps[0];
      }

      /**
       * Returns all archived values for a single datasource.
       * Returned values correspond to timestamps
       * returned with {@link #getTimestamps() getTimestamps()} method.
       *
       * @param dsIndex Datasource index.
       * @return Array of single datasource values.
       */
      public double[] getValues(int dsIndex)
      {
         return Values[dsIndex];
      }

      /**
       * Returns all archived values for all datasources.
       * Returned values correspond to timestamps
       * returned with {@link #getTimestamps() getTimestamps()} method.
       *
       * @return Two-dimensional aray of all datasource values.
       */
      public double[][] getValues()
      {
         return Values;
      }

      /**
       * Returns all archived values for a single datasource.
       * Returned values correspond to timestamps
       * returned with {@link #getTimestamps() getTimestamps()} method.
       *
       * @param dsName Datasource name.
       * @return Array of single datasource values.
       */
      public double[] getValues(String dsName)
      {
         for (int dsIndex = 0; dsIndex < getColumnCount(); dsIndex++)
         {
            if (dsName.CompareTo(DsNames[dsIndex]) == 0)
            {
               return getValues(dsIndex);
            }
         }
         throw new ArgumentException("Datasource [" + dsName + "] not found");
      }

      /**
       * Returns the first timestamp in this FetchData object.
       *
       * @return The smallest timestamp.
       */
      public long getFirstTimestamp()
      {
         return Timestamps[0];
      }

      /**
       * Returns the last timestamp in this FecthData object.
       *
       * @return The biggest timestamp.
       */
      public long getLastTimestamp()
      {
         return Timestamps[Timestamps.Length - 1];
      }

      /**
       * Returns array of datasource names found in the corresponding RRD. If the request
       * was filtered (data was fetched only for selected datasources), only datasources selected
       * for fetching are returned.
       *
       * @return Array of datasource names.
       */
      public String[] getDsNames()
      {
         return DsNames;
      }

      /**
       * Retrieve the table index number of a datasource by name.  Names are case sensitive.
       *
       * @param dsName Name of the datasource for which to find the index.
       * @return Index number of the datasources in the value table.
       */
      public int getDsIndex(String dsName)
      {
         // Let's assume the table of dsNames is always small, so it is not necessary to use a hashmap for lookups
         for (int i = 0; i < DsNames.Length; i++)
         {
            if (DsNames[i].CompareTo(dsName) == 0)
            {
               return i;
            }
         }
         return -1;		// Datasource not found !
      }

      /**
       * Dumps the content of the whole FetchData object. Useful for debugging.
       */
      public String dump()
      {
         var buffer = new StringBuilder("");
         for (int row = 0; row < getRowCount(); row++)
         {
            buffer.Append(Timestamps[row]);
            buffer.Append(":  ");
            for (int dsIndex = 0; dsIndex < getColumnCount(); dsIndex++)
            {
               buffer.Append(Util.formatDouble(Values[dsIndex][row], true));
               buffer.Append("  ");
            }
            buffer.Append("\n");
         }
         return buffer.ToString();
      }

      /**
       * Returns string representing fetched data in a RRDTool-like form.
       *
       * @return Fetched data as a string in a rrdfetch-like output form.
       */
      public String toString()
      {
         // print header row
         var buff = new StringBuilder();
         buff.Append(padWithBlanks("", 10));
         buff.Append(" ");
         foreach (String dsName in DsNames)
         {
            buff.Append(padWithBlanks(dsName, 18));
         }
         buff.Append("\n \n");
         for (int i = 0; i < Timestamps.Length; i++)
         {
            buff.Append(padWithBlanks("" + Timestamps[i], 10));
            buff.Append(":");
            for (int j = 0; j < DsNames.Length; j++)
            {
               double value = Values[j][i];
               String valueStr = Double.IsNaN(value) ? "nan" : Util.formatDouble(value);
               buff.Append(padWithBlanks(valueStr, 18));
            }
            buff.Append("\n");
         }
         return buff.ToString();
      }

      private static String padWithBlanks(String input, int width)
      {
         var buff = new StringBuilder("");
         int diff = width - input.Length;
         while (diff-- > 0)
         {
            buff.Append(' ');
         }
         buff.Append(input);
         return buff.ToString();
      }

      /**
       * Returns the step of the corresponding RRA archive
       * @return Archive step in seconds
       */
      public long getArcStep()
      {
         return arcStep;
      }

      /**
       * Returns the timestamp of the last populated slot in the corresponding RRA archive
       * @return Timestamp in seconds
       */
      public long getArcEndTime()
      {
         return arcEndTime;
      }
   }
}
