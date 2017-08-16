using System;
using System.Text.RegularExpressions;
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

   class PrintText : CommentText
   {
      static String UNIT_MARKER = "([^%]?)%(s|S)";

      public string SrcName { get; private set; }
      private bool includedInGraph;
      private bool strftime;

      public PrintText(String srcName, String text, bool includedInGraph, bool strftime)
         : base(text)
      {
         this.SrcName = srcName;
         this.includedInGraph = includedInGraph;
         this.strftime = strftime;
      }

      override public bool isPrint()
      {
         return !includedInGraph;
      }

      override public void resolveText(DataProcessor dproc, ValueScaler valueScaler)
      {
         base.resolveText(dproc, valueScaler);
         if (resolvedText != null)
         {
            if (strftime)
            {
               long timeStamp = dproc.getTimestamps(SrcName)[0];
               DateTime dt = Util.ConvertToDateTime(timeStamp);
               resolvedText = string.Format("{0:yyyy}",dt);//dt.ToString();
            }
            else
            {
               // Don't support old version GPRINT:vname:CF:format (deprecated)
               double value = dproc.getValues(SrcName)[0];

               Regex regex = new Regex(UNIT_MARKER);
               Match matcher = regex.Match(resolvedText);
               //   // ToDo: chek this up!
               //   if (matcher.Groups.Count > 0)
               //   {
               //      // unit specified
               //      Scaled scaled = valueScaler.scale(value, matcher.Groups[2].Value.CompareTo("s") == 0);
               //      resolvedText = resolvedText.Substring(0, matcher.Index) +
               //            matcher.Groups[1] + scaled.unit + resolvedText.Substring(matcher.Index + matcher.Length);
               //      value = scaled.value;
               //   }
               resolvedText = Util.sprintf(resolvedText, new object[] { value });
            }

            trimIfGlue();
         }
      }
   }
}
