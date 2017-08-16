using System;
using System.Collections.Generic;
using System.Text;
using rrd4n.Parser;
using rrd4n.Core;
using RRDConfigTool.Data;

namespace RRDConfigTool.Controllers
{
   public class CreateDatabaseController
   {
      private CreateDatabaseForm view = null;
      private Model model = null;

      public CreateDatabaseController(CreateDatabaseForm view, Model model)
      {
         this.view = view;
         this.model = model;
         view.Controller = this;
      }
      public bool Run()
      {
         if (view.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
            return false;
         return true;
      }

      public void SetDatabaseDefinition(string command)
      {
         RrdDbParser parser = new RrdDbParser(command);
         RrdDef rrdDef = parser.CreateDatabaseDef();
         if (model.DatabaseExist(rrdDef.Path))
            throw new ApplicationException("Datbase " + rrdDef.Path + " already exist!");
         model.CreateDatabase(rrdDef);
      }
   }
}
