/* ============================================================
 * Rrd4n : Pure c# implementation of RRDTool's functionality
 * ============================================================
 *
 * Project Info:  http://minidev.se
 * Project Lead:  Mikael Nilsson (info@minidev.se)
 *
 * (C) Copyright 2009-2010, by Mikael Nilsson.
 *
 * This library is free software; you can redistribute it and/or modify it under the terms
 * of the GNU Lesser General Public License as published by the Free Software Foundation;
 * either version 2.1 of the License, or (at your option) any later version.
 *
 * Developers:    Mikael Nilsson
 *
 *
 * This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
 * without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
 * See the GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License along with this
 * library; if not, write to the Free Software Foundation, Inc., 59 Temple Place, Suite 330,
 * Boston, MA 02111-1307, USA.
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace rrd4n.Common
{
   public class DsType
   {
      public enum DsTypes
      {
         GAUGE, COUNTER, DERIVE, ABSOLUTE
      };

      private DsTypes dsType;
      public DsTypes Dt
      { get { return dsType; } }

      public DsType(DsTypes dsType)
      {
         this.dsType = dsType;
      }
      public DsType(string dsTypeName)
         :this (DsType.ValueOf(dsTypeName))
      {
      }

      public string Name
      {
         get
         {
            switch (dsType)
            {
               case DsTypes.ABSOLUTE:
                  return "ABSOLUTE";
               case DsTypes.COUNTER:
                  return "COUNTER";
               case DsTypes.DERIVE:
                  return "DERIVE";
               case DsTypes.GAUGE:
                  return "GAUGE";
               default:
                  throw new ApplicationException("Invalid data source type");
            }
         }
      }

      public override string ToString()
      {
         return Name;
      }

      public static DsTypes ValueOf(string typeName)
      {
         switch (typeName)
         {
            case "ABSOLUTE":
               return DsTypes.ABSOLUTE;
            case "COUNTER":
               return DsTypes.COUNTER;
            case "DERIVE":
               return DsTypes.DERIVE;
            case "GAUGE":
               return DsTypes.GAUGE;
            default:
               throw new ApplicationException("Invalid data source type name");
         }

      }

   }
}