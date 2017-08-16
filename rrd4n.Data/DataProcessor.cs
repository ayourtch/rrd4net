using System;
using System.Collections.Generic;
using System.Text;
using rrd4n.Common;
using rrd4n.DataAccess.Data;
using rrd4n.DataAccess.Interface;

namespace rrd4n.Data
{
   /* ============================================================
    * Rrd4n : Pure c# implementation of RRDTool's functionality
    * ============================================================
    *
    * Project Info:  http://minidev.se
    * Project Lead:  Mikael Nilsson (info@minidev.se)
    *
    * (C) Copyright 2009-2010, by Mikael Nilsson.
    *
    * Developers:    Mikael Nilsson
    *
    *
    * This library is free software; you can redistribute it and/or modify it under the terms
    * of the GNU Lesser General Public License as published by the Free Software Foundation;
    * either version 2.1 of the License, or (at your option) any later version.
    *
    * This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
    * without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
    * See the GNU Lesser General Public License for more details.
    *
    * You should have received a copy of the GNU Lesser General Public License along with this
    * library; if not, write to the Free Software Foundation, Inc., 59 Temple Place, Suite 330,
    * Boston, MA 02111-1307, USA.
    */


   /**
    * Class which should be used for all calculations based on the data fetched from RRD files. This class
    * supports ordinary DEF datasources (defined in RRD files), CDEF datasources (RPN expressions evaluation),
    * SDEF (static datasources - extension of Rrd4n) and PDEF (plottables, see
    * {@link org.Rrd4n.data.Plottable Plottable} for more information.<p>
    *
    * Typical class usage:<p>
    * <pre>
    * final long t1 = []
    * final long t2 = []
    * DataProcessor dp = new DataProcessor(t1, t2);
    * // DEF datasource
    * dp.addDatasource("x", "demo.rrd", "some_source", "AVERAGE");
    * // DEF datasource
    * dp.addDatasource("y", "demo.rrd", "some_other_source", "AVERAGE");
    * // CDEF datasource, z = (x + y) / 2
    * dp.addDatasource("z", "x,y,+,2,/");
    * // ACTION!
    * dp.processData();
    * // Dump calculated values
    * System.out.println(dp.dump());
    * </pre>
    */
   public class DataProcessor
   {
      public RrdDbAccessInterface DbAccessInterface { get; private set; }

      /**
       * Constant representing the default number of pixels on a Rrd4n graph (will be used if
       * no other value is specified with {@link #setStep(long) setStep()} method.
       */
      public static readonly int DEFAULT_PIXEL_COUNT = 600;
      private static readonly double DEFAULT_PERCENTILE = 95.0; // %
      public int PixelCount { get; set; }

      private readonly long tStart;
      private long tEnd;
      long[] timestamps;
      // this will be adjusted later
      public long Step { get; set; }
      // resolution to be used for RRD fetch operation
      private long FetchRequestResolution { get; set; }

      // the order is important, ordinary HashMap is unordered
      private List<DataSource> sources = new List<DataSource>();
      private List<DataShift> shifts = new List<DataShift>();

      private DataDef[] defSources;

      /**
       * Creates new DataProcessor object for the given time span. Ending timestamp may be set to zero.
       * In that case, the class will try to find the optimal ending timestamp based on the last update time of
       * RRD files processed with the {@link #processData()} method.
       *
       * @param t1 Starting timestamp in seconds without milliseconds
       * @param t2 Ending timestamp in seconds without milliseconds
       */
      public DataProcessor(long t1, long t2, RrdDbAccessInterface rrdDbAccess)
      {
         if ((t1 >= t2 || t1 <= 0 || t2 <= 0) && (t1 <= 0 || t2 != 0))
            throw new ArgumentException("Invalid timestamps specified: " + t1 + ", " + t2);

         tStart = t1;
         tEnd = t2;
         PixelCount = DEFAULT_PIXEL_COUNT;
         FetchRequestResolution = 1;
         DbAccessInterface = rrdDbAccess;
      }

      /**
       * Creates new DataProcessor object for the given time span. Ending date may be set to null.
       * In that case, the class will try to find optimal ending date based on the last update time of
       * RRD files processed with the {@link #processData()} method.
       *
       * @param d1 Starting date
       * @param d2 Ending date
       */
      public DataProcessor(DateTime d1, DateTime d2, RrdDbAccessInterface rrdDbAccess)
         : this(Util.getTimestamp(d1), d2 != null ? Util.getTimestamp(d2) : 0,rrdDbAccess)
      {      }

      public long StartTime
      {
         get { return tStart; }
      }
      public long EndTime
      {
         get { return tEnd; }
      }

      /**
       * Sets the number of pixels (target graph width). This number is used only to calculate pixel coordinates
       * for Rrd4n graphs (methods {@link #getValuesPerPixel(String)} and {@link #getTimestampsPerPixel()}),
       * but has influence neither on datasource values calculated with the
       * {@link #processData()} method nor on aggregated values returned from {@link #getAggregates(String)}
       * and similar methods. In other words, aggregated values will not change once you decide to change
       * the dimension of your graph.<p>
       *
       * The default number of pixels is defined by constant {@link #DEFAULT_PIXEL_COUNT}
       * and can be changed with a {@link #setPixelCount(int)} method.
       *
       * @param pixelCount The number of pixels. If you process RRD data in order to display it on the graph,
       *                   this should be the width of your graph.
       */
      public void setPixelCount(int pixelCount)
      {
         PixelCount = pixelCount;
      }

      /**
       * Returns the number of pixels (target graph width). See {@link #setPixelCount(int)} for more information.
       * @return Target graph width
       */
      public int getPixelCount()
      {
         return PixelCount;
      }

      /**
       * Roughly corresponds to the --step option in RRDTool's graph/xport commands. Here is an explanation borrowed
       * from RRDTool:<p>
       *
       * <i>"By default rrdgraph calculates the width of one pixel in the time
       * domain and tries to get data at that resolution from the RRD. With
       * this switch you can override this behavior. If you want rrdgraph to
       * get data at 1 hour resolution from the RRD, then you can set the
       * step to 3600 seconds. Note, that a step smaller than 1 pixel will
       * be silently ignored."</i><p>
       *
       * I think this option is not that useful, but it's here just for compatibility.<p>
       *
       * @param step Time step at which data should be fetched from RRD files. If this method is not used,
       *             the step will be equal to the smallest RRD step of all processed RRD files. If no RRD file is processed,
       *             the step will be roughly equal to the with of one graph pixel (in seconds).
       */
      public void setStep(long step)
      {
         Step = step;
      }

      /**
       * Returns the time step used for data processing. Initially, this method returns zero.
       * Once {@link #processData()} is finished, the method will return the real value used for
       * all internal computations. Roughly corresponds to the --step option in RRDTool's graph/xport commands.
       *
       * @return Step used for data processing.
       */
      public long getStep()
      {
         return Step;
      }

      /**
       * Returns desired RRD archive step (reslution) in seconds to be used while fetching data
       * from RRD files. In other words, this value will used as the last parameter of
       * {@link RrdDb#createFetchRequest(ConsolFun, long, long, long) RrdDb.createFetchRequest()} method
       * when this method is called internally by this DataProcessor.
       *
       * @return Desired archive step (fetch resolution) in seconds.
       */
      public long getFetchRequestResolution()
      {
         return FetchRequestResolution;
      }

      /**
       * Sets desired RRD archive step in seconds to be used internally while fetching data
       * from RRD files. In other words, this value will used as the last parameter of
       * {@link RrdDb#createFetchRequest(ConsolFun, long, long, long) RrdDb.createFetchRequest()} method
       * when this method is called internally by this DataProcessor. If this method is never called, fetch
       * request resolution defaults to 1 (smallest possible archive step will be chosen automatically).
       *
       * @param fetchRequestResolution Desired archive step (fetch resoltuion) in seconds.
       */
      public void setFetchRequestResolution(long fetchRequestResolution)
      {
         FetchRequestResolution = fetchRequestResolution;
      }

      /**
       * Returns ending timestamp. Basically, this value is equal to the ending timestamp
       * specified in the constructor. However, if the ending timestamps was zero, it
       * will be replaced with the real timestamp when the {@link #processData()} method returns. The real
       * value will be calculated from the last update times of processed RRD files.
       *
       * @return Ending timestamp in seconds
       */
      public long getEndingTimestamp()
      {
         return tEnd;
      }

      /**
       * Returns consolidated timestamps created with the {@link #processData()} method.
       *
       * @return array of timestamps in seconds
       */
      public long[] getTimestamps()
      {
         if (timestamps == null) throw new ArgumentException("Timestamps not calculated yet");
         
         return timestamps;
      }

      public long[] getTimestamps(String sourceName)
      {
         DataSource source = getSource(sourceName);
         long[] ts = source.getTimestamps();
         if (ts == null) throw new ArgumentException("Values not available for source [" + sourceName + "]");

         return ts;
      }
      /**
       * Returns calculated values for a single datasource. Corresponding timestamps can be obtained from
       * the {@link #getTimestamps()} method.
       *
       * @param sourceName Datasource name
       * @return an array of datasource values
       * @throws ArgumentException Thrown if invalid datasource name is specified,
       *                      or if datasource values are not yet calculated (method {@link #processData()}
       *                      was not called)
       */
      public double[] getValues(String sourceName)
      {
         DataSource source = getSource(sourceName);
         double[] values = source.getValues();
         if (values == null)
         {
            throw new ArgumentException("Values not available for source [" + sourceName + "]");
         }
         return values;
      }

      /**
       * This method is just an alias for {@link #getPercentile(String)} method.
       *
       * Used by ISPs which charge for bandwidth utilization on a "95th percentile" basis.<p>
       *
       * The 95th percentile is the highest source value left when the top 5% of a numerically sorted set
       * of source data is discarded. It is used as a measure of the peak value used when one discounts
       * a fair amount for transitory spikes. This makes it markedly different from the average.<p>
       *
       * Read more about this topic at
       * <a href="http://www.red.net/support/resourcecentre/leasedline/percentile.php">Rednet</a> or
       * <a href="http://www.bytemark.co.uk/support/tech/95thpercentile.html">Bytemark</a>.
       *
       * @param sourceName Datasource name
       * @return 95th percentile of fetched source values
       */
      public double get95Percentile(String sourceName)
      {
         return getPercentile(sourceName);
      }

      /**
       * Used by ISPs which charge for bandwidth utilization on a "95th percentile" basis.<p>
       *
       * The 95th percentile is the highest source value left when the top 5% of a numerically sorted set
       * of source data is discarded. It is used as a measure of the peak value used when one discounts
       * a fair amount for transitory spikes. This makes it markedly different from the average.<p>
       *
       * Read more about this topic at
       * <a href="http://www.red.net/support/resourcecentre/leasedline/percentile.php">Rednet</a> or
       * <a href="http://www.bytemark.co.uk/support/tech/95thpercentile.html">Bytemark</a>.
       *
       * @param sourceName Datasource name
       * @return 95th percentile of fetched source values
       */
      public double getPercentile(String sourceName)
      {
         return getPercentile(sourceName, DEFAULT_PERCENTILE);
      }

      /**
       * The same as {@link #getPercentile(String)} but with a possibility to define custom percentile boundary
       * (different from 95).
       * @param sourceName Datasource name.
       * @param percentile Boundary percentile. Value of 95 (%) is suitable in most cases, but you are free
       * to provide your own percentile boundary between zero and 100.
       * @return Requested percentile of fetched source values
       */
      public double getPercentile(String sourceName, double percentile)
      {
         if (percentile <= 0.0 || percentile > 100.0)
         {
            throw new ArgumentException("Invalid percentile [" + percentile + "], should be between 0 and 100");
         }
         DataSource source = getSource(sourceName);
         return source.getPercentile(tStart, tEnd, percentile);
      }

      /**
       * Returns array of datasource names defined in this DataProcessor.
       *
       * @return array of datasource names
       */
      public String[] getSourceNames()
      {
         List<string> sourceNames = new List<string>(sources.Count);
         foreach (var source in sources)
         {
            sourceNames.Add(source.getName());
         }
         return sourceNames.ToArray();
      }

      /**
       * Returns an array of all datasource values for all datasources. Each row in this two-dimensional
       * array represents an array of calculated values for a single datasource. The order of rows is the same
       * as the order in which datasources were added to this DataProcessor object.
       *
       * @return All datasource values for all datasources. The first index is the index of the datasource,
       *         the second index is the index of the datasource value. The number of datasource values is equal
       *         to the number of timestamps returned with {@link #getTimestamps()}  method.
       * @throws ArgumentException Thrown if invalid datasource name is specified,
       *                      or if datasource values are not yet calculated (method {@link #processData()}
       *                      was not called)
       */
      public double[][] getValues()
      {
         String[] names = getSourceNames();
         double[][] values = new double[names.Length][];
         for (int i = 0; i < names.Length; i++)
         {
            values[i] = getValues(names[i]);
         }
         return values;
      }

      private DataSource getSource(String sourceName)
      {

         foreach (var source in sources)
         {
            if (source.getName().CompareTo(sourceName) == 0)
               return source;

         }
         throw new ArgumentException("Unknown source: " + sourceName);
      }

      /////////////////////////////////////////////////////////////////
      // DATASOURCE DEFINITIONS
      /////////////////////////////////////////////////////////////////

      /**
       * <p>Adds a custom, {@link org.Rrd4n.data.Plottable plottable} datasource (<b>PDEF</b>).
       * The datapoints should be made available by a class extending
       * {@link org.Rrd4n.data.Plottable Plottable} class.</p>
       *
       * @param name      source name.
       * @param plottable class that extends Plottable class and is suited for graphing.
       */
      public void addDatasource(String name, Plottable plottable)
      {
         sources.Add(new DataPDef(name, plottable));
      }

      /**
       * <p>Adds complex source (<b>CDEF</b>).
       * Complex sources are evaluated using the supplied <code>RPN</code> expression.</p>
       *
       * <p>Complex source <code>name</code> can be used:</p>
       * <ul>
       * <li>To specify sources for line, area and stack plots.</li>
       * <li>To define other complex sources.</li>
       * </ul>
       *
       * <p>Rrd4n supports the following RPN functions, operators and constants: +, -, *, /,
       * %, SIN, COS, LOG, EXP, FLOOR, CEIL, ROUND, POW, ABS, SQRT, RANDOM, LT, LE, GT, GE, EQ,
       * IF, MIN, MAX, LIMIT, DUP, EXC, POP, UN, UNKN, NOW, TIME, PI, E,
       * AND, OR, XOR, PREV, PREV(sourceName), INF, NEGINF, STEP, YEAR, MONTH, DATE,
       * HOUR, MINUTE, SECOND, WEEK, SIGN and RND.</p>
       *
       * <p>Rrd4n does not force you to specify at least one simple source name as RRDTool.</p>
       *
       * <p>For more details on RPN see RRDTool's
       * <a href="http://people.ee.ethz.ch/~oetiker/webtools/rrdtool/manual/rrdgraph.html" target="man">
       * rrdgraph man page</a>.</p>
       *
       * @param name          source name.
       * @param rpnExpression RPN expression containig comma (or space) delimited simple and complex
       *                      source names, RPN constants, functions and operators.
       */
      public void addDatasource(String name, String rpnExpression)
      {
         sources.Add(new DataCDef(name, rpnExpression));
      }

      /**
       * <p>Adds static source (<b>SDEF</b>). Static sources are the result of a consolidation function applied
       * to *any* other source that has been defined previously.</p>
       *
       * @param name      source name.
       * @param defName   Name of the datasource to calculate the value from.
       * @param consolFun Consolidation function to use for value calculation
       */
      public void addDatasource(String name, String defName, AggregateFunction.Type aggregateFunction)
      {
         sources.Add(new DatatSDef(name, defName, aggregateFunction));
      }

      /**
       * <p>Adds simple datasource (<b>DEF</b>). Simple source <code>name</code>
       * can be used:</p>
       * <ul>
       * <li>To specify sources for line, area and stack plots.</li>
       * <li>To define complex sources
       * </ul>
       *
       * @param name       source name.
       * @param file       Path to RRD file.
       * @param dsName     Datasource name defined in the RRD file.
       * @param consolFunc Consolidation function that will be used to extract data from the RRD
       */
      public void addDatasource(String name, String file, String dsName, ConsolFun consolFunc)
      {
         sources.Add(new DataDef(name, file, dsName, consolFunc));
      }

      public void addDatasource(String name, String file, String dsName, ConsolFun consolFunc, long step, long startTime, long endTime, string reduceName)
      {
         long defStartTime = startTime;
         if (defStartTime == long.MinValue)
            defStartTime = StartTime;
         long defEndTime = endTime;
         if (defEndTime == long.MinValue)
            defEndTime = EndTime;
         sources.Add(new DataDef(name, file, dsName, consolFunc, step, defStartTime, defEndTime, reduceName));
      }
      /**
       * <p>Adds simple source (<b>DEF</b>). Source <code>name</code> can be used:</p>
       * <ul>
       * <li>To specify sources for line, area and stack plots.</li>
       * <li>To define complex sources
       * </ul>
       *
       * @param name       Source name.
       * @param file       Path to RRD file.
       * @param dsName     Data source name defined in the RRD file.
       * @param consolFunc Consolidation function that will be used to extract data from the RRD
       *                   file ("AVERAGE", "MIN", "MAX" or "LAST" - these string constants are conveniently defined
       *                   in the {@link org.Rrd4n.ConsolFun ConsolFun} class).
       * @param backend    Name of the RrdBackendFactory that should be used for this RrdDb.
       */
      public void addDatasource(String name, String file, String dsName, ConsolFun consolFunc, String backend)
      {
         sources.Add(new DataDef(name, file, dsName, consolFunc, backend));
      }

      /**
       * Adds DEF datasource with datasource values already available in the FetchData object. This method is
       * used internally by Rrd4n and probably has no purpose outside of it.
       *
       * @param name Source name.
       * @param fetchData Fetched data containing values for the given source name.
       */
      public void addDatasource(String name, FetchData fetchData)
      {
         sources.Add(new DataDef(name, fetchData));
      }

      public void addDatasource(string name, long shiftOffset)
      {
         shifts.Add(new DataShift(name,shiftOffset));
      }
      /////////////////////////////////////////////////////////////////
      // CALCULATIONS
      /////////////////////////////////////////////////////////////////

      /**
       * Method that should be called once all datasources are defined. Data will be fetched from
       * RRD files, RPN expressions will be calculated, etc.
       *
       * @ Thrown in case of I/O error (while fetching data from RRD files)
       */
      public void processData()
      {
         extractDefs();
         fetchRrdData();
         fixZeroEndingTimestamp();
         chooseOptimalStep();
         createTimestamps();
         assignTimestampsToSources();
         TimeShiftData();
         normalizeRrdValues();
         calculateNonRrdSources();
      }

      /**
       * Method used to calculate datasource values which should be presented on the graph
       * based on the desired graph width. Each value returned represents a single pixel on the graph.
       * Corresponding timestamp can be found in the array returned from {@link #getTimestampsPerPixel()}
       * method.
       *
       * @param sourceName Datasource name
       * @param pixelCount Graph width
       * @return Per-pixel datasource values
       * @throws ArgumentException Thrown if datasource values are not yet calculated (method {@link #processData()}
       * was not called)
       */
      public double[] getValuesPerPixel(String sourceName, int pixelCount)
      {
         setPixelCount(pixelCount);
         return getValuesPerPixel(sourceName);
      }

      /**
       * Method used to calculate datasource values which should be presented on the graph
       * based on the graph width set with a {@link #setPixelCount(int)} method call.
       * Each value returned represents a single pixel on the graph. Corresponding timestamp can be
       * found in the array returned from {@link #getTimestampsPerPixel()} method.
       *
       * @param sourceName Datasource name
       * @return Per-pixel datasource values
       * @throws ArgumentException Thrown if datasource values are not yet calculated (method {@link #processData()}
       * was not called)
       */
      public double[] getValuesPerPixel(String sourceName)
      {
         double[] values = getValues(sourceName);
         double[] pixelValues = Util.FillArray(double.NaN, PixelCount);

         long span = tEnd - tStart;
         // this is the ugliest nested loop I have ever made
         for (int pix = 0, refer = 0; pix < PixelCount; pix++)
         {
            double t = tStart + span * pix / (double)(PixelCount - 1);
            while (refer < timestamps.Length)
            {
               if (t <= timestamps[refer] - Step)
                  // too left, nothing to do, already NaN
                  break;

               if (t <= timestamps[refer])
               {
                  // in brackets, get this value
                  pixelValues[pix] = values[refer];
                  break;
               }
               // too right
               refer++;
            }
         }
         return pixelValues;
      }

      /**
       * Calculates timestamps which correspond to individual pixels on the graph.
       *
       * @param pixelCount Graph width
       * @return Array of timestamps
       */
      public long[] getTimestampsPerPixel(int pixelCount)
      {
         setPixelCount(pixelCount);
         return getTimestampsPerPixel();
      }

      /**
       * Calculates timestamps which correspond to individual pixels on the graph
       * based on the graph width set with a {@link #setPixelCount(int)} method call.
       *
       * @return Array of timestamps
       */
      public long[] getTimestampsPerPixel()
      {
         long[] times = new long[PixelCount];
         long span = tEnd - tStart;
         for (int i = 0; i < PixelCount; i++)
         {
            times[i] = (long)Math.Round(tStart + span * i / (double)(PixelCount - 1));
         }
         return times;
      }

      /**
       * Dumps timestamps and values of all datasources in a tabelar form. Very useful for debugging.
       *
       * @return Dumped object content.
       */
      public String dump()
      {
         String[] names = getSourceNames();
         double[][] values = getValues();
         StringBuilder buffer = new StringBuilder();
         buffer.Append(format("timestamp", 12));
         foreach (String name in names)
         {
            buffer.Append(format(name, 20));
         }
         buffer.Append("\n");
         for (int i = 0; i < timestamps.Length; i++)
         {
            buffer.Append(format("" + timestamps[i], 12));
            for (int j = 0; j < names.Length; j++)
            {
               buffer.Append(format(Util.formatDouble(values[j][i]), 20));
            }
            buffer.Append("\n");
         }
         return buffer.ToString();
      }

      // PRIVATE METHODS

      private void extractDefs()
      {
         List<DataDef> defList = new List<DataDef>();
         foreach (DataSource source in sources)
         {
            if (source.GetType() == typeof(DataDef))
            {
               defList.Add((DataDef)source);
            }
         }
         defSources = defList.ToArray();
      }

      private void fetchRrdData()
      {
         long tEndFixed = (tEnd == 0) ? Util.getTimestamp() : tEnd;
         for (int i = 0; i < defSources.Length; i++)
         {
            if (!defSources[i].isLoaded())
            {
               // not fetched yet
               List<String> dsNames = new List<String>();
               dsNames.Add(defSources[i].getDsName());
               // look for all other datasources with the same path and the same consolidation function
               for (int j = i + 1; j < defSources.Length; j++)
               {
                  if (defSources[i].isCompatibleWith(defSources[j]))
                  {
                     dsNames.Add(defSources[j].getDsName());
                  }
               }
               // now we have everything
               FetchRequest req = null;
               if (defSources[i].StartTime == long.MinValue)
                  req = new FetchRequest(defSources[i].getPath(), defSources[i].getConsolFun().Name, tStart, tEndFixed, FetchRequestResolution);
               else
               {
                  long step = Math.Max(1, defSources[i].Step);
                  string reduceFunctionName = defSources[i].ReduceCfName;

                  if (string.IsNullOrEmpty(reduceFunctionName))
                     reduceFunctionName = defSources[i].getConsolFun().Name;

                  req = new FetchRequest(defSources[i].getPath(), reduceFunctionName, defSources[i].StartTime, defSources[i].EndTime, step);
               }
               req.setFilter(dsNames);
               FetchData data = DbAccessInterface.GetData(req);

               defSources[i].setFetchData(data);
               for (int j = i + 1; j < defSources.Length; j++)
               {
                  if (defSources[i].isCompatibleWith(defSources[j]))
                     defSources[j].setFetchData(data);
               }
            }
         }
      }

      private void TimeShiftData()
      {
         foreach (DataShift dataShift in shifts)
         {
            DataDef dataSource = getSource(dataShift.getName()) as DataDef;
            dataShift.TimeShiftData(dataSource.getRrdTimestamps());
         }
      }

      private void fixZeroEndingTimestamp()
      {
         if (tEnd == 0)
         {
            if (defSources.Length == 0)
            {
               throw new ApplicationException("Could not adjust zero ending timestamp, no DEF source provided");
            }
            tEnd = defSources[0].getArchiveEndTime();
            for (int i = 1; i < defSources.Length; i++)
            {
               tEnd = Math.Min(tEnd, defSources[i].getArchiveEndTime());
            }
            if (tEnd <= tStart)
            {
               throw new ApplicationException("Could not resolve zero ending timestamp.");
            }
         }
      }

      // Tricky and ugly. Should be redesigned some time in the future
      private void chooseOptimalStep()
      {
         long newStep = long.MaxValue;
         foreach (DataDef defSource in defSources)
         {
            long fetchStep = defSource.getFetchStep(), tryStep = fetchStep;
            if (Step > 0)
            {
               tryStep = Math.Min(newStep, (((Step - 1) / fetchStep) + 1) * fetchStep);
            }
            newStep = Math.Min(newStep, tryStep);
         }
         if (newStep != long.MaxValue)
         {
            // step resolved from a RRD file
            Step = newStep;
         }
         else
         {
            // choose step based on the number of pixels (useful for plottable datasources)
            Step = Math.Max((tEnd - tStart) / PixelCount, 1);
         }
      }

      private void createTimestamps()
      {
         long t1 = Util.normalize(tStart, Step);
         long t2 = Util.normalize(tEnd, Step);
         if (t2 < tEnd)
         {
            t2 += Step;
         }
         int count = (int)(((t2 - t1) / Step) + 1);
         timestamps = new long[count];
         for (int i = 0; i < count; i++)
         {
            timestamps[i] = t1;
            t1 += Step;
         }
      }

      private void assignTimestampsToSources()
      {
         foreach (DataSource src in sources)
         {
            src.setTimestamps(timestamps);
         }
      }

      private void normalizeRrdValues()
      {
         Normalizer normalizer = new Normalizer(timestamps);
         foreach (DataDef def in defSources)
         {
            long[] rrdTimestamps = def.getRrdTimestamps();
            double[] rrdValues = def.getRrdValues();
            def.setValues(normalizer.normalize(rrdTimestamps, rrdValues));
         }
      }

      private void calculateNonRrdSources()
      {
         foreach (DataSource source in sources)
         {
            if (source.GetType() == typeof(DatatSDef))
               calculateSDef((DatatSDef)source);

            else if (source.GetType() == typeof(DataCDef))
               calculateCDef((DataCDef)source);
         }
      }

      private void calculateCDef(DataCDef cDef)
      {
         RpnCalculator calc = new RpnCalculator(cDef.getRpnExpression(), cDef.getName(), this);
         cDef.setValues(calc.calculateValues());
      }

      private void calculateSDef(DatatSDef sDef)
      {
         sDef.setValue(getSource(sDef.DefName).getAggregates(tStart, tEnd));
      }


      private static String format(String s, int length)
      {
         StringBuilder b = new StringBuilder(s);
         for (int i = 0; i < length - s.Length; i++)
         {
            b.Append(' ');
         }
         return b.ToString();
      }
   }
}
