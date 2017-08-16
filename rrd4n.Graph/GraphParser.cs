using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using rrd4n.Common;
using rrd4n.Graph;

namespace rrd4n.Graph
{
   public class GraphParser : CommandParser
   {
      static string IN_MEMORY_IMAGE = "-";

      static Color BLIND_COLOR = Color.FromArgb(0, 0, 0);
      private const string DEFAULT_START = "now-10s";
      static string DEFAULT_END = "now";
      private RrdGraphDef gdef = new RrdGraphDef();

      public GraphParser(string command)
      {
         TokenizeCommand(command);
      }

      public RrdGraphDef CreateGraphDef()
      {
         
         // OPTIONS

         // START, END
         string startTimeValue = getOptionValue("s", "start", DEFAULT_START);
         string endTimeValue = getOptionValue("e", "end", DEFAULT_END);

         long[] times = Util.getTimestamps(startTimeValue, endTimeValue);// GetTime("s", "start");
         long t1 = Util.getTimestamp();
         long t2 = Util.getTimestamp(DateTime.Now.AddSeconds(-10));
//         long endTime = GetTime("e", "end");

         gdef.setTimeSpan(times[0], times[1]);
         parseXGrid(getOptionValue("x", "x-grid"));
         parseYGrid(getOptionValue("y", "y-grid"));
         gdef.setAltYGrid(getBooleanOption("Y", "alt-y-grid"));
         gdef.setNoMinorGrid(getBooleanOption(null, "no-minor"));
         gdef.setAltYMrtg(getBooleanOption("R", "alt-y-mrtg"));
         gdef.setAltAutoscale(getBooleanOption("A", "alt-autoscale"));
         gdef.setAltAutoscaleMax(getBooleanOption("M", "alt-autoscale-max"));
         String opt = getOptionValue("X", "units-exponent");
         if (opt != null)
            gdef.setUnitsExponent(int.Parse(opt));

         opt = getOptionValue("L", "units-length");
         if (opt != null)
            gdef.setUnitsLength(int.Parse(opt));

         opt = getOptionValue("v", "vertical-label");
         if (opt != null)
            gdef.setVerticalLabel(opt);

         opt = getOptionValue("w", "width");
         if (opt != null)
            gdef.setWidth(int.Parse(opt));
         opt = getOptionValue("h", "height");
         if (opt != null)
            gdef.setHeight(int.Parse(opt));
         gdef.setInterlaced(getBooleanOption("i", "interlaced"));
         opt = getOptionValue("f", "imginfo");
         if (opt != null)
            gdef.setImageInfo(opt);
         opt = getOptionValue("a", "imgformat");
         if (opt != null)
            gdef.setImageFormat(opt);
         opt = getOptionValue("B", "background");
         if (opt != null)
            gdef.setBackgroundImage(opt);
         opt = getOptionValue("O", "overlay");
         if (opt != null)
            gdef.setOverlayImage(opt);
         opt = getOptionValue("U", "unit");
         if (opt != null)
            gdef.setUnit(opt);
         gdef.setLazy(getBooleanOption("z", "lazy"));
         opt = getOptionValue("u", "upper-limit");
         if (opt != null)
            gdef.setMaxValue(double.Parse(opt));
         opt = getOptionValue("l", "lower-limit");
         if (opt != null)
            gdef.setMinValue(double.Parse(opt));
         gdef.setRigid(getBooleanOption("r", "rigid"));
         opt = getOptionValue("b", "base");
         if (opt != null)
            gdef.setBase(double.Parse(opt));

         gdef.setLogarithmic(getBooleanOption("o", "logarithmic"));
         parseColors(getMultipleOptionValues("c", "color"));
         gdef.setNoLegend(getBooleanOption("g", "no-legend"));
         gdef.setOnlyGraph(getBooleanOption("j", "only-graph"));
         gdef.setForceRulesLegend(getBooleanOption("F", "force-rules-legend"));
         opt = getOptionValue("t", "title");
         if (opt != null)
            gdef.setTitle(opt);

         opt = getOptionValue("S", "step");
         if (opt != null)
            gdef.setStep(long.Parse(opt));

         // NON-OPTIONS

         String[] words = getRemainingWords();
         // the first word must be a filename
         if (words.Length < 1)
         {
            throw new ArgumentException("Image filename must be specified");
         }
         gdef.setFilename(words[0]);
         // parse remaining words, in no particular order
         for (int i = 1; i < words.Length; i++)
         {
            if (words[i].StartsWith("DEF:"))
            {
               parseDef(words[i]);
            }
            else if (words[i].StartsWith("CDEF:"))
            {
               parseCDef(words[i]);
            }
            else if (words[i].StartsWith("SDEF:")
               || words[i].StartsWith("VDEF:"))
            {
               parseSDef(words[i]);
            }
            else if (words[i].StartsWith("PRINT:"))
            {
               parsePrint(words[i]);
            }
            else if (words[i].StartsWith("GPRINT:"))
            {
               parseGPrint(words[i]);
            }
            else if (words[i].StartsWith("COMMENT:"))
            {
               parseComment(words[i]);
            }
            else if (words[i].StartsWith("HRULE:"))
            {
               parseHRule(words[i]);
            }
            else if (words[i].StartsWith("VRULE:"))
            {
               parseVRule(words[i]);
            }
            else if (words[i].StartsWith("LINE1:") || words[i].StartsWith("LINE2:") || words[i].StartsWith("LINE3:"))
            {
               parseLine(words[i]);
            }
            else if (words[i].StartsWith("AREA:"))
            {
               parseArea(words[i]);
            }
            else if (words[i].StartsWith("STACK:"))
            {
               parseStack(words[i]);
            }
            else if (words[i].StartsWith("SHIFT:"))
            {
               ParseShift(words[i]);
            }
            else
            {
               throw new ArgumentException("Unexpected GRAPH token encountered: " + words[i]);
            }
         }
         // create diagram finally
         return gdef;

         //RrdGraphInfo info = new RrdGraph(gdef).getRrdGraphInfo();
         //if (info.getFilename().CompareTo(RrdGraphConstants.IN_MEMORY_IMAGE) == 0)
         //{
         //   //println(new String(info.getBytes()));
         //}
         //else
         //{
         //   //println(info.getWidth() + "x" + info.getHeight());
         //   //String[] plines = info.getPrintLines();
         //   //   foreach (String pline in plines) {
         //   //       println(pline);
         //   //   }
         //   //if(info.getImgInfo() != null && info.getImgInfo().Length() > 0) {
         //   //   println(info.getImgInfo());
         //}
         //return info;
      }

      private void parseLine(String word)
      {
         String[] tokens1 = CommandSplitter.split(word);
         if (tokens1.Length != 2 && tokens1.Length != 3) throw new ArgumentException("Invalid LINE statement: " + word);

         String[] tokens2 = tokens1[1].Split('#');
         if (tokens2.Length != 1 && tokens2.Length != 2) throw new ArgumentException("Invalid LINE statement: " + word);

         float width = int.Parse(tokens1[0].Substring(tokens1[0].Length - 1));
         String name = tokens2[0];
         Color color = tokens2.Length == 2 ? Util.parseColor(tokens2[1]) : BLIND_COLOR;
         String legend = tokens1.Length == 3 ? tokens1[2] : null;
         gdef.line(name, color, legend, width);
      }

      private void parseArea(String word)
      {
         String[] tokens1 = CommandSplitter.split(word);
         if (tokens1.Length != 2 && tokens1.Length != 3) throw new ArgumentException("Invalid AREA statement: " + word);

         String[] tokens2 = tokens1[1].Split('#');
         if (tokens2.Length != 1 && tokens2.Length != 2) throw new ArgumentException("Invalid AREA statement: " + word);

         String name = tokens2[0];
         Color color = tokens2.Length == 2 ? Util.parseColor(tokens2[1]) : BLIND_COLOR;
         String legend = tokens1.Length == 3 ? tokens1[2] : null;
         gdef.area(name, color, legend);
      }

      private void parseStack(String word)
      {
         String[] tokens1 = CommandSplitter.split(word);
         if (tokens1.Length != 2 && tokens1.Length != 3) throw new ArgumentException("Invalid STACK statement: " + word);

         String[] tokens2 = tokens1[1].Split('#');
         if (tokens2.Length != 1 && tokens2.Length != 2) throw new ArgumentException("Invalid STACK statement: " + word);

         String name = tokens2[0];
         Color color = tokens2.Length == 2 ? Util.parseColor(tokens2[1]) : BLIND_COLOR;
         String legend = tokens1.Length == 3 ? tokens1[2] : null;
         gdef.stack(name, color, legend);
      }

      private void parseHRule(String word)
      {
         String[] tokens1 = CommandSplitter.split(word);
         if (tokens1.Length < 2 || tokens1.Length > 3) throw new ArgumentException("Invalid HRULE statement: " + word);

         String[] tokens2 = tokens1[1].Split('#');
         if (tokens2.Length != 2) throw new ArgumentException("Invalid HRULE statement: " + word);

         double value = double.Parse(tokens2[0]);
         Color color = Util.parseColor(tokens2[1]);
         gdef.hrule(value, color, tokens1.Length == 3 ? tokens1[2] : null);
      }

      private void parseVRule(String word)
      {
         String[] tokens1 = CommandSplitter.split(word);
         if (tokens1.Length < 2 || tokens1.Length > 3) throw new ArgumentException("Invalid VRULE statement: " + word);

         String[] tokens2 = tokens1[1].Split('#');
         if (tokens2.Length != 2)
            throw new ArgumentException("Invalid VRULE statement: " + word);

         long timestamp;
         if (!long.TryParse(tokens2[0], out timestamp))
         {
            DateTime time;
            if (!DateTime.TryParse(tokens2[0], out time))
               throw new ArgumentException("Wrong time format in VRULE " + tokens2[0]);
            timestamp = Util.getTimestamp(time);
         }
         Color color = Util.parseColor(tokens2[1]);
         gdef.vrule(timestamp, color, tokens1.Length == 3 ? tokens1[2] : null);
      }

      private void parseComment(String word)
      {
         String[] tokens = CommandSplitter.split(word);
         if (tokens.Length != 2)
            throw new ArgumentException("Invalid COMMENT specification: " + word);
         gdef.comment(tokens[1]);
      }

      //DEF:<vname>=<rrdfile>:<ds-name>:<CF>[:step=<step>][:start=<time>][:end=<time>][:reduce=<CF>] 
      private void parseDef(String word)
      {
         String[] tokens1 = CommandSplitter.split(word);
         if (tokens1.Length < 4) throw new ArgumentException("Invalid DEF specification: " + word);
         int parameterIndex = 1;
         string[] pair = tokens1[parameterIndex].Split('=');
         Def def = new Def(pair[0], pair[1]);

         parameterIndex++;
         def.dsName = tokens1[parameterIndex];
         parameterIndex++;
         def.SetConsulFunType(tokens1[parameterIndex]);
         parameterIndex++;
         while (parameterIndex < tokens1.Length)
         {
            pair = tokens1[parameterIndex].Split('=');
            switch (pair[0])
            {
               case "step":
                  def.Step = long.Parse(pair[1]);
                  break;
               case "start":
                  def.StartTime = Util.ParseDateTime(pair[1]);
                  break;
               case "end":
                  def.EndTime = Util.ParseDateTime(pair[1]);
                  break;
               case "reduce":
                  def.ReduceName = pair[1];
                  break;
            }
            parameterIndex++;
         }
         gdef.AddDatasource(def);
      }

      private void parseCDef(String word)
      {
         String[] tokens1 = CommandSplitter.split(word);
         if (tokens1.Length != 2) throw new ArgumentException("Invalid CDEF specification: " + word);

         String[] tokens2 = tokens1[1].Split('=');
         if (tokens2.Length != 2) throw new ArgumentException("Invalid DEF specification: " + word);
   
         gdef.datasource(tokens2[0], tokens2[1]);
      }

      private void parseSDef(String word)
      {
         String[] tokens1 = CommandSplitter.split(word);
         if (tokens1.Length != 2) throw new ArgumentException("Invalid SDEF specification: " + word);

         String[] tokens2 = tokens1[1].Split('=');
         if (tokens2.Length != 2) throw new ArgumentException("Invalid SDEF specification: " + word);

         string[] tokens3 = tokens2[1].Split(',');
         gdef.datasource(tokens2[0], tokens3[0], AggregateFunction.Create(tokens3[1]));
      }



      private void parsePrint(String word)
      {
         String[] tokens = CommandSplitter.split(word);
         bool strftime = (tokens[tokens.Length - 1].Contains("strftime"));
         if (tokens.Length < 3) throw new ArgumentException("Invalid GPRINT specification: " + word);
         gdef.print(tokens[1], tokens[2], strftime);
      }


      //GPRINT:vname:format[:strftime] in case of VDEF-based vname
      private void parseGPrint(String word)
      {
         String[] tokens = CommandSplitter.split(word);
         bool strftime = (tokens[tokens.Length - 1].Contains("strftime"));

         if (tokens.Length < 3) throw new ArgumentException("Invalid GPRINT specification: " + word);

         gdef.gprint(tokens[1], tokens[2], strftime);
      }

      private void ParseShift(String word)
      {
         String[] tokens = CommandSplitter.split(word);
         if (tokens.Length < 3) throw new ArgumentException("Invalid SHIFT specification: " + word);
         gdef.datasource(tokens[1],long.Parse(tokens[2]));
      }

      private void parseColors(String[] colorOptions)
      {
         if (colorOptions == null)
            return;

         foreach (String colorOption in colorOptions)
         {
            String[] tokens = colorOption.Split('#');
            if (tokens.Length != 2) throw new ArgumentException("Invalid COLOR specification: " + colorOption);

            String colorName = tokens[0];
            Color Color = Util.parseColor(tokens[1]);
            gdef.setColor(colorName, Color);
         }
      }

      private void parseYGrid(String ygrid)
      {
         if (ygrid == null)
            return;

         if (ygrid.ToLower().CompareTo("none") == 0)
         {
            gdef.setDrawYGrid(false);
            return;
         }
         String[] tokens = ygrid.Split(':');
         if (tokens.Length != 2) throw new ArgumentException("Invalid YGRID settings: " + ygrid);

         double gridStep = double.Parse(tokens[0]);
         int labelFactor = int.Parse(tokens[1]);
         gdef.setValueAxis(gridStep, labelFactor);
      }

      private void parseXGrid(String xgrid)
      {
         if (xgrid == null)
            return;

         if (xgrid.ToLower().CompareTo("none") == 0)
         {
            gdef.setDrawXGrid(false);
            return;
         }
         String[] tokens = xgrid.Split(':');
         if (tokens.Length != 8) throw new ArgumentException("Invalid XGRID settings: " + xgrid);

         int minorUnit = resolveUnit(tokens[0]), majorUnit = resolveUnit(tokens[2]),
             labelUnit = resolveUnit(tokens[4]);
         int minorUnitCount = int.Parse(tokens[1]), majorUnitCount = int.Parse(tokens[3]),
             labelUnitCount = int.Parse(tokens[5]);
         int labelSpan = int.Parse(tokens[6]);
         String fmt = tokens[7];
         gdef.setTimeAxis(minorUnit, minorUnitCount, majorUnit, majorUnitCount,
                          labelUnit, labelUnitCount, labelSpan, fmt);
      }

      private int resolveUnit(String unitName)
      {
         string[] unitNames = { "SECOND", "MINUTE", "HOUR", "DAY", "WEEK", "MONTH", "YEAR" };
         // ToDo: Check constants
         int[] units = { 0, 1, 2, 3, 4, 5, 6 };
         for (int i = 0; i < unitNames.Length; i++)
         {
            if (unitName.Equals(unitNames[i],StringComparison.OrdinalIgnoreCase))
               return units[i];
         }
         throw new ArgumentException("Unknown time unit specified: " + unitName);
      }

   }
}