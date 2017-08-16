using System;
using System.Collections.Generic;
using System.Text;
using rrd4n;
using rrd4n.Common;
using rrd4n.Core;

namespace rrd4n.Parser
{
   public class RrdDbParser : CommandParser
   {

      public RrdDbParser(string command)
      {
         TokenizeCommand(command);
      }

      public RrdDef CreateDatabaseDef()
      {
         long start;
         DateTime startDateTime;
         string startOption = getOptionValue("b", "start", DEFAULT_START);

         try
         {
            if (DateTime.TryParse(startOption, out startDateTime))
               start = Util.getTimestamp(startDateTime);
            else
               start = long.Parse(startOption);
         }
         catch (FormatException ex)
         {
            throw new ApplicationException("Bad date format:[" + startOption + "]." + ex.Message);
         }

         String stepOption = getOptionValue("s", "step", DEFAULT_STEP);
         long step = long.Parse(stepOption);

         String[] words = getRemainingWords();
         if (words.Length < 3) throw new ArgumentException("To few arguments! Use: create name DS:name:heartbeat:min:max [RRAdef]");
         if (words[0] != "create") throw new ArgumentException("Wrong command format! Use: create name DS:name:heartbeat:min:max [RRAdef]");
         RrdDef rrdDef = new RrdDef(words[1], start, step);

         for (int i = 2; i < words.Length; i++)
         {
            if (words[i].StartsWith("DS:"))
               rrdDef.addDatasource(parseDef(words[i]));
            else if (words[i].StartsWith("RRA:"))
               rrdDef.addArchive(parseRra(words[i]));
            else
               throw new ArgumentException("Invalid rrdcreate syntax. Not a DSDef or RRADef  " + words[i] + "\nUse: create name DS:name:heartbeat:min:max [RRAdef]");
         }
         if (rrdDef.getDsCount() == 0)
            throw new ArgumentException("No a Data source defined.\nUse: create name DS:name:heartbeat:min:max [RRAdef]");

         return rrdDef;
      }

      public DsDef parseDef(String word)
      {
         // DEF:name:type:heratbeat:min:max
         String[] tokens = word.Split(':');
         if (tokens.Length < 6) throw new ArgumentException("Invalid DS definition: " + word);

         String dsName = tokens[1];
         DsType dsType = new DsType(tokens[2]);

         long heartbeat;
         TimeSpan heartbeatSpan;
         if (!long.TryParse(tokens[3], out heartbeat))
         {
            heartbeatSpan = TimeSpan.Parse(tokens[3]);
            heartbeat = (long)heartbeatSpan.TotalSeconds;
         }

         double min;
         if (!double.TryParse(tokens[4], out min))
            min = double.NaN;

         double max;
         if (!double.TryParse(tokens[5], out max))
            max = double.NaN;

         return new DsDef(dsName, dsType, heartbeat, min, max);
      }

      public ArcDef parseRra(String word)
      {
         // RRA:cfun:xff:steps:rows
         String[] tokens = word.Split(':');
         if (tokens.Length < 5) throw new ArgumentException("Invalid RRA definition: " + word);

         ConsolFun cf = new ConsolFun(tokens[1]);
         double xff;
         if (!double.TryParse(tokens[2], out xff))
            xff = double.NaN;
         int steps = int.Parse(tokens[3]);
         int rows = int.Parse(tokens[4]);

         return new ArcDef(cf, xff, steps, rows);
      }
   }
}
