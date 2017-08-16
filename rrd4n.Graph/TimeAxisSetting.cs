using System;
using System.Collections.Generic;
using System.Text;

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

   class TimeAxisSetting
   {
      internal readonly long secPerPix;
      internal readonly int minorUnit;
      internal readonly int minorUnitCount;
      internal readonly int majorUnit;
      internal readonly int majorUnitCount;
      internal readonly int labelUnit;
      internal readonly int labelUnitCount;
      internal readonly int labelSpan;
      internal readonly String format;

      public TimeAxisSetting(long secPerPix, int minorUnit, int minorUnitCount, int majorUnit, int majorUnitCount,
                  int labelUnit, int labelUnitCount, int labelSpan, String format)
      {
         this.secPerPix = secPerPix;
         this.minorUnit = minorUnit;
         this.minorUnitCount = minorUnitCount;
         this.majorUnit = majorUnit;
         this.majorUnitCount = majorUnitCount;
         this.labelUnit = labelUnit;
         this.labelUnitCount = labelUnitCount;
         this.labelSpan = labelSpan;
         this.format = format;
      }

      public TimeAxisSetting(TimeAxisSetting s)
      {
         this.secPerPix = s.secPerPix;
         this.minorUnit = s.minorUnit;
         this.minorUnitCount = s.minorUnitCount;
         this.majorUnit = s.majorUnit;
         this.majorUnitCount = s.majorUnitCount;
         this.labelUnit = s.labelUnit;
         this.labelUnitCount = s.labelUnitCount;
         this.labelSpan = s.labelSpan;
         this.format = s.format;
      }

      public TimeAxisSetting(int minorUnit, int minorUnitCount, int majorUnit, int majorUnitCount,
                  int labelUnit, int labelUnitCount, int labelSpan, String format)
         : this(0, minorUnit, minorUnitCount, majorUnit, majorUnitCount,
         labelUnit, labelUnitCount, labelSpan, format)
      {
      }

   }
}
