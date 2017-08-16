using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Castle.MicroKernel;

using RRDConfigTool.Controllers;
using WeifenLuo.WinFormsUI.Docking;
namespace RRDConfigTool
{
   public partial class DockingMainForm : Form
   {
            
      private ViewController controller = null;
      private readonly IKernel kernel;
      public DockingMainForm(IKernel kernel)
         : this()
      {
         this.kernel = kernel;
      }

      public DockingMainForm()
      {
         InitializeComponent();
      }
      public void RegisterController(ViewController viewController)
      {
         this.controller = viewController;
      }

      private void DockingMainForm_Load(object sender, EventArgs e)
      {
         //LoggerConfiguration.LoggerTreeForm loggerFrm = new LoggerConfiguration.LoggerTreeForm();
         //loggerFrm.Leave += new EventHandler(loggerFrm_Leave);
         //loggerFrm.LostFocus += new EventHandler(loggerFrm_LostFocus);
         //loggerFrm.MdiChildActivate += new EventHandler(loggerFrm_MdiChildActivate);
         //loggerFrm.Shown += new EventHandler(loggerFrm_Shown);
         //loggerFrm.VisibleChanged += new EventHandler(loggerFrm_VisibleChanged);
         //loggerFrm.Enter += new EventHandler(loggerFrm_Enter);
         //loggerFrm.ShowHint = DockState.DockLeft;
         //loggerFrm.Show(dockPanel1);

         var createDatabaseController = kernel[typeof(CreateDatabaseController)] as CreateDatabaseController;

         ViewController controller = (ViewController)kernel[typeof(ViewController)];

         controller.SetDockingForm(this);
         controller.Run();

         GraphController graphController = kernel[typeof(GraphController)] as GraphController;
         graphController.DockingForm = this;
         graphController.Run();
      }

      void loggerFrm_Enter(object sender, EventArgs e)
      {
      }

      void loggerFrm_VisibleChanged(object sender, EventArgs e)
      {
      }

      void loggerFrm_Shown(object sender, EventArgs e)
      {
      }

      void loggerFrm_MdiChildActivate(object sender, EventArgs e)
      {
         throw new NotImplementedException();
      }

      void loggerFrm_LostFocus(object sender, EventArgs e)
      {
         throw new NotImplementedException();
      }

      void loggerFrm_Leave(object sender, EventArgs e)
      {
      }

      public void AddDockingPanel(DockContent panel)
      {
         panel.Show(dockPanel1);
      }

      private void graphsToolStripMenuItem_Click(object sender, EventArgs e)
      {
         foreach (var doc in this.dockPanel1.Contents)
         {
            if (doc.GetType() == typeof(GraphForm))
            {
               doc.DockHandler.Activate();
               return;
            }
         }

      }

      private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
      {
         AboutBox frm = new AboutBox();
         frm.ShowDialog(this);

      }
   }
}
