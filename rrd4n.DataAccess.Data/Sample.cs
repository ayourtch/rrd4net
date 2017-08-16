using System;
using System.Text;
using rrd4n.Common;

namespace rrd4n.DataAccess.Data
{
   /**
    * <p>Class to represent data source values for the given timestamp. Objects of this
    * class are never created directly (no public constructor is provided). To learn more how
    * to update RRDs, see RRDTool's
    * <a href="../../../../man/rrdupdate.html" target="man">rrdupdate man page</a>.
    * <p/>
    * <p>To update a RRD with Rrd4n use the following procedure:</p>
    * <p/>
    * <ol>
    * <li>Obtain empty Sample object by calling method {@link RrdDb#createSample(long)
    * createSample()} on respective {@link RrdDb RrdDb} object.
    * <li>Adjust Sample timestamp if necessary (see {@link #setTime(long) setTime()} method).
    * <li>Supply data source values (see {@link #setValue(String, double) setValue()}).
    * <li>Call Sample's {@link #update() update()} method.
    * </ol>
    * <p/>
    * <p>Newly created Sample object contains all data source values set to 'unknown'.
    * You should specifify only 'known' data source values. However, if you want to specify
    * 'unknown' values too, use <code>Double.NaN</code>.</p>
    *
    * @author Mikael Nilsson
    */
   public class Sample
   {
      public string DatabasePath { get; private set; }
      public long Time { get; set; }
      private readonly String[] dsNames;
      public double[] Values { get; set; }

      public Sample(string databasePath, string[] dsNames, long time)
      {
         DatabasePath = databasePath;
         Time = time;
         this.dsNames = dsNames;
         clearValues();
      }

      public void clearValues()
      {
         Values = Util.FillArray(double.NaN, dsNames.Length);
      }

      /**
       * Sets single data source value in the sample.
       *
       * @param dsName Data source name.
       * @param value  Data source value.
       * @return This <code>Sample</code> object
       * @throws ArgumentException Thrown if invalid data source name is supplied.
       */
      public Sample setValue(String dsName, double value)
      {
         for (int i = 0; i < Values.Length; i++)
         {
            if (dsNames[i].CompareTo(dsName) != 0) continue;
            Values[i] = value;
            return this;
         }
         throw new ArgumentException("Datasource " + dsName + " not found");
      }

      /**
       * Sets single datasource value using data source index. Data sources are indexed by
       * the order specified during RRD creation (zero-based).
       *
       * @param i     Data source index
       * @param value Data source values
       * @return This <code>Sample</code> object
       * @throws ArgumentException Thrown if data source index is invalid.
       */
      public Sample setValue(int i, double value)
      {
         if (i >= Values.Length) throw new ArgumentException("Sample datasource index " + i + " out of bounds");

         Values[i] = value;
         return this;
      }

      /**
       * Sets some (possibly all) data source values in bulk. Data source values are
       * assigned in the order of their definition inside the RRD.
       *
       * @param values Data source values.
       * @return This <code>Sample</code> object
       * @throws ArgumentException Thrown if the number of supplied values is zero or greater
       *                                  than the number of data sources defined in the RRD.
       */
      public Sample setValues(double[] values)
      {
         if (values.Length > Values.Length) throw new ArgumentException("Invalid number of values specified (found " +
                                        values.Length + ", only " + dsNames.Length + " allowed)");

         for (int i = 0; i < values.Length; i++)
            Values[i] = values[i];
         return this;
      }

      /**
       * Returns all current data source values in the sample.
       *
       * @return Data source values.
       */
      public double[] getValues()
      {
         return Values;
      }

      /**
       * Returns sample timestamp (in seconds, without milliseconds).
       *
       * @return Sample timestamp.
       */
      public long getTime()
      {
         return Time;
      }

      /**
       * Sets sample timestamp. Timestamp should be defined in seconds (without milliseconds).
       *
       * @param time New sample timestamp.
       * @return This <code>Sample</code> object
       */
      public Sample setTime(long time)
      {
         Time = time;
         return this;
      }

      public Sample setDateTime(DateTime timeStamp)
      {
         Time = Util.getTimestamp(timeStamp);
         return this;
      }



      /**
       * Dumps sample content using the syntax of RRDTool's update command.
       *
       * @return Sample dump.
       */
      public String dump()
      {
         StringBuilder buffer = new StringBuilder("update \"");
         buffer.Append(DatabasePath).Append("\" ").Append(Time);
         foreach (double value in Values)
         {
            buffer.Append(":");
            buffer.Append(Util.formatDouble(value, "U", false));
         }
         return buffer.ToString();
      }

   }
}
