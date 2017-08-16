using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.ServiceProcess;
using System.Text;
using RrdDbServer;

namespace RrdbFileService
{
   public partial class RrdbFileService : ServiceBase
   {
      private TcpChannel channel = null;
      private log4net.ILog log = null;
      public RrdbFileService()
      {
         InitializeComponent();
      }

      protected override void OnStart(string[] args)
      {
         string baseDir = AppDomain.CurrentDomain.BaseDirectory;
         log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo(baseDir + "log4net.xml"));
         log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
         try
         {
            // Pick one
            //RemotingConfiguration.CustomErrorsEnabled(false);
            RemotingConfiguration.CustomErrorsMode = CustomErrorsModes.Off;
            //RemotingConfiguration.Configure(baseDir + "RrdbFileService.exe.config");

            log.Info("RrdDb file server started");

            var nameValueCollection = (NameValueCollection)ConfigurationManager.GetSection("rrdbfileserver");
            log.InfoFormat("Database file base path:{0}", nameValueCollection["databasepath"]);
            string port = nameValueCollection["port"];
            int portNumber = int.Parse(port);

            channel = new TcpChannel(portNumber);
            ChannelServices.RegisterChannel(channel, false);
            log.InfoFormat("Server object registerd on port {0}", portNumber);
            RemotingConfiguration.RegisterWellKnownServiceType(
               typeof(RrdDbAdapter),
               "GetRrdDbAdapter",
               WellKnownObjectMode.Singleton);
            log.Info("Service up and running");
         }
         catch (Exception ex)
         {
            log.Error(ex);
            throw;
         }
      }

      protected override void OnStop()
      {
         if (channel != null)
            ChannelServices.UnregisterChannel(channel);
         log.Info("Service stopped");
      }
   }
}
