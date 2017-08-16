using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Text;
using System.IO;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using rrd4n.DataAccess.Interface;
using rrd4n.DataAccess.ServerFile;
using rrd4n.Graph;

namespace LoggerWeb
{
   /// <summary>
   /// Summary description for $codebehindclassname$
   /// </summary>
   [WebService(Namespace = "http://tempuri.org/")]
   [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
   public class GraphHandler : IHttpHandler
   {
      private RrdDbAccessInterface rrdDbAccessInterface;

      public void ProcessRequest(HttpContext context)
      {
         try
         {
            var nameValueCollection = (NameValueCollection)ConfigurationManager.GetSection("rrdbfileserver");
            string url = nameValueCollection["url"];

            rrdDbAccessInterface = new ServerAccessor(url);//"tcp://server:8100/GetRrdDbAdapter");
            //string channelName = context.Request.QueryString["c"];
            DateTime start = new DateTime(2005, 12, 19);
            DateTime end = new DateTime(2006, 12, 12);
            //\Users\miknil\Documents\Visual Studio 2008\Projects\rrd4n\RRDConfigTool\
            string databaseName = "car_day.rra";
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("- --start \"{0}\" --end \"{1}\"", start.ToShortDateString(), end.ToShortDateString());
            sb.Append(" --imgformat PNG");
            sb.AppendFormat(" DEF:myruntime=\"{0}\":milage:AVERAGE", databaseName);
            sb.Append(" CDEF:mil=myruntime,86400,* LINE2:mil#FF0000 -w 800 -h 400 CDEF:km=myruntime,1000,*");
            sb.Append(" SDEF:value_sum=km,TOTAL  GPRINT:myruntime:TOTAL:\"usage {0}\"");
            GraphParser parser = new GraphParser(sb.ToString());
            RrdGraphDef graphDef = parser.CreateGraphDef();

            RrdGraph graph_1 = new RrdGraph(graphDef, rrdDbAccessInterface);
            RrdGraphInfo info = graph_1.getRrdGraphInfo();
            MemoryStream ms = new MemoryStream(info.getBytes());

            context.Response.ContentType = "image/png";
            context.Response.BinaryWrite(ms.ToArray());
         }
         catch (Exception ex)
         {
            context.Response.ContentType = "text/plain";
            context.Response.Write(ex.Message);
         }
      }

      public bool IsReusable
      {
         get
         {
            return false;
         }
      }
   }
}
