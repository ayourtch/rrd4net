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
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using rrd4n.Common;
using rrd4n.DataAccess.Data;

namespace rrd4n.Core
{
   /**
    * <p>Main class used to create and manipulate round robin databases (RRDs). Use this class to perform
    * update and fetch operations on exisiting RRDs, to create new RRD from
    * the definition (object of class {@link org.Rrd4n.core.RrdDef RrdDef}) or
    * from XML file (dumped content of RRDTool's or Rrd4n's RRD file).</p>
    * <p/>
    * <p>Each RRD is backed with some kind of storage. For example, RRDTool supports only one kind of
    * storage (disk file). On the contrary, Rrd4n gives you freedom to use other storage (backend) types
    * even to create your own backend types for some special purposes. Rrd4n by default stores
    * RRD data in files (as RRDTool), but you might choose to store RRD data in memory (this is
    * supported in Rrd4n), to use java.nio.* instead of java.io.* package for file manipulation
    * (also supported) or to store whole RRDs in the SQL database
    * (you'll have to extend some classes to do this).</p>
    * <p/>
    * <p>Note that Rrd4n uses binary format different from RRDTool's format. You cannot
    * use this class to manipulate RRD files created with RRDTool. <b>However, if you perform
    * the same sequence of create, update and fetch operations, you will get exactly the same
    * results from Rrd4n and RRDTool.</b><p>
    * <p/>
    * <p/>
    * You will not be able to use Rrd4n API if you are not familiar with
    * basic RRDTool concepts. Good place to start is the
    * <a href="http://people.ee.ethz.ch/~oetiker/webtools/rrdtool/tutorial/rrdtutorial.html">official RRD tutorial</a>
    * and relevant RRDTool man pages: <a href="../../../../man/rrdcreate.html" target="man">rrdcreate</a>,
    * <a href="../../../../man/rrdupdate.html" target="man">rrdupdate</a>,
    * <a href="../../../../man/rrdfetch.html" target="man">rrdfetch</a> and
    * <a href="../../../../man/rrdgraph.html" target="man">rrdgraph</a>.
    * For RRDTool's advanced graphing capabilities (RPN extensions), also supported in Rrd4n,
    * there is an excellent
    * <a href="http://people.ee.ethz.ch/~oetiker/webtools/rrdtool/tutorial/cdeftutorial.html" target="man">CDEF tutorial</a>.
    * </p>
    *
    * @see RrdBackend
    * @see RrdBackendFactory
    */
   public class RrdDb : RrdUpdater
   {
      private static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

      /**
       * prefix to identify external XML file source used in various RrdDb constructors
       */
      public static readonly String PREFIX_XML = "xml:/";
      /**
       * prefix to identify external RRDTool file source used in various RrdDb constructors
       */
      public static readonly String PREFIX_RRDTool = "rrdtool:/";

      // static readonly String RRDTOOL = "rrdtool";
      static readonly int XML_INITIAL_BUFFER_CAPACITY = 100000; // bytes

      private RrdBackend backend;
      private RrdAllocator allocator = new RrdAllocator();

      private readonly Header header;
      private readonly Datasource[] Datasources;
      private readonly Archive[] archives;

      private bool closed = false;
      private object sync = new object();

      /**
       * <p>Constructor used to create new RRD object from the definition. This RRD object will be backed
       * with a storage (backend) of the default type. Initially, storage type defaults to "NIO"
       * (RRD bytes will be put in a file on the disk). Default storage type can be changed with a static
       * {@link RrdBackendFactory#setDefaultFactory(String)} method call.</p>
       * <p/>
       * <p>New RRD file structure is specified with an object of class
       * {@link RrdDef <b>RrdDef</b>}. The underlying RRD storage is created as soon
       * as the constructor returns.</p>
       * <p/>
       * <p>Typical scenario:</p>
       * <p/>
       * <pre>
       * // create new RRD definition
       * RrdDef def = new RrdDef("test.rrd", 300);
       * def.addDatasource("input", DsType.DT_COUNTER, 600, 0, Double.NaN);
       * def.addDatasource("output", DsType.DT_COUNTER, 600, 0, Double.NaN);
       * def.addArchive(ConsolFun.CF_AVERAGE, 0.5, 1, 600);
       * def.addArchive(ConsolFun.CF_AVERAGE, 0.5, 6, 700);
       * def.addArchive(ConsolFun.CF_AVERAGE, 0.5, 24, 797);
       * def.addArchive(ConsolFun.CF_AVERAGE, 0.5, 288, 775);
       * def.addArchive(ConsolFun.CF_MAX, 0.5, 1, 600);
       * def.addArchive(ConsolFun.CF_MAX, 0.5, 6, 700);
       * def.addArchive(ConsolFun.CF_MAX, 0.5, 24, 797);
       * def.addArchive(ConsolFun.CF_MAX, 0.5, 288, 775);
       * <p/>
       * // RRD definition is now completed, create the database!
       * RrdDb rrd = new RrdDb(def);
       * // new RRD file has been created on your disk
       * </pre>
       *
       * @param rrdDef Object describing the structure of the new RRD file.
       * @Thrown in case of I/O error.
       */
      public RrdDb(RrdDef rrdDef)
         : this(rrdDef, RrdFileBackendFactory.getDefaultFactory())
      { }

      /**
       * Constructor used to create new RRD object from the definition object but with a storage
       * (backend) different from default.
       *
       * <p>Rrd4n uses <i>factories</i> to create RRD backend objecs. There are three different
       * backend factories supplied with Rrd4n, and each factory has its unique name:</p>
       * <p/>
       * <ul>
       * <li><b>FILE</b>: backends created from this factory will store RRD data to files by using
       * java.io.* classes and methods
       * <li><b>NIO</b>: backends created from this factory will store RRD data to files by using
       * java.nio.* classes and methods
       * <li><b>MEMORY</b>: backends created from this factory will store RRD data in memory. This might
       * be useful in runtime environments which prohibit disk utilization, or for storing temporary,
       * non-critical data (it gets lost as soon as JVM exits).
       * </ul>
       * <p/>
       * <p>For example, to create RRD in memory, use the following code</p>
       * <pre>
       * RrdBackendFactory factory = RrdBackendFactory.getFactory("MEMORY");
       * RrdDb rrdDb = new RrdDb(rrdDef, factory);
       * rrdDb.close();
       * </pre>
       * <p/>
       * <p>New RRD file structure is specified with an object of class
       * {@link RrdDef <b>RrdDef</b>}. The underlying RRD storage is created as soon
       * as the constructor returns.</p>
       *
       * @param rrdDef  RRD definition object
       * @param factory The factory which will be used to create storage for this RRD
       * @Thrown in case of I/O error
       * @see RrdBackendFactory
       */
      public RrdDb(RrdDef rrdDef, RrdBackendFactory factory)
      {
         if (!rrdDef.hasDatasources())
         {
            throw new ArgumentException("No RRD Datasource specified. At least one is needed.");
         }
         if (!rrdDef.hasArchives())
         {
            throw new ArgumentException("No RRD archive specified. At least one is needed.");
         }

         String path = rrdDef.getPath();
         backend = factory.open(path, false);
         try
         {
            backend.setLength(rrdDef.getEstimatedSize());
            // create header
            header = new Header(this, rrdDef);
            // create Datasources
            DsDef[] dsDefs = rrdDef.getDsDefs();
            Datasources = new Datasource[dsDefs.Length];
            for (int i = 0; i < dsDefs.Length; i++)
            {
               Datasources[i] = new Datasource(this, dsDefs[i]);
            }
            // create archives
            ArcDef[] arcDefs = rrdDef.getArcDefs();
            archives = new Archive[arcDefs.Length];
            for (int i = 0; i < arcDefs.Length; i++)
            {
               archives[i] = new Archive(this, arcDefs[i]);
            }
         }
         catch (IOException e)
         {
            backend.close();
            throw;
         }
      }

      /**
       * Constructor used to open already existing RRD. This RRD object will be backed
       * with a storage (backend) of the default type (file on the disk). Constructor
       * obtains read or read/write access to this RRD.
       *
       * @param path     Path to existing RRD.
       * @param readOnly Should be set to <code>false</code> if you want to update
       *                 the underlying RRD. If you want just to fetch data from the RRD file
       *                 (read-only access), specify <code>true</code>. If you try to update RRD file
       *                 open in read-only mode (<code>readOnly</code> set to <code>true</code>),
       *                 <code>IOException</code> will be thrown.
       * @Thrown in case of I/O error.
       */
      public RrdDb(String path, bool readOnly)
         : this(path, readOnly, RrdBackendFactory.getDefaultFactory())
      {
      }

      /**
       * Constructor used to open already existing RRD backed
       * with a storage (backend) different from default. Constructor
       * obtains read or read/write access to this RRD.
       *
       * @param path     Path to existing RRD.
       * @param readOnly Should be set to <code>false</code> if you want to update
       *                 the underlying RRD. If you want just to fetch data from the RRD file
       *                 (read-only access), specify <code>true</code>. If you try to update RRD file
       *                 open in read-only mode (<code>readOnly</code> set to <code>true</code>),
       *                 <code>IOException</code> will be thrown.
       * @param factory  Backend factory which will be used for this RRD.
       * @throws FileNotFoundException Thrown if the requested file does not exist.
       * @          Thrown in case of general I/O error (bad RRD file, for example).
       * @see RrdBackendFactory
       */
      public RrdDb(String path, bool readOnly, RrdBackendFactory factory)
      {
         if (!factory.exists(path)) throw new System.IO.FileNotFoundException("Could not open " + path + " [non existent]");

         backend = factory.open(path, readOnly);
         try
         {
            // restore header
            header = new Header(this, (RrdDef)null);

            if (factory.shouldValidateHeader(path))
            {
               header.validateHeader();
            }

            // restore Datasources
            int dsCount = header.getDsCount();
            Datasources = new Datasource[dsCount];
            for (int i = 0; i < dsCount; i++)
            {
               Datasources[i] = new Datasource(this, null);
            }
            // restore archives
            int arcCount = header.getArcCount();
            archives = new Archive[arcCount];
            for (int i = 0; i < arcCount; i++)
            {
               archives[i] = new Archive(this, null);
            }
         }
         catch (IOException e)
         {
            backend.close();
            log.ErrorFormat("RrdDb ctor failed on:{0} [{1]}", path, e.Message);
            throw e;
         }
      }

      /**
       * <p>Constructor used to open already existing RRD in R/W mode, with a default storage
       * (backend) type (file on the disk).
       *
       * @param path Path to existing RRD.
       * @Thrown in case of I/O error.
       */
      public RrdDb(String path)
         : this(path, false)
      {
      }

      /**
       * <p>Constructor used to open already existing RRD in R/W mode with a storage (backend) type
       * different from default.</p>
       *
       * @param path    Path to existing RRD.
       * @param factory Backend factory used to create this RRD.
       * @Thrown in case of I/O error.
       * @see RrdBackendFactory
       */
      public RrdDb(String path, RrdBackendFactory factory)
         : this(path, false, factory)
      {
      }

      /**
       * <p>Constructor used to create RRD files from external file sources.
       * Supported external file sources are:</p>
       * <p/>
       * <ul>
       * <li>RRDTool/Rrd4n XML file dumps (i.e files created with <code>rrdtool dump</code> command).
       * <li>RRDTool binary files.
       * </ul>
       * <p/>
       * <p>Newly created RRD will be backed with a default storage (backend) type
       * (file on the disk).</p>
       * <p/>
       * <p>Rrd4n and RRDTool use the same format for XML dump and this constructor should be used to
       * (re)create Rrd4n RRD files from XML dumps. First, dump the content of a RRDTool
       * RRD file (use command line):</p>
       * <p/>
       * <pre>
       * rrdtool dump original.rrd > original.xml
       * </pre>
       * <p/>
       * <p>Than, use the file <code>original.xml</code> to create Rrd4n RRD file named
       * <code>copy.rrd</code>:</p>
       * <p/>
       * <pre>
       * RrdDb rrd = new RrdDb("copy.rrd", "original.xml");
       * </pre>
       * <p/>
       * <p>or:</p>
       * <p/>
       * <pre>
       * RrdDb rrd = new RrdDb("copy.rrd", "xml:/original.xml");
       * </pre>
       * <p/>
       * <p>See documentation for {@link #dumpXml(String) dumpXml()} method
       * to see how to convert Rrd4n files to RRDTool's format.</p>
       * <p/>
       * <p>To read RRDTool files directly, specify <code>rrdtool:/</code> prefix in the
       * <code>externalPath</code> argument. For example, to create Rrd4n compatible file named
       * <code>copy.rrd</code> from the file <code>original.rrd</code> created with RRDTool, use
       * the following code:</p>
       * <p/>
       * <pre>
       * RrdDb rrd = new RrdDb("copy.rrd", "rrdtool:/original.rrd");
       * </pre>
       * <p/>
       * <p>Note that the prefix <code>xml:/</code> or <code>rrdtool:/</code> is necessary to distinguish
       * between XML and RRDTool's binary sources. If no prefix is supplied, XML format is assumed</p>
       *
       * @param rrdPath      Path to a RRD file which will be created
       * @param externalPath Path to an external file which should be imported, with an optional
       *                     <code>xml:/</code> or <code>rrdtool:/</code> prefix.
       * @Thrown in case of I/O error
       */
      public RrdDb(String rrdPath, String externalPath)
         : this(rrdPath, externalPath, RrdBackendFactory.getDefaultFactory())
      {
      }

      /**
       * <p>Constructor used to create RRD files from external file sources with a backend type
       * different from default. Supported external file sources are:</p>
       * <p/>
       * <ul>
       * <li>RRDTool/Rrd4n XML file dumps (i.e files created with <code>rrdtool dump</code> command).
       * <li>RRDTool binary files.
       * </ul>
       * <p/>
       * <p>Rrd4n and RRDTool use the same format for XML dump and this constructor should be used to
       * (re)create Rrd4n RRD files from XML dumps. First, dump the content of a RRDTool
       * RRD file (use command line):</p>
       * <p/>
       * <pre>
       * rrdtool dump original.rrd > original.xml
       * </pre>
       * <p/>
       * <p>Than, use the file <code>original.xml</code> to create Rrd4n RRD file named
       * <code>copy.rrd</code>:</p>
       * <p/>
       * <pre>
       * RrdDb rrd = new RrdDb("copy.rrd", "original.xml");
       * </pre>
       * <p/>
       * <p>or:</p>
       * <p/>
       * <pre>
       * RrdDb rrd = new RrdDb("copy.rrd", "xml:/original.xml");
       * </pre>
       * <p/>
       * <p>See documentation for {@link #dumpXml(String) dumpXml()} method
       * to see how to convert Rrd4n files to RRDTool's format.</p>
       * <p/>
       * <p>To read RRDTool files directly, specify <code>rrdtool:/</code> prefix in the
       * <code>externalPath</code> argument. For example, to create Rrd4n compatible file named
       * <code>copy.rrd</code> from the file <code>original.rrd</code> created with RRDTool, use
       * the following code:</p>
       * <p/>
       * <pre>
       * RrdDb rrd = new RrdDb("copy.rrd", "rrdtool:/original.rrd");
       * </pre>
       * <p/>
       * <p>Note that the prefix <code>xml:/</code> or <code>rrdtool:/</code> is necessary to distinguish
       * between XML and RRDTool's binary sources. If no prefix is supplied, XML format is assumed</p>
       *
       * @param rrdPath      Path to RRD which will be created
       * @param externalPath Path to an external file which should be imported, with an optional
       *                     <code>xml:/</code> or <code>rrdtool:/</code> prefix.
       * @param factory      Backend factory which will be used to create storage (backend) for this RRD.
       * @Thrown in case of I/O error
       * @see RrdBackendFactory
       */
      public RrdDb(String rrdPath, String externalPath, RrdBackendFactory factory) {
          DataImporter reader;
      //    if (externalPath.startsWith(PREFIX_RRDTool)) {
      //        String rrdToolPath = externalPath.substring(PREFIX_RRDTool.Length);
      //        reader = new RrdToolReader(rrdToolPath);
      //    }
      //    else if (externalPath.startsWith(PREFIX_XML)) {
      //        externalPath = externalPath.substring(PREFIX_XML.Length);
      //        reader = new XmlReader(externalPath);
      //    }
      //    else {
              reader = new XmlImporter(externalPath);
      //    }
          backend = factory.open(rrdPath, false);
          try {
              backend.setLength(reader.getEstimatedSize());
              // create header
              header = new Header(this, reader);
      //        // create Datasources
      //        Datasources = new Datasource[reader.getDsCount()];
      //        for (int i = 0; i < Datasources.Length; i++) {
      //            Datasources[i] = new Datasource(this, reader, i);
      //        }
      //        // create archives
      //        archives = new Archive[reader.getArcCount()];
      //        for (int i = 0; i < archives.Length; i++) {
      //            archives[i] = new Archive(this, reader, i);
      //        }
              reader.release();
              // XMLReader is a rather huge DOM tree, release memory ASAP
              reader = null;
          }
          catch (IOException e)
          {
             backend.close();
             throw e;
          }
      }

      /**
       * Closes RRD. No further operations are allowed on this RrdDb object.
       *
       * @Thrown in case of I/O related error.
       */
      public void close()
      {
         lock (sync)
         {
            if (!closed)
            {
               closed = true;
               backend.close();
            }
         }
      }

      /**
       * Returns true if the RRD is closed.
       *
       * @return true if closed, false otherwise
       */
      public bool isClosed()
      {
         return closed;
      }

      /**
       * Returns RRD header.
       *
       * @return Header object
       */
      public Header getHeader()
      {
         return header;
      }

      /**
       * Returns Datasource object for the given Datasource index.
       *
       * @param dsIndex Datasource index (zero based)
       * @return Datasource object
       */
      public Datasource getDatasource(int dsIndex)
      {
         return Datasources[dsIndex];
      }

      /**
       * Returns Archive object for the given archive index.
       *
       * @param arcIndex Archive index (zero based)
       * @return Archive object
       */
      public Archive getArchive(int arcIndex)
      {
         return archives[arcIndex];
      }

      /**
       * <p>Returns an array of Datasource names defined in RRD.</p>
       *
       * @return Array of Datasource names.
       * @Thrown in case of I/O error.
       */
      public String[] getDsNames()
      {
         int n = Datasources.Length;
         String[] dsNames = new String[n];
         for (int i = 0; i < n; i++)
         {
            dsNames[i] = Datasources[i].DsName;
         }
         return dsNames;
      }

      /**
       * <p>Creates new sample with the given timestamp and all Datasource values set to
       * 'unknown'. Use returned <code>Sample</code> object to specify
       * Datasource values for the given timestamp. See documentation for
       * {@link Sample Sample} for an explanation how to do this.</p>
       * <p/>
       * <p>Once populated with data source values, call Sample's
       * {@link Sample#update() update()} method to actually
       * store sample in the RRD associated with it.</p>
       *
       * @param time Sample timestamp rounded to the nearest second (without milliseconds).
       * @return Fresh sample with the given timestamp and all data source values set to 'unknown'.
       * @Thrown in case of I/O error.
       */
      public Sample createSample(long time)
      {
         return new Sample(this.getPath(), getDsNames(), time);
      }

      /**
       * <p>Creates new sample with the current timestamp and all data source values set to
       * 'unknown'. Use returned <code>Sample</code> object to specify
       * Datasource values for the current timestamp. See documentation for
       * {@link Sample Sample} for an explanation how to do this.</p>
       * <p/>
       * <p>Once populated with data source values, call Sample's
       * {@link Sample#update() update()} method to actually
       * store sample in the RRD associated with it.</p>
       *
       * @return Fresh sample with the current timestamp and all
       *         data source values set to 'unknown'.
       * @Thrown in case of I/O error.
       */
      public Sample createSample(DateTime timeStamp)
      {
         return createSample(Util.getTimestamp(timeStamp));
      }

      /**
       * <p>Prepares fetch request to be executed on this RRD. Use returned
       * <code>FetchRequest</code> object and its {@link FetchRequest#fetchData() fetchData()}
       * method to actually fetch data from the RRD file.</p>
       *
       * @param consolFun  Consolidation function to be used in fetch request. Allowed values are
       *                   "AVERAGE", "MIN", "MAX" and "LAST" (these constants are conveniently defined in the
       *                   {@link ConsolFun} class).
       * @param fetchStart Starting timestamp for fetch request.
       * @param fetchEnd   Ending timestamp for fetch request.
       * @param resolution Fetch resolution (see RRDTool's
       *                   <a href="../../../../man/rrdfetch.html" target="man">rrdfetch man page</a> for an
       *                   explanation of this parameter.
       * @return Request object that should be used to actually fetch data from RRD
       */
      public FetchRequest createFetchRequest(ConsolFun consolFun, long fetchStart, long fetchEnd, long resolution)
      {
         return new FetchRequest(this.getPath(), consolFun.Name, Util.getDate(fetchStart), Util.getDate(fetchEnd), resolution);
      }

      /**
       * <p>Prepares fetch request to be executed on this RRD. Use returned
       * <code>FetchRequest</code> object and its {@link FetchRequest#fetchData() fetchData()}
       * method to actually fetch data from this RRD. Data will be fetched with the smallest
       * possible resolution (see RRDTool's
       * <a href="../../../../man/rrdfetch.html" target="man">rrdfetch man page</a>
       * for the explanation of the resolution parameter).</p>
       *
       * @param consolFun  Consolidation function to be used in fetch request. Allowed values are
       *                   AVERAGE, MIN, MAX and LAST (see {@link ConsolFun} enum).
       * @param fetchStart Starting timestamp for fetch request.
       * @param fetchEnd   Ending timestamp for fetch request.
       * @return Request object that should be used to actually fetch data from RRD.
       */
      public FetchRequest createFetchRequest(ConsolFun consolFun, long fetchStart, long fetchEnd)
      {
         return createFetchRequest(consolFun, fetchStart, fetchEnd, 1);
      }

      public void store(Sample sample)
      {
         store(sample.getTime(), sample.getValues());
      }

      public void store(long timeStamp, double[] newValues)
      {
         lock (sync)
         {
            if (closed) throw new ApplicationException("RRD already closed, cannot store this sample");

            long lastTime = header.getLastUpdateTime();
            if (lastTime >= timeStamp)
               throw new ArgumentException("Bad sample time: " + rrd4n.Common.Util.getDate(timeStamp) +
                       ". Last update time was " + rrd4n.Common.Util.getDate(lastTime) + ", at least one second step is required");
            if (newValues.Length != Datasources.Length)
               throw new ApplicationException("Datasource mitchmatch. RrdDb datasource count:" + Datasources.Length + ". Datapoint count:" + newValues.Length);

            for (int i = 0; i < Datasources.Length; i++)
            {
               Datasources[i].process(timeStamp, newValues[i]);
            }
            header.setLastUpdateTime(timeStamp);
         }
      }


      public FetchData fetchData(FetchRequest request)
      {
         lock (sync)
         {
            if (closed)
            {
               throw new ApplicationException("RRD already closed, cannot fetch data");
            }
            Archive archive = findMatchingArchive(request);
            return archive.fetchData(request);
         }
      }

      private Archive findMatchingArchive(FetchRequest request)
      {
         long fetchStart = request.FetchStart;
         long fetchEnd = request.FetchEnd;
         long resolution = request.Resolution;
         Archive bestFullMatch = null, bestPartialMatch = null;
         long bestStepDiff = 0, bestMatch = 0;
         foreach (Archive archive in archives)
         {
            if (archive.getConsolFun().Name == request.ConsolidateFunctionName)
            {
               long arcStep = archive.getArcStep();
               long arcStart = archive.getStartTime() - arcStep;
               long arcEnd = archive.getEndTime();
               long fullMatch = fetchEnd - fetchStart;
               if (arcEnd >= fetchEnd && arcStart <= fetchStart)
               {
                  long tmpStepDiff = Math.Abs(archive.getArcStep() - resolution);

                  if (tmpStepDiff < bestStepDiff || bestFullMatch == null)
                  {
                     bestStepDiff = tmpStepDiff;
                     bestFullMatch = archive;
                  }
               }
               else
               {
                  long tmpMatch = fullMatch;

                  if (arcStart > fetchStart)
                  {
                     tmpMatch -= (arcStart - fetchStart);
                  }
                  if (arcEnd < fetchEnd)
                  {
                     tmpMatch -= (fetchEnd - arcEnd);
                  }
                  if (bestPartialMatch == null || bestMatch < tmpMatch)
                  {
                     bestPartialMatch = archive;
                     bestMatch = tmpMatch;
                  }
               }
            }
         }
         if (bestFullMatch != null)
         {
            return bestFullMatch;
         }
         else if (bestPartialMatch != null)
         {
            return bestPartialMatch;
         }
         else
         {
            throw new ApplicationException("RRD file does not contain RRA: " + request.ConsolidateFunctionName + " archive");
         }
      }

      /**
       * Finds the archive that best matches to the start time (time period being start-time until now)
       * and requested resolution.
       *
       * @param consolFun  Consolidation function of the Datasource.
       * @param startTime  Start time of the time period in seconds.
       * @param resolution Requested fetch resolution.
       * @return Reference to the best matching archive.
       * @Thrown in case of I/O related error.
       */
      // Not used???
      [Obsolete]
      public Archive findStartMatchArchive(String consolFun, long startTime, long resolution)
      {
         long arcStep, diff;
         int fallBackIndex = 0;
         int arcIndex = -1;
         long minDiff = long.MaxValue;
         long fallBackDiff = long.MaxValue;

         for (int i = 0; i < archives.Length; i++)
         {
            if (archives[i].getConsolFun().Name.CompareTo(consolFun) == 0)
            {
               arcStep = archives[i].getArcStep();
               diff = Math.Abs(resolution - arcStep);

               // Now compare start time, see if this archive encompasses the requested interval
               if (startTime >= archives[i].getStartTime())
               {
                  if (diff == 0)                // Best possible match either way
                  {
                     return archives[i];
                  }
                  else if (diff < minDiff)
                  {
                     minDiff = diff;
                     arcIndex = i;
                  }
               }
               else if (diff < fallBackDiff)
               {
                  fallBackDiff = diff;
                  fallBackIndex = i;
               }
            }
         }

         return (arcIndex >= 0 ? archives[arcIndex] : archives[fallBackIndex]);
      }

      /**
       * <p>Returns string representing complete internal RRD state. The returned
       * string can be printed to <code>stdout</code> and/or used for debugging purposes.</p>
       *
       * @return String representing internal RRD state.
       * @Thrown in case of I/O related error.
       */
      public String dump()
      {
         lock (sync)
         {
            StringBuilder buffer = new StringBuilder();
            buffer.Append(header.dump());
            foreach (Datasource Datasource in Datasources)
            {
               buffer.Append(Datasource.dump());
            }
            foreach (Archive archive in archives)
            {
               buffer.Append(archive.dump());
            }
            return buffer.ToString();
         }
      }

      public void archive(Datasource Datasource, double value, long numUpdates)
      {
         int dsIndex = getDsIndex(Datasource.DsName);
         foreach (Archive archive in archives)
         {
            archive.archive(dsIndex, value, numUpdates);
         }
      }

      /**
       * Returns internal index number for the given Datasource name.
       *
       * @param dsName Data source name.
       * @return Internal index of the given data source name in this RRD.
       * @Thrown in case of I/O error.
       */
      public int getDsIndex(String dsName)
      {
         for (int i = 0; i < Datasources.Length; i++)
         {
            if (Datasources[i].DsName.CompareTo(dsName) == 0)
            {
               return i;
            }
         }
         throw new ArgumentException("Unknown Datasource name: " + dsName);
      }

      /**
       * Checks presence of a specific Datasource.
       *
       * @param dsName Datasource name to check
       * @return <code>true</code> if Datasource is present in this RRD, <code>false</code> otherwise
       * @Thrown in case of I/O error.
       */
      public bool containsDs(String dsName)
      {
         foreach (Datasource Datasource in Datasources)
         {
            if (Datasource.DsName.CompareTo(dsName) == 0)
            {
               return true;
            }
         }
         return false;
      }

      Datasource[] getDatasources()
      {
         return Datasources;
      }

      public Archive[] getArchives()
      {
         return archives;
      }

      /**
       * <p>Writes the RRD content to OutputStream using XML format. This format
       * is fully compatible with RRDTool's XML dump format and can be used for conversion
       * purposes or debugging.</p>
       *
       * @param destination Output stream to receive XML data
       * @Thrown in case of I/O related error
       */
      public void dumpXml(StreamWriter destination)
      {
         XmlWriter writer = XmlWriter.Create(destination);
         writer.WriteStartDocument();
         writer.WriteStartElement("rrd");
         // dump header
         header.appendXml(writer);
         // dump Datasources
         foreach (Datasource Datasource in Datasources)
         {
            Datasource.appendXml(writer);
         }
         // dump archives
         foreach (Archive archive in archives)
         {
            archive.appendXml(writer);
         }
         writer.WriteEndElement();
         writer.WriteEndDocument();
         writer.Flush();
      }

      /**
       * <p>Returns string representing internal RRD state in XML format. This format
       * is fully compatible with RRDTool's XML dump format and can be used for conversion
       * purposes or debugging.</p>
       *
       * @return Internal RRD state in XML format.
       * @ Thrown in case of I/O related error
       */
      //public synchronized String getXml() {
      //    ByteArrayOutputStream destination = new ByteArrayOutputStream(XML_INITIAL_BUFFER_CAPACITY);
      //    dumpXml(destination);
      //    return destination.ToString();
      //}

      /**
       * Dumps internal RRD state to XML file.
       * Use this XML file to convert your Rrd4n RRD to RRDTool format.
       *
       * <p>Suppose that you have a Rrd4n RRD file <code>original.rrd</code> and you want
       * to convert it to RRDTool format. First, execute the following java code:</p>
       *
       * <code>RrdDb rrd = new RrdDb("original.rrd");
       * rrd.dumpXml("original.xml");</code>
       *
       * <p>Use <code>original.xml</code> file to create the corresponding RRDTool file
       * (from your command line):
       *
       * <code>rrdtool restore copy.rrd original.xml</code>
       *
       * @param filename Path to XML file which will be created.
       * @ Thrown in case of I/O related error.
       */
      public void dumpXml(String filename)
      {
         StreamWriter outputStream = null;
         try
         {
            outputStream = new StreamWriter(filename, false);
            dumpXml(outputStream);
         }
         finally
         {
            if (outputStream != null) outputStream.Close();
         }
      }


      /**
       * Returns time of last update operation as timestamp (in seconds).
       *
       * @return Last update time (in seconds).
       */
      public long getLastUpdateTime()
      {
         lock (sync)
         {
            return header.getLastUpdateTime();
         }
      }

      public DateTime getLastUpdateDateTime()
      {
         lock (sync)
         {
            return Util.ConvertToDateTime(header.getLastUpdateTime());
         }
      }
      /**
       * <p>Returns RRD definition object which can be used to create new RRD
       * with the same creation parameters but with no data in it.</p>
       * <p/>
       * <p>Example:</p>
       * <p/>
       * <pre>
       * RrdDb rrd1 = new RrdDb("original.rrd");
       * RrdDef def = rrd1.getRrdDef();
       * // fix path
       * def.setPath("empty_copy.rrd");
       * // create new RRD file
       * RrdDb rrd2 = new RrdDb(def);
       * </pre>
       *
       * @return RRD definition.
       */
      public RrdDef getRrdDef()
      {
         lock (sync)
         {
            // set header
            long startTime = header.getLastUpdateTime();
            long step = header.getStep();
            String path = backend.getPath();
            RrdDef rrdDef = new RrdDef(path, startTime, step);
            // add Datasources
            foreach (Datasource Datasource in Datasources)
            {
               DsDef dsDef = new DsDef(Datasource.DsName,
                       Datasource.DsType, Datasource.Heartbeat,
                       Datasource.MinValue, Datasource.MaxValue);
               rrdDef.addDatasource(dsDef);
            }
            // add archives
            foreach (Archive archive in archives)
            {
               ArcDef arcDef = new ArcDef(archive.getConsolFun(),
                       archive.getXff(), archive.getSteps(), archive.getRows());
               rrdDef.addArchive(arcDef);
            }
            return rrdDef;
         }
      }

      protected void finalize()
      {
         try
         {
            close();
         }
         catch (IOException)
         { }
      }

      /**
       * Copies object's internal state to another RrdDb object.
       *
       * @param other New RrdDb object to copy state to
       * @Thrown in case of I/O error
       */
      public void copyStateTo(RrdUpdater other)
      {
         lock (sync)
         {
            if (other.GetType() != typeof(RrdDb))
            {
               throw new ArgumentException("Cannot copy RrdDb object to " + other.GetType().ToString());
            }
            RrdDb otherRrd = (RrdDb)other;
            header.copyStateTo(otherRrd.header);
            for (int i = 0; i < Datasources.Length; i++)
            {
               int j = getMatchingDatasourceIndex(this, i, otherRrd);
               if (j >= 0)
               {
                  Datasources[i].copyStateTo(otherRrd.Datasources[j]);
               }
            }
            for (int i = 0; i < archives.Length; i++)
            {
               int j = getMatchingArchiveIndex(this, i, otherRrd);
               if (j >= 0)
               {
                  archives[i].copyStateTo(otherRrd.archives[j]);
               }
            }
         }
      }

      /**
       * Returns Datasource object corresponding to the given Datasource name.
       *
       * @param dsName Datasource name
       * @return Datasource object corresponding to the give Datasource name or null
       *         if not found.
       * @Thrown in case of I/O error
       */
      public Datasource getDatasource(String dsName)
      {
         try
         {
            return getDatasource(getDsIndex(dsName));
         }
         catch (ArgumentException)
         {
            return null;
         }
      }

      /**
       * Returns index of Archive object with the given consolidation function and the number
       * of steps. Exception is thrown if such archive could not be found.
       *
       * @param consolFun Consolidation function
       * @param steps     Number of archive steps
       * @return Requested Archive object
       * @Thrown in case of I/O error
       */
      public int getArcIndex(ConsolFun consolFun, int steps)
      {
         for (int i = 0; i < archives.Length; i++)
         {
            if (archives[i].getConsolFun().Name.CompareTo(consolFun.Name) == 0 && archives[i].getSteps() == steps)
            {
               return i;
            }
         }
         throw new ArgumentException("Could not find archive " + consolFun + "/" + steps);
      }

      /**
       * Returns Archive object with the given consolidation function and the number
       * of steps.
       *
       * @param consolFun Consolidation function
       * @param steps     Number of archive steps
       * @return Requested Archive object or null if no such archive could be found
       * @Thrown in case of I/O error
       */
      public Archive getArchive(ConsolFun consolFun, int steps)
      {
         try
         {
            return getArchive(getArcIndex(consolFun, steps));
         }
         catch (ArgumentException)
         {
            return null;
         }
      }

      /**
       * Returns canonical path to the underlying RRD file. Note that this method makes sense just for
       * ordinary RRD files created on the disk - an exception will be thrown for RRD objects created in
       * memory or with custom backends.
       *
       * @return Canonical path to RRD file;
       * @Thrown in case of I/O error or if the underlying backend is
       *                     not derived from RrdFileBackend.
       */
      public String getCanonicalPath()
      {
         if (backend.GetType() != typeof(RrdFileBackend))
            throw new IOException("The underlying backend has no canonical path");

         return ((RrdFileBackend)backend).getCanonicalPath();
      }

      /**
       * Returns path to this RRD.
       *
       * @return Path to this RRD.
       */
      public String getPath()
      {
         return backend.getPath();
      }

      /**
       * Returns backend object for this RRD which performs actual I/O operations.
       *
       * @return RRD backend for this RRD.
       */
      public RrdBackend getRrdBackend()
      {
         return backend;
      }

      /**
       * Required to implement RrdUpdater interface. You should never call this method directly.
       *
       * @return Allocator object
       */
      public RrdAllocator getRrdAllocator()
      {
         return allocator;
      }

      /**
       * Returns an array of bytes representing the whole RRD.
       *
       * @return All RRD bytes
       * @Thrown in case of I/O related error.
       */
      public byte[] getBytes()
      {
         lock (sync)
         {
            return backend.readAll();
         }
      }

      /**
       * Sets default backend factory to be used. This method is just an alias for
       * {@link RrdBackendFactory#setDefaultFactory(String)}.<p>
       *
       * @param factoryName Name of the backend factory to be set as default.
       * @throws ArgumentException Thrown if invalid factory name is supplied, or not called
       *                                  before the first backend object (before the first RrdDb object) is created.
       */
      public static void setDefaultFactory(String factoryName)
      {
         RrdBackendFactory.setDefaultFactory(factoryName);
      }

      /**
       * Returns an array of last Datasource values. The first value in the array corresponds
       * to the first Datasource defined in the RrdDb and so on.
       *
       * @return Array of last Datasource values
       * @Thrown in case of I/O error
       */
      public double[] getLastDatasourceValues()
      {
         lock (sync)
         {
            double[] values = new double[Datasources.Length];
            for (int i = 0; i < values.Length; i++)
            {
               values[i] = Datasources[i].LastValue;
            }
            return values;
         }
      }

      /**
       * Returns the last stored value for the given Datasource.
       *
       * @param dsName Datasource name
       * @return Last stored value for the given Datasource
       * @ Thrown in case of I/O error
       * @throws ArgumentException Thrown if no Datasource in this RrdDb matches the given Datasource name
       */
      public double getLastDatasourceValue(String dsName)
      {
         lock (sync)
         {
            return Datasources[getDsIndex(dsName)].LastValue;
         }
      }

      /**
       * Returns the number of Datasources defined in the file
       *
       * @return The number of Datasources defined in the file
       */
      public int getDsCount()
      {
         return Datasources.Length;
      }

      /**
       * Returns the number of RRA arcihves defined in the file
       *
       * @return The number of RRA arcihves defined in the file
       */
      public int getArcCount()
      {
         return archives.Length;
      }

      /**
       * Returns the last time when some of the archives in this RRD was updated. This time is not the
       * same as the {@link #getLastUpdateTime()} since RRD file can be updated without updating any of
       * the archives.
       *
       * @return last time when some of the archives in this RRD was updated
       * @Thrown in case of I/O error
       */
      public long getLastArchiveUpdateTime()
      {
         long last = 0;
         foreach (Archive archive in archives)
         {
            last = Math.Max(last, archive.getEndTime());
         }
         return last;
      }

      public DateTime getLastArchiveUpdateDateTime()
      {
         return Util.ConvertToDateTime(getLastArchiveUpdateTime());
      }

      public String getInfo()
      {
         lock (sync)
         {
            return header.getInfo();
         }
      }

      public void setInfo(String info)
      {
         lock (sync)
         {
            header.setInfo(info);
         }
      }
      private static int getMatchingArchiveIndex(RrdDb rrd1, int arcIndex, RrdDb rrd2)
      {
         Archive archive = rrd1.getArchive(arcIndex);
         ConsolFun consolFun = archive.getConsolFun();
         int steps = archive.getSteps();
         try
         {
            return rrd2.getArcIndex(consolFun, steps);
         }
         catch (ArgumentException e)
         {
            return -1;
         }
      }

      private static int getMatchingDatasourceIndex(RrdDb rrd1, int dsIndex, RrdDb rrd2)
      {
         String dsName = rrd1.getDatasource(dsIndex).DsName;
         try
         {
            return rrd2.getDsIndex(dsName);
         }
         catch (ArgumentException e)
         {
            return -1;
         }
      }

      //public static void main(String[] args) {
      //    System.out.println("Rrd4n :: RRDTool choice for the Java world");
      //    System.out.println("==================================================================");
      //    System.out.println("Rrd4n base directory: " + Util.getRrd4nHomeDirectory());
      //    long time = Util.getTime();
      //    System.out.println("Current time: " + time + ": " + new Date(time * 1000L));
      //    System.out.println("------------------------------------------------------------------");
      //    System.out.println("For the latest information visit: https://Rrd4n.dev.java.net");
      //    System.out.println("(C) 2003-2007 Mikael Nilsson and Mathias Bogaert. All rights reserved.");
      //}

   }
}
