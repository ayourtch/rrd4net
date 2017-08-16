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
   /**
    * Class to represent successfully created Rrd4n graph. Objects of this class are created by method
    * {@link RrdGraph#getRrdGraphInfo()}.
    */
   public class RrdGraphInfo
   {
      internal String filename;
      internal int width;
      internal int height;
      internal byte[] bytes;
      internal String imgInfo;

      private readonly List<String> printLines = new List<String>();

      public RrdGraphInfo()
      {
         // cannot instantiate this class
      }

      public void addPrintLine(String printLine)
      {
         printLines.Add(printLine);
      }

      /**
       * Returns filename of the graph
       * @return filename of the graph. '-' denotes in-memory graph (no file created)
       */
      public String getFilename()
      {
         return filename;
      }

      /**
       * Returns total graph width
       * @return total graph width
       */
      public int getWidth()
      {
         return width;
      }

      /**
       * Returns total graph height
       * @return total graph height
       */
      public int getHeight()
      {
         return height;
      }

      /**
       * Returns graph bytes
       * @return Graph bytes
       */
      public byte[] getBytes()
      {
         return bytes;
      }

      /**
       * Returns PRINT lines requested by {@link RrdGraphDef#print(String, org.Rrd4n.ConsolFun, String)} method.
       * @return An array of formatted PRINT lines
       */
      public String[] getPrintLines()
      {
         return printLines.ToArray();
      }

      /**
       * Returns image information requested by {@link RrdGraphDef#setImageInfo(String)} method
       * @return Image information
       */
      public String getImgInfo()
      {
         return imgInfo;
      }

      /**
       * Returns the number of bytes in the graph file
       * @return Length of the graph file
       */
      public int getByteCount()
      {
         return bytes != null ? bytes.Length : 0;
      }

      /**
       * Dumps complete graph information. Useful for debugging purposes.
       * @return String containing complete graph information
       */
      public String dump()
      {
         StringBuilder b = new StringBuilder();
         b.Append("filename = \"").Append(getFilename()).AppendLine("\"");
         b.Append("width = ").Append(getWidth()).Append(", height = ").Append(getHeight()).AppendLine("");
         b.Append("byteCount = ").Append(getByteCount()).AppendLine();
         b.Append("imginfo = \"").Append(getImgInfo()).AppendLine("\"");
         String[] plines = getPrintLines();
         if (plines.Length == 0)
         {
            b.Append("No print lines found\n");
         }
         else
         {
            for (int i = 0; i < plines.Length; i++)
            {
               b.Append("print[").Append(i).Append("] = \"").Append(plines[i]).AppendLine("\"");
            }
         }
         return b.ToString();
      }
   }
}
