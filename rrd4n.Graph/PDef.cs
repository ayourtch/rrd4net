using System;
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

   class PDef : Source
   {
      private readonly Plottable plottable;

      public PDef(String name, Plottable plottable)
         : base(name)
      {
         this.plottable = plottable;
      }

      override public void requestData(DataProcessor dproc)
      {
         dproc.addDatasource(name, plottable);
      }
   }
}
