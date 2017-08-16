using System;
using System.Windows.Forms;
using Castle.Core.Resource;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;

using RRDConfigTool.Controllers;

namespace RRDConfigTool
{
   static class Program
   {
      /// <summary>
      /// The main entry point for the application.
      /// </summary>
      [STAThread]
      static void Main()
      {
         Application.EnableVisualStyles();
         Application.SetCompatibleTextRenderingDefault(false);

         IWindsorContainer container = new WindsorContainer(new XmlInterpreter(new ConfigResource("castle")));
         DockingMainForm frm = (DockingMainForm)container[typeof(DockingMainForm)];
         Application.Run(frm);
         container.Release(frm);
      }
   }
}
