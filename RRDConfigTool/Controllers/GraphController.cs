using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using RRDConfigTool.Data;
using rrd4n.DataAccess.Interface;
using rrd4n.Graph;

namespace RRDConfigTool.Controllers
{
   public class GraphController
   {
      private GraphForm view = null;
      private GraphLeftForm leftView = null;
      public RrdDbAccessInterface dbAccess { get; set; }
      public  DockingMainForm DockingForm { get; set; }

      public GraphController(GraphForm view, GraphLeftForm leftView)
      {
         this.view = view;
         this.leftView = leftView;
         view.RegisterController(this);
         leftView.RegisterController(this);
      }

      public bool Run()
      {
         DockingForm.AddDockingPanel(view);
         leftView.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockLeft;
         DockingForm.AddDockingPanel(leftView);
         view.EndTime = DateTime.Now;
         view.StartTime = new DateTime(2009, 10, 21, 0, 0, 0);
         return true;
      }

      public void ShowGraph(RrdGraphDef graphDef)
      {
         RrdGraph graph_1 = new RrdGraph(graphDef, dbAccess);
         RrdGraphInfo info = graph_1.getRrdGraphInfo();
         if (info.getByteCount() == 0)
            return;
         MemoryStream ms = new MemoryStream(info.getBytes());
         view.SetImge(ms);
      }

      public void SaveGraph(string graphPath, string graphDefinition)
      {
         StreamWriter sw = new StreamWriter(graphPath,false);    
         sw.Write(graphDefinition);
         leftView.SetFilepath(graphPath);
         sw.Close();
      }
      public void LoadGraph(string graphPath)
      {
         StreamReader sr = new StreamReader(graphPath);
         string  graphDefinition = sr.ReadToEnd();
         view.SetGraphDefinition(graphDefinition);
         view.SetTitle(Path.GetFileNameWithoutExtension(graphPath));
      }
   }
}
