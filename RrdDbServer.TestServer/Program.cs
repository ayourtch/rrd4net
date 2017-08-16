using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace RrdDbServer.TestServer
{
   class Program
   {
      static void Main(string[] args)
      {
         log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo("log4net.xml"));
         log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
         log.Info("RrdDb file server started");
         var nameValueCollection = (NameValueCollection)ConfigurationManager.GetSection("rrdbfileserver");
         string port = nameValueCollection["port"];
         int portNumber = int.Parse(port);

         TcpChannel channel = new TcpChannel(portNumber);
         ChannelServices.RegisterChannel(channel, false);

         RemotingConfiguration.RegisterWellKnownServiceType(
            typeof(RrdDbAdapter),
            "GetRrdDbAdapter",
            WellKnownObjectMode.Singleton);

         Console.ReadLine();
         log.Info("RrdDb file server closed");
      }
   }
}
