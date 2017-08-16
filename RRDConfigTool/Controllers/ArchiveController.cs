using System;
using System.Collections.Generic;
using System.Text;
using rrd4n.Core;
using rrd4n.Parser;
using RRDConfigTool.Data;
namespace RRDConfigTool.Controllers
{
   public class ArchiveController
   {
      private NewArchiveForm view = null;
      private Model model = null;
      public ArcDef ArchiveDef { get; set; }
      public DatabaseData SrcDatabaseData { get; set; }

      private const string timespanPattern = @"(\d+\.\d{1,2}:\d{1,2}:\d{1,2})|(\dx?)";

      public ArchiveController(NewArchiveForm view, Model model)
      {
         this.view = view;
         this.model = model;
         view.Controller = this;
      }

      public bool Run()
      {
         view.ArchiveDef = ArchiveDef;
         if (view.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
            return false;
         return true;
      }

      public TimeSpan DatabaseSteps
      {
         get { return model.EditingDatabaseData.Definition.getStepTimeSpan(); }
      }

      public string CreateDef(string archiveType, string xxfText, string stepsText, string rowsText)
      {
         long steps;
         long rows;
         double xxf;


         if (!long.TryParse(stepsText, out steps))
         {
            TimeSpan stepSpan;
            if (!TimeSpan.TryParse(stepsText, out stepSpan))
               return "Bad step format";
            steps = (long)((stepSpan.TotalSeconds + 0.5) / DatabaseSteps.TotalSeconds);
         }
         xxf = double.Parse(xxfText);

         if (!long.TryParse(rowsText, out rows))
         {
            TimeSpan rowSpan;
            if (!TimeSpan.TryParse(rowsText, out rowSpan))
               return "Bad row format";

            long stepTime = steps * (long)(DatabaseSteps.TotalSeconds + 0.5);
            rows = (long)((rowSpan.TotalSeconds + 0.5) / stepTime);
         }
         return string.Format("RRA:{0}:{1}:{2}:{3}",
                               archiveType,
                               xxf.ToString(),
                               steps.ToString(),
                               rows.ToString()
                               );

      }

      public void AddArchive(string command)
      {
         RrdDbParser parser = new RrdDbParser("");
         ArcDef arcDef = parser.parseRra(command);
         if (model.EditingDatabaseData.Definition.ContainsArchive(arcDef))
            throw new ArgumentException("Archive already exist");
         model.AddArchive(arcDef);
      }

      public void UpdateArchive(string command)
      {
         RrdDbParser parser = new RrdDbParser("");
         ArcDef arcDef = parser.parseRra(command);
         if (!model.EditingDatabaseData.Definition.ContainsArchive(arcDef))
            throw new ArgumentException("Archive don't exist");
         model.UpdateArchive(arcDef);
      }
   }
}
