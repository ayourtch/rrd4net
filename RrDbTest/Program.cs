using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Castle.Windsor;
using Castle.Core.Resource;
using Castle.Windsor.Configuration.Interpreters;
using rrd4n;
using rrd4n.Core;
using rrd4n.Common;
using rrd4n.DataAccess.Data;
using rrd4n.DataAccess.Interface;
using rrd4n.Graph;
using rrd4n.Parser;

namespace RrDbTest
{
   public class GaugeSource
   {
      static int SEED = 1909752002;
      Random rnd1 = new Random();

      private double value;
      private double step;

      public GaugeSource(double value, double step)
      {
         this.value = value;
         this.step = step;
      }

      public long getValue()
      {
         double oldValue = value;
         double increment = rnd1.NextDouble() * step;
         if (rnd1.NextDouble() > 0.5)
         {
            increment *= -1;
         }
         value += increment;
         if (value <= 0)
         {
            value = 0;
         }
         return (long)Math.Round(oldValue);
      }
   }


   class Program
   {
      enum DataDateFormat
      {
         date,
         time,
         dateandtime
      };

      static int SEED = 1909752002;
      static Random RANDOM = new Random();

      static String FILE = "demo";
      static long START = rrd4n.Common.Util.getTimestamp(2003, 4, 1);
      static long END = rrd4n.Common.Util.getTimestamp(2003, 4, 2);
      //static long END = Util.getTimestamp(2003, 5, 1);
      static int MAX_STEP = 300;

      static int IMG_WIDTH = 500;
      static int IMG_HEIGHT = 300;
      static IWindsorContainer container = new WindsorContainer(new XmlInterpreter(new ConfigResource("castle")));

      static RrdDb BuildRRd(string filePath, DateTime startTime)
      {
         //rrdtool create net_test.rrd --start 920804400 DS:speed:COUNTER:600:U:U RRA:AVERAGE:0.5:1:24 RRA:AVERAGE:0.5:6:10

         RrdDef rrdDef;// = new RrdDef(filePath, startTime, 300);
         //rrdDef.addDatasource("speed", new DsType(DsType.DsTypes.COUNTER), 600, Double.NaN, Double.NaN);
         //rrdDef.addArchive(new ConsolFun(ConsolFun.ConsolFunTypes.AVERAGE), 0.5, 1, 24);
         //rrdDef.addArchive(new ConsolFun(ConsolFun.ConsolFunTypes.AVERAGE), 0.5, 6, 10);
         //Console.WriteLine(rrdDef.dump());
         DateTime startDateTime = rrd4n.Common.Util.getDate(920804400);
         RrdDbParser parser = new RrdDbParser("net_test.rrd --start \"" + startDateTime.ToString() + "\" DS:speed:COUNTER:600:U:U RRA:AVERAGE:0,5:1:24 RRA:AVERAGE:0,5:6:10");
         rrdDef = parser.CreateDatabaseDef();
         Console.WriteLine(rrdDef.dump());
         Console.WriteLine("Estimated file size: " + rrdDef.getEstimatedSize());
         return new RrdDb(rrdDef);
      }

      static void UpdateRRd(string databasePath, long timeStamp, string dataSurceName, int value)
      {
         //Sample sample = dataBase.createSample(timeStamp);
         Sample sample = new Sample(databasePath,new string[]{dataSurceName},timeStamp );
         sample.setValue(dataSurceName, value);
         RrdDbAccessInterface rrdUpdateDbAccess = container["databaseaccessor.local"] as RrdDbAccessInterface;
         rrdUpdateDbAccess.StoreData(sample);

         sample.clearValues();
      }

      private static List<FetchedData> ReadAndUnifyData(string dataPath, TimeSpan expectedTick)
      {
         DataDateFormat dataDateFormat;

         List<FetchedData> rawData = new List<FetchedData>();
         List<FetchedData> unifiedData = new List<FetchedData>();
         //         Dictionary<DateTime, double> rawData = new Dictionary<DateTime, double>();
         DateTime fromDate;
         DateTime nextDate;
         double deltaValue;
         double span;
         int datapoints;

         using (StreamReader reader = new StreamReader(dataPath))
         {
            // Remove columns header
            string line = reader.ReadLine();
            if (line.StartsWith("Year;Month;Day;Value"))
               dataDateFormat = DataDateFormat.date;
            else if (line.StartsWith("Year;Month;Day;Hour;Minute;Second;Value"))
               dataDateFormat = DataDateFormat.dateandtime;
            else if (line.StartsWith("Hour;Minute;Second;Value"))
               dataDateFormat = DataDateFormat.time;
            else
               throw new ApplicationException("Wrong header format." + line);

            line = reader.ReadLine();


            double value;
            DateTime timeStamp = DateTime.MinValue;
            while (!string.IsNullOrEmpty(line))
            {
               try
               {
                  var values = line.Split(';');
                  value = double.Parse(values[3]);
                  timeStamp = DateTime.MinValue;
                  switch (dataDateFormat)
                  {
                     case DataDateFormat.date:
                        timeStamp = new DateTime(int.Parse(values[0]), int.Parse(values[1]), int.Parse(values[2]));
                        break;
                     case DataDateFormat.time:
                        timeStamp = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, int.Parse(values[0]), int.Parse(values[1]), int.Parse(values[2]));
                        break;
                     case DataDateFormat.dateandtime:
                        timeStamp = new DateTime(int.Parse(values[0]), int.Parse(values[1]), int.Parse(values[2]), int.Parse(values[3]), int.Parse(values[4]), int.Parse(values[5]));
                        value = double.Parse(values[6]);
                        break;
                  }
               }
               catch (ArgumentException)
               {
                  value = double.NaN;
               }
               rawData.Add(new FetchedData { TimeStamp = timeStamp, Value = value });
               line = reader.ReadLine();
            }


         } if (expectedTick == TimeSpan.MinValue)
         {
            return new List<FetchedData>(rawData);
         }


         unifiedData.Add(rawData[0]);
         fromDate = rawData[0].TimeStamp;
         nextDate = fromDate + expectedTick;
         double fromvalue = rawData[0].Value;
         int dataIndex = 1;
         while (dataIndex < rawData.Count)
         {
            DateTime dataTimeStamp = rawData[dataIndex].TimeStamp;
            while (dataTimeStamp < nextDate)
            {
               dataIndex++;
               if (dataIndex > rawData.Count)
                  break;
               dataTimeStamp = rawData[dataIndex].TimeStamp;
            }
            if (dataIndex > rawData.Count)
               break;

            int deltaSeconds = (int)(dataTimeStamp - fromDate).TotalSeconds;
            double diff = rawData[dataIndex].Value - fromvalue;
            deltaValue = (expectedTick.TotalSeconds / deltaSeconds) * diff;
            if (deltaValue < 0)
               continue;
            if (nextDate == dataTimeStamp)
            {
               deltaValue = 0;
            }
            while (nextDate < dataTimeStamp)
            {
               fromvalue += deltaValue;
               unifiedData.Add(new FetchedData { TimeStamp = nextDate, Value = fromvalue });
               nextDate += expectedTick;
            }
            fromDate = rawData[dataIndex].TimeStamp;
            //nextDate = fromDate + expectedTick;
            fromvalue = rawData[dataIndex].Value;
            dataIndex++;
         }
         return unifiedData;
      }

      static void Main(string[] args)
      {
         DateTime EPOC = new DateTime(1970, 01, 1);
         long startTimeInSeconds = 920804400;
         DateTime startTime = EPOC.AddSeconds(startTimeInSeconds);
         long endTimeInSeconds = 920808000;
         string rrdPath = @"net_test.rrd";
         String imgPath = @"net_test.png";
         Console.WriteLine("== Starting demo");
         
         RrdDb rrdDb;
         FetchRequest request;
         FetchData fetchData;


         //List<FetchedData> unified = ReadAndUnifyData(@"C:\Development\CS_Project\rrd4n\RrDbTest\el.csv",
         //                                             new TimeSpan(0, 10, 0));

         bool createDb = true; 
         if (createDb)
         {
            rrdDb = BuildRRd(rrdPath, startTime);
            Console.WriteLine(rrdDb.dump());
            rrdDb.close();
            //rrdDb = new RrdDb(rrdPath, false);

            int[] values = new int[] {
                        12345,12357,12363,12363,12363,12373, 12383,12393,12399,12405,12411 ,12415,12420 ,12422 ,12423
                        };

            //rrdtool update net_test.rrd 920804700:12345 920805000:12357 920805300:12363
            //rrdtool update net_test.rrd 920805600:12363 920805900:12363 920806200:12373
            //rrdtool update net_test.rrd 920806500:12383 920806800:12393 920807100:12399
            //rrdtool update net_test.rrd 920807400:12405 920807700:12411 920808000:12415
            //rrdtool update net_test.rrd 920808300:12420 920808600:12422 920808900:12423

            for (int i = 0; i < 15; i++)
            {
               UpdateRRd(rrdPath, 920804700 + (i * 300), "speed", values[i]);
            }
            //rrdDb.close();

            // Read back test
            rrdDb = new RrdDb(rrdPath, true);
            Console.WriteLine("File reopen in read-only mode");
            Console.WriteLine("== Last update time was: " + rrdDb.getLastUpdateTime());
            Console.WriteLine("== Last info was: " + rrdDb.getInfo());

            // fetch data
            Console.WriteLine("== Fetching data");
            request = rrdDb.createFetchRequest(new ConsolFun(ConsolFun.ConsolFunTypes.AVERAGE), startTimeInSeconds, endTimeInSeconds);
            Console.WriteLine(request.dump());
            fetchData = rrdDb.fetchData(request);
            Console.WriteLine("== Data fetched. " + fetchData.getRowCount() + " points obtained");
            Console.WriteLine(fetchData.toString());
            Console.WriteLine("== Fetch completed");
         }


         DateTime startDateTime = rrd4n.Common.Util.getDate(920804400);
         DateTime endDateTime = rrd4n.Common.Util.getDate(920808000);

         GraphParser parser = new GraphParser("net_speed_1.png --start \"" + startDateTime.ToString() + "\" --end \"" + endDateTime.ToString() + "\" --imgformat PNG DEF:myspeed=" + rrdPath + ":speed:AVERAGE LINE2:myspeed#FF0000");
         RrdGraphDef gDef_1 = parser.CreateGraphDef();

         RrdDbAccessInterface rrdDbAccess = container["databaseaccessor.local"] as RrdDbAccessInterface;
         RrdGraph graph_1 = new RrdGraph(gDef_1,rrdDbAccess);

         // Create graph
         // rrdtool graph net_speed.png --start 920804400 --end 920808000 
         //  DEF:myspeed=net_test.rrd:speed:AVERAGE 
         //  LINE2:myspeed#FF0000 
         //  --font "DEFAULT:0:C:\Windows\fonts\cour.ttf"
         Console.WriteLine("Creating graph ");
         RrdGraphDef gDef = new RrdGraphDef();
         gDef.setWidth(IMG_WIDTH);
         gDef.setHeight(IMG_HEIGHT);
         gDef.setFilename(imgPath);
         gDef.setStartTime(startTimeInSeconds);
         gDef.setEndTime(endTimeInSeconds);
         gDef.setTitle("Speed");
         //            gDef.setVerticalLabel("temperature");
         gDef.datasource("myspeed", rrdPath, "speed", new rrd4n.Common.ConsolFun(rrd4n.Common.ConsolFun.ConsolFunTypes.AVERAGE));
         gDef.line("myspeed", Color.Red, "My sPeedj", 2);
         gDef.hrule(0.02, Color.Red, "Maximum 200", 3);

         //            gDef.print("shade", new ConsolFun(ConsolFun.ConsolFunTypes.AVERAGE), "avgShade = %.3f%S\\r");
         //            gDef.setImageInfo("<img src='%s' width='%d' height = '%d'>");
         gDef.setPoolUsed(false);
         gDef.setImageFormat("png");
         //Console.WriteLine("Rendering graph " + rrd4n.Common.Util.getLapTime());
         // create graph finally
         RrdGraph graph = new RrdGraph(gDef, rrdDbAccess);

         // Create bar chart test graph
         //rrdtool graph speed3.png --start 920804400 --end 920808000 --vertical-label km/h DEF:myspeed=test.rrd:speed:AVERAGE "CDEF:kmh=myspeed,3600,*" CDEF:fast=kmh,100,GT,kmh,0,IF CDEF:good=kmh,100,GT,0,kmh,IF HRULE:100#0000FF:"Maximum allowed" AREA:good#00FF00:"Good speed" AREA:fast#FF0000:"Too fast" --font "DEFAULT:0:C:\Windows\fonts\cour.ttf"
         imgPath = @"net_test_bar.png";

         Console.WriteLine("Creating bar graph ");
         gDef = new RrdGraphDef();
         gDef.setWidth(IMG_WIDTH);
         gDef.setHeight(IMG_HEIGHT);
         gDef.setFilename(imgPath);
         gDef.setStartTime(startTimeInSeconds);
         gDef.setEndTime(endTimeInSeconds + 900);
         gDef.setTitle("Speed");
         gDef.setVerticalLabel("km/h");
         //DEF:myspeed=test.rrd:speed:AVERAGE
         gDef.datasource("myspeed", rrdPath, "speed", new ConsolFun(ConsolFun.ConsolFunTypes.AVERAGE));
         //"CDEF:kmh=myspeed,3600,*" 
         gDef.datasource("kmh", "myspeed,3600,*");
         //CDEF:fast=kmh,100,GT,kmh,0,IF 
         gDef.datasource("fast", "kmh,100,GT,kmh,0,IF");
         //CDEF:good=kmh,100,GT,0,kmh,IF HRULE:100#0000FF:"Maximum allowed" AREA:good#00FF00:"Good speed" AREA:fast#FF0000:"Too fast"
         gDef.datasource("good", "kmh,100,GT,0,kmh,IF");
         //HRULE:100#0000FF:"Maximum allowed" 
         gDef.hrule(100, Color.Red, "Maximum allowed", 3);
         gDef.hrule(200, Color.Red, "Maximum 200", 3);
         // AREA:good#00FF00:"Good speed" 
         gDef.area("good",Color.Green,"Good speed");
         // AREA:fast#FF0000:"Too fast"
         gDef.area("fast", Color.Red, "Too fast");
         gDef.setPoolUsed(false);
         gDef.setImageFormat("png");
         //Console.WriteLine("Rendering graph " + Util.getLapTime());
         // create graph finally
         graph = new RrdGraph(gDef,rrdDbAccess);

         //rrdtool graph speed4.png --start 920804400 --end 920808000 --vertical-label km/h DEF:myspeed=test.rrd:speed:AVERAGE CDEF:nonans=myspeed,UN,0,myspeed,IF CDEF:kmh=nonans,3600,* CDEF:fast=kmh,100,GT,100,0,IF CDEF:over=kmh,100,GT,kmh,100,-,0,IF CDEF:good=kmh,100,GT,0,kmh,IF HRULE:100#0000FF:"Maximum allowed" AREA:good#00FF00:"Good speed" AREA:fast#550000:"Too fast"  STACK:over#FF0000:"Over speed" --font "DEFAULT:0:C:\Windows\fonts\cour.ttf"
         Console.WriteLine("Creating stack graph ");
         imgPath = @"net_test_stack.png";
         gDef = new RrdGraphDef();
         gDef.setWidth(IMG_WIDTH);
         gDef.setHeight(IMG_HEIGHT);
         gDef.setFilename(imgPath);
         gDef.setStartTime(startTimeInSeconds + 300);
         gDef.setEndTime(endTimeInSeconds + 1200);
         gDef.setTitle("Speed");
         //--vertical-label km/h 
         gDef.setVerticalLabel("km/h");
         // DEF:myspeed=test.rrd:speed:AVERAGE 
         gDef.datasource("myspeed", rrdPath, "speed", new ConsolFun(ConsolFun.ConsolFunTypes.AVERAGE));
         // CDEF:nonans=myspeed,UN,0,myspeed,IF 
         gDef.datasource("nonans", "myspeed,UN,0,myspeed,IF");
         //CDEF:kmh=nonans,3600,* 
         gDef.datasource("kmh", "nonans,3600,*");
         //CDEF:fast=kmh,100,GT,100,0,IF 
         gDef.datasource("fast", "kmh,100,GT,100,0,IF");
         //CDEF:over=kmh,100,GT,kmh,100,-,0,IF 
         gDef.datasource("over", "kmh,100,GT,kmh,100,-,0,IF");
         //CDEF:good=kmh,100,GT,0,kmh,IF 
         gDef.datasource("good", "kmh,100,GT,0,kmh,IF");
         //HRULE:100#0000FF:"Maximum allowed" 
         gDef.hrule(100, Color.Blue, "Maximum allowed", 3);
         // AREA:good#00FF00:"Good speed" 
         gDef.area("good", Color.Green, "Good speed");
         // AREA:fast#550000:"Too fast"  
         gDef.area("fast", Color.Black, "Too fast");
         //STACK:over#FF0000:"Over speed" 
         gDef.stack("over", Color.Red, "Over speed");
         
         gDef.setPoolUsed(false);
         gDef.setImageFormat("png");
         //Console.WriteLine("Rendering graph " + Util.getLapTime());
         // create graph finally
         graph = new RrdGraph(gDef,rrdDbAccess);



















         long startMillis = DateTime.Now.Millisecond;
         return;
         //if (args.Length > 0)
         //{
         //   Console.WriteLine("Setting default backend factory to " + args[0]);
         //   RrdDb.setDefaultFactory(args[0]);
         //}
         //long start = START;
         //long end = END;
         ////rrdPath = Util.getRrd4nDemoPath(FILE + ".rrd");
         ////String xmlPath = Util.getRrd4nDemoPath(FILE + ".xml");
         ////String rrdRestoredPath = Util.getRrd4nDemoPath(FILE + "_restored.rrd");
         ////imgPath = Util.getRrd4nDemoPath(FILE + ".png");
         ////String logPath = Util.getRrd4nDemoPath(FILE + ".log");
         ////PrintWriter log = new PrintWriter(new BufferedOutputStream(new FileOutputStream(logPath, false)));
         //// creation
         ////Console.WriteLine("== Creating RRD file " + rrdPath);
         ////RrdDef rrdDef = new RrdDef(rrdPath, start - 1, 300);
         ////rrdDef.addDatasource("sun", new DsType(DsType.DsTypes.GAUGE), 600, 0, Double.NaN);
         ////rrdDef.addDatasource("shade", new DsType(DsType.DsTypes.GAUGE), 600, 0, Double.NaN);
         ////rrdDef.addArchive(new ConsolFun(ConsolFun.ConsolFunTypes.AVERAGE), 0.5, 1, 600);
         ////rrdDef.addArchive(new ConsolFun(ConsolFun.ConsolFunTypes.AVERAGE), 0.5, 6, 700);
         ////rrdDef.addArchive(new ConsolFun(ConsolFun.ConsolFunTypes.AVERAGE), 0.5, 24, 775);
         ////rrdDef.addArchive(new ConsolFun(ConsolFun.ConsolFunTypes.AVERAGE), 0.5, 288, 797);
         ////rrdDef.addArchive(new ConsolFun(ConsolFun.ConsolFunTypes.MAX), 0.5, 1, 600);
         ////rrdDef.addArchive(new ConsolFun(ConsolFun.ConsolFunTypes.MAX), 0.5, 6, 700);
         ////rrdDef.addArchive(new ConsolFun(ConsolFun.ConsolFunTypes.MAX), 0.5, 24, 775);
         ////rrdDef.addArchive(new ConsolFun(ConsolFun.ConsolFunTypes.MAX), 0.5, 288, 797);
         ////Console.WriteLine(rrdDef.dump());
         //////log.Console.WriteLine(rrdDef.dump());
         ////Console.WriteLine("Estimated file size: " + rrdDef.getEstimatedSize());
         ////RrdDb rrdDb = new RrdDb(rrdDef);
         ////Console.WriteLine("== RRD file created.");
         ////if (rrdDb.getRrdDef().equals(rrdDef))
         ////{
         ////    Console.WriteLine("Checking RRD file structure... OK");
         ////}
         ////else
         ////{
         ////    Console.WriteLine("Invalid RRD file created. This is a serious bug, bailing out");
         ////    return;
         ////}
         ////rrdDb.close();
         ////Console.WriteLine("== RRD file closed.");

         ////// update database
         ////GaugeSource sunSource = new GaugeSource(1200, 20);
         ////GaugeSource shadeSource = new GaugeSource(300, 10);
         ////Console.WriteLine("== Simulating one month of RRD file updates with step not larger than " +
         ////        MAX_STEP + " seconds (* denotes 1000 updates)");
         ////long t = start;
         ////int n = 0;
         ////rrdDb = new RrdDb(rrdPath);
         ////Sample sample = rrdDb.createSample();

         ////while (t <= end + 86400L)
         ////{
         ////    sample.setTime(t);
         ////    sample.setValue("sun", sunSource.getValue());
         ////    sample.setValue("shade", shadeSource.getValue());
         ////    //log.Console.WriteLine(sample.dump());
         ////    sample.update();
         ////    t += (long)(RANDOM.NextDouble() * MAX_STEP) + 1;
         ////    if (((++n) % 1000) == 0)
         ////    {
         ////        Console.Write("*");
         ////    }
         ////}

         ////rrdDb.close();

         ////Console.WriteLine("");
         ////Console.WriteLine("== Finished. RRD file updated " + n + " times");

         //// test read-only access!
         //rrdDb = new RrdDb(rrdPath, true);
         //Console.WriteLine("File reopen in read-only mode");
         //Console.WriteLine("== Last update time was: " + rrdDb.getLastUpdateTime());
         //Console.WriteLine("== Last info was: " + rrdDb.getInfo());

         //// fetch data
         //Console.WriteLine("== Fetching data for the whole month");
         //request = rrdDb.createFetchRequest(new ConsolFun(ConsolFun.ConsolFunTypes.AVERAGE), start, end);
         //Console.WriteLine(request.dump());
         ////  log.Console.WriteLine(request.dump());
         //fetchData = request.fetchData();
         //Console.WriteLine("== Data fetched. " + fetchData.getRowCount() + " points obtained");
         //Console.WriteLine(fetchData.toString());
         //Console.WriteLine("== Dumping fetched data to XML format");
         ////  Console.WriteLine(fetchData.exportXml());
         //Console.WriteLine("== Fetch completed");

         //// dump to XML file
         //Console.WriteLine("== Dumping RRD file to XML file " + xmlPath + " (can be restored with RRDTool)");
         ////  rrdDb.exportXml(xmlPath);
         //Console.WriteLine("== Creating RRD file " + rrdRestoredPath + " from XML file " + xmlPath);
         ////  RrdDb rrdRestoredDb = new RrdDb(rrdRestoredPath, xmlPath);

         //// close files
         //Console.WriteLine("== Closing both RRD files");
         //rrdDb.close();
         //Console.WriteLine("== First file closed");
         ////  rrdRestoredDb.close();
         //Console.WriteLine("== Second file closed");

         //// create graph
         //Console.WriteLine("Creating graph " + Util.getLapTime());
         //Console.WriteLine("== Creating graph from the second file");
         //gDef = new RrdGraphDef();
         //gDef.setWidth(IMG_WIDTH);
         //gDef.setHeight(IMG_HEIGHT);
         //gDef.setFilename(imgPath);
         //gDef.setStartTime(start);
         //gDef.setEndTime(end);
         //gDef.setTitle("Temperatures in May 2003");
         //gDef.setVerticalLabel("temperature");
         //gDef.datasource("sun", rrdPath/*rrdRestoredPath*/, "sun", new ConsolFun(ConsolFun.ConsolFunTypes.AVERAGE));
         //gDef.datasource("shade", rrdPath/*rrdRestoredPath*/, "shade", new ConsolFun(ConsolFun.ConsolFunTypes.AVERAGE));
         //gDef.datasource("median", "sun,shade,+,2,/");
         //gDef.datasource("diff", "sun,shade,-,ABS,-1,*");
         //gDef.datasource("sine", "TIME," + start + ",-," + (end - start) +
         //        ",/,2,PI,*,*,SIN,1000,*");
         //gDef.line("sun", Color.Green, "sun temp");
         //gDef.line("shade", Color.Blue, "shade temp");
         //gDef.line("median", Color.Magenta, "median value");
         //gDef.area("diff", Color.Yellow, "difference\\r");
         //gDef.line("diff", Color.Red, null);
         //gDef.line("sine", Color.Cyan, "sine function demo\\r");
         //gDef.hrule(2568, Color.Green, "hrule");
         //gDef.vrule((start + 2 * end) / 3, Color.Magenta, "vrule\\r");
         //gDef.gprint("sun", new ConsolFun(ConsolFun.ConsolFunTypes.MAX), "maxSun = %.3f%s");
         //gDef.gprint("sun", new ConsolFun(ConsolFun.ConsolFunTypes.AVERAGE), "avgSun = %.3f%S\\r");
         //gDef.gprint("shade", new ConsolFun(ConsolFun.ConsolFunTypes.MAX), "maxShade = %.3f%S");
         //gDef.gprint("shade", new ConsolFun(ConsolFun.ConsolFunTypes.AVERAGE), "avgShade = %.3f%S\\r");
         //gDef.print("sun", new ConsolFun(ConsolFun.ConsolFunTypes.MAX), "maxSun = %.3f%s");
         //gDef.print("sun", new ConsolFun(ConsolFun.ConsolFunTypes.AVERAGE), "avgSun = %.3f%S\\r");
         //gDef.print("shade", new ConsolFun(ConsolFun.ConsolFunTypes.MAX), "maxShade = %.3f%S");
         //gDef.print("shade", new ConsolFun(ConsolFun.ConsolFunTypes.AVERAGE), "avgShade = %.3f%S\\r");
         //gDef.setImageInfo("<img src='%s' width='%d' height = '%d'>");
         //gDef.setPoolUsed(false);
         //gDef.setImageFormat("png");
         //Console.WriteLine("Rendering graph " + Util.getLapTime());
         //// create graph finally
         //graph = new RrdGraph(gDef);

         //Console.WriteLine(graph.getRrdGraphInfo().dump());
         //Console.WriteLine("== Graph created " + Util.getLapTime());
         //// locks info
         ////Console.WriteLine("== Locks info ==");
         ////Console.WriteLine(RrdSafeFileBackend.getLockInfo());
         //// demo ends
         ////log.close();
         //Console.WriteLine("== Demo completed in " +
         //        ((DateTime.Now.Millisecond - startMillis) / 1000.0) + " sec");


      }
      public class FetchedData
      {
         public DateTime TimeStamp { get; set; }
         public double Value { get; set; }
      }

   }
}
