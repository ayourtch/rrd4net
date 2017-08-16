using System;
using System.Collections.Generic;
using System.Text;
using rrd4n.Common;
using rrd4n.Data;
namespace rrd4n.Graph
{
   /* ============================================================
    * Rrd4n : Pure c# implementation of RRDTool's functionality
    * ============================================================
    *
    * Project Info:  http://minidev.se
    * Project Lead:  Mikael Nilsson (info@minidev.se)
    *
    * Developers:    Mikael Nilsson
    *
    *
    * (C) Copyright 2009-2010, by Mikael Nilsson.
    *
    * This library is free software; you can redistribute it and/or modify it under the terms
    * of the GNU Lesser General Public License as published by the Free Software Foundation;
    * either version 2.1 of the License, or (at your option) any later version.
    *
    * This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
    * without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
    * See the GNU Lesser General Public License for more details.
    *
    * You should have received a copy of the GNU Lesser General Public License along with this
    * library; if not, write to the Free Software Foundation, Inc., 59 Temple Place, Suite 330,
    * Boston, MA 02111-1307, USA.
    */

   public class Def : Source
   {
      public String rrdPath { get; set; }
      public String dsName { get; set; }
      private readonly String backend;
      private ConsolFun consolFun;
      public long StartTime { get; set; }
      public long EndTime { get; set; }
      public long Step { get; set; }
      public string ReduceName { get; set; }


      public Def(String name, string rrdPath)
         : base(name)
      {
         this.rrdPath = rrdPath;
         Step = long.MinValue;
         StartTime = long.MinValue;
         EndTime = long.MinValue;
         ReduceName = string.Empty;
      }

      public Def(String name, String rrdPath, String dsName, ConsolFun consolFun)
         : this(name, rrdPath, dsName, consolFun, null)
      {
      }

      public Def(String name, String rrdPath, String dsName, ConsolFun consolFun, String backend)
         : base(name)
      {
         this.rrdPath = rrdPath;
         this.dsName = dsName;
         this.consolFun = consolFun;
         this.backend = backend;
         Step = long.MinValue;
         StartTime = long.MinValue;
         EndTime = long.MinValue;
         ReduceName = string.Empty;
      }
      public void SetConsulFunType(string consolFunName)
      {
         consolFun = new ConsolFun(consolFunName);
      }

      override public void requestData(DataProcessor dproc)
      {
         if (backend == null)
         {
            dproc.addDatasource(name, rrdPath, dsName, consolFun,Step,StartTime,EndTime,ReduceName);
         }
         else
         {
            dproc.addDatasource(name, rrdPath, dsName, consolFun, backend);
         }
      }
   }
}
