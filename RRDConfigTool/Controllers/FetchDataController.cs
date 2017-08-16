using System;
using System.Collections.Generic;
using System.Text;
using Castle.Core;
using Castle.Core.Logging;
using rrd4n.Common;
using rrd4n.Core;
using rrd4n;
using RRDConfigTool.Data;
using rrd4n.DataAccess.Data;
namespace RRDConfigTool.Controllers
{
   public class FetchDataController
   {

      private FetchDataForm view;
      private Model model;

      private ILogger logger = NullLogger.Instance;

      public ILogger Logger
      {
         get { return logger; }
         set { logger = value; }
      }

      public FetchDataController(FetchDataForm view, Model model)
      {
         this.view = view;
         this.model = model;
         view.Controller = this;
      }

      public void Run()
      {
         view.ShowDialog(view.Parent);
      }

      [Obsolete]
      public void LoadData(string consolFunType, DateTime fromDate, DateTime toDate, TimeSpan resolution)
      {
         throw new NotImplementedException("LoadData");
         //ConsolFun consolFun = new ConsolFun(consolFunType);
         //long res = (int)resolution.TotalSeconds;

         //FetchData data = model.Database.fetchData(new FetchRequest(model.DatabasePath, consolFunType,
         //                                                     fromDate, toDate, res));
         //double[] values = data.getValues(0);
         //DateTime[] timestamps = data.getDateTimestamps();
         //List<FetchedData> fetchedData = new List<FetchedData>();
         //for (var i = 0; i < values.Length; i++)
         //{
         //   fetchedData.Add(new FetchedData{  TimeStamp=timestamps[i], Value=values[i]});
         //}

         //view.SetData(fetchedData);
      }

   }
}
