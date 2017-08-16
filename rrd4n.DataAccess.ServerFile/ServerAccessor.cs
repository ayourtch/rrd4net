using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using rrd4n.DataAccess.Interface;
using rrd4n.ServerAccess.Interface;
using rrd4n.ServerAccess.Data;

namespace rrd4n.DataAccess.ServerFile
{
   public class ServerAccessor : RrdDbAccessInterface
   {
      TcpChannel channel = null;
      private string fileServerUrl;

      public ServerAccessor(string fileServerUrl)
      {
         this.fileServerUrl = fileServerUrl;
         IChannel[] channels = ChannelServices.RegisteredChannels;
         bool registred = false;
         foreach (var ch in channels)
         {
            if (ch.ChannelName == "tcp")
            {
               registred = true;
               break;
            }
         }
         if (!registred)
         {
            channel = new TcpChannel();
            ChannelServices.RegisterChannel(channel, false);
         }
      }
      ~ServerAccessor()
      {
         if (channel != null)
            ChannelServices.UnregisterChannel(channel);
      }

      private RrdServerInterface GetServerAccessor()
      {
         return (RrdServerInterface) Activator.GetObject(typeof(RrdServerInterface),fileServerUrl);
      }

      #region RrdDbAccessInterface Members

      public rrd4n.DataAccess.Data.FetchData GetData(rrd4n.DataAccess.Data.FetchRequest request)
      {
         RrdServerInterface remoteAccessor = GetServerAccessor();

         RemoteFetchData remoteData = remoteAccessor.FetchData(request.DatabasePath, request.FetchStart, request.FetchEnd,
            request.ConsolidateFunctionName, request.Resolution);

         rrd4n.DataAccess.Data.FetchData localData = new rrd4n.DataAccess.Data.FetchData(remoteData.ArchiveSteps,
            remoteData.ArchiveEndTimeTicks, remoteData.DatasourceNames);
         localData.Timestamps = remoteData.Timestamps;
         localData.Values = remoteData.Values;
         return localData;
      }

      public void StoreData(rrd4n.DataAccess.Data.Sample sample)
      {
         RrdServerInterface remoteAccessor = GetServerAccessor();
         remoteAccessor.StoreData(sample.DatabasePath, sample.getTime(), sample.getValues());
      }

      #endregion
   }
}
