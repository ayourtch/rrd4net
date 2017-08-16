using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using rrd4n.Core;
using rrd4n.Parser;
using RRDConfigTool.Data;

namespace RRDConfigTool.Controllers
{
   public class DataSourceController
   {
      private NewDatasourceForm view = null;
      private Model model = null;
      public DsDef DataSourceDef { get; set; }
      public DatabaseData SrcDatabase { get; set; }
      private const string heartbeatPattern = @"(\d+\.\d{1,2}:\d{1,2}:\d{1,2})|(\dx?)";

      public DataSourceController(NewDatasourceForm view, Model model)
      {
         this.view = view;
         this.model = model;
         view.Controller = this;
      }
      public bool Run()
      {
         view.DataSourceDef = DataSourceDef;
         if (view.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
            return false;
         return true;
      }

      public long DatabaseStep
      {
         get { return model.EditingDatabaseData.Definition.getStep(); }
      }

      public string CreateDef(string minValueText, string maxValueText, string heartbeatText, string dataSourceName, string dataSourceType)
      {
         double minValue = minValueText.CompareTo(double.NaN.ToString()) == 0 ? double.NaN : double.Parse(minValueText);
         double maxValue = maxValueText.CompareTo(double.NaN.ToString()) == 0 ? double.NaN : double.Parse(maxValueText);

         Regex validateHeartbeatRegex = new Regex(heartbeatPattern);
         if (!validateHeartbeatRegex.IsMatch(heartbeatText))
            return "Bad timespan format. Use seconds or d.hh.mm.ss";

         Match match = validateHeartbeatRegex.Match(heartbeatText);
         long heartbeat;
         if (heartbeatText.EndsWith("x"))
         {
            long multiplier = long.Parse(heartbeatText.Substring(0, heartbeatText.Length - 1));
            heartbeat = DatabaseStep * multiplier;
         }
         else
         {
            if (!long.TryParse(heartbeatText, out heartbeat))
            {
               TimeSpan heartbeatTimeSpan;
               if (!TimeSpan.TryParse(heartbeatText, out heartbeatTimeSpan))
                  return "Bad timespan format. Use seconds or d.hh.mm.ss or nnx";
               heartbeat = (long)heartbeatTimeSpan.TotalSeconds;
            }
         }
         // DEF:name:type:heratbeat:min:max
         return string.Format("DEF:{0}:{1}:{2}:{3}:{4}",
            dataSourceName,
            dataSourceType,
            heartbeat,
            minValue.ToString(),
            maxValue.ToString()
            );
      }


      public void AddDataSource(string command)
      {
         RrdDbParser parser = new RrdDbParser("");
         model.AddDataSource(parser.parseDef(command));
      }

      public void UpdateDataSource(string command, DsDef originalDsDef)
      {
         RrdDbParser parser = new RrdDbParser("");
         model.UpdateDataSource(SrcDatabase, parser.parseDef(command), originalDsDef);
      }
   }
}
