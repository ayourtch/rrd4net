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
   public class ConsolFun
   {
      public enum ConsolFunTypes
      {
         AVERAGE = 0,
         MIN = 1,
         MAX = 2,
         LAST = 3,
         FIRST = 4,
         TOTAL = 5
      }

      private ConsolFunTypes consolFunType;
      public ConsolFunTypes CSType
      { get { return consolFunType; } }

      public ConsolFun(ConsolFunTypes consolFunType)
      {
         this.consolFunType = consolFunType;
      }

      public ConsolFun(string type)
         :this(ValueOf(type))
      {
      }

      public string Name
      {
         get
         {
            switch (consolFunType)
            {
               case ConsolFunTypes.AVERAGE:
                  return "AVERAGE";
               case ConsolFunTypes.MIN:
                  return "MIN";
               case ConsolFunTypes.MAX:
                  return "MAX";
               case ConsolFunTypes.LAST:
                  return "LAST";
               case ConsolFunTypes.FIRST:
                  return "FIRST";
               case ConsolFunTypes.TOTAL:
                  return "TOTAL";
               default:
                  throw new ApplicationException("Invalid consolidation type");
            }
         }
      }
      public override string ToString()
      {
         return Name;
      }

      public static ConsolFunTypes ValueOf(string typeName)
      {
         switch (typeName)
         {
            case "AVERAGE":
               return ConsolFunTypes.AVERAGE;
            case "MIN":
               return ConsolFunTypes.MIN;
            case "MAX":
               return ConsolFunTypes.MAX;
            case "LAST":
               return ConsolFunTypes.LAST;
            case "FIRST":
               return ConsolFunTypes.FIRST;
            case "TOTAL":
               return ConsolFunTypes.TOTAL;
            default:
               throw new ApplicationException("Invalid consolidation type name");
         }
      }

   }
}