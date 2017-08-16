using System;
using System.Collections.Generic;
using System.Drawing;
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

   class ValueAxisLogarithmic : RrdGraphConstants
   {
      private static double[][] yloglab = new double[7][];
      static ValueAxisLogarithmic()
      {
         yloglab[0] = new double[] { 1e9, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
         yloglab[1] = new double[] { 1e3, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
         yloglab[2] = new double[] { 1e1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
         /* {  1e1, 1,  5,  0,  0,  0,  0,  0,  0,  0,  0,  0 }, */
         yloglab[3] = new double[] { 1e1, 1, 2.5, 5, 7.5, 0, 0, 0, 0, 0, 0, 0 };
         yloglab[4] = new double[] { 1e1, 1, 2, 4, 6, 8, 0, 0, 0, 0, 0, 0 };
         yloglab[5] = new double[] { 1e1, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 0 };
         yloglab[6] = new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
      }

      private RrdGraph rrdGraph;
      private ImageParameters im;
      private ImageWorker worker;
      private RrdGraphDef gdef;

      internal ValueAxisLogarithmic(RrdGraph rrdGraph)
      {
         this.rrdGraph = rrdGraph;
         this.im = rrdGraph.im;
         this.gdef = rrdGraph.gdef;
         this.worker = rrdGraph.worker;
      }

      internal bool draw()
      {
         Font font = gdef.smallFont;
         Color gridColor = gdef.colors[COLOR_GRID];
         Color mGridColor = gdef.colors[COLOR_MGRID];
         Color fontColor = gdef.colors[COLOR_FONT];
         int fontHeight = (int)Math.Ceiling(rrdGraph.getSmallFontHeight());
         int labelOffset = (int)(worker.getFontAscent(font) / 2);

         double pixpex = (double)im.ysize / (Math.Log10(im.maxval) - Math.Log10(im.minval));
         if (Double.IsNaN(pixpex))
         {
            return false;
         }
         double minstep, pixperstep;
         int minoridx = 0, majoridx = 0;
         for (int i = 0; yloglab[i][0] > 0; i++)
         {
            minstep = Math.Log10(yloglab[i][0]);
            for (int ii = 1; yloglab[i][ii + 1] > 0; ii++)
            {
               if (yloglab[i][ii + 2] == 0)
               {
                  minstep = Math.Log10(yloglab[i][ii + 1]) - Math.Log10(yloglab[i][ii]);
                  break;
               }
            }
            pixperstep = pixpex * minstep;
            if (pixperstep > 5)
            {
               minoridx = i;
            }
            if (pixperstep > 2 * fontHeight)
            {
               majoridx = i;
            }
         }
         int x0 = im.xorigin, x1 = x0 + im.xsize;
         for (double value = Math.Pow(10, Math.Log10(im.minval)
               - Math.Log10(im.minval) % Math.Log10(yloglab[minoridx][0]));
             value <= im.maxval;
             value *= yloglab[minoridx][0])
         {
            if (value < im.minval) continue;
            int i = 0;
            while (yloglab[minoridx][++i] > 0)
            {
               int y = rrdGraph.mapper.ytr(value * yloglab[minoridx][i]);
               if (y <= im.yorigin - im.ysize)
               {
                  break;
               }
               worker.drawLine(x0 - 1, y, x0 + 1, y, gridColor, TICK_STROKE);
               worker.drawLine(x1 - 1, y, x1 + 1, y, gridColor, TICK_STROKE);
               worker.drawLine(x0, y, x1, y, gridColor, GRID_STROKE);
            }
         }
         for (double value = Math.Pow(10, Math.Log10(im.minval)
               - (Math.Log10(im.minval) % Math.Log10(yloglab[majoridx][0])));
             value <= im.maxval;
             value *= yloglab[majoridx][0])
         {
            if (value < im.minval)
            {
               continue;
            }
            int i = 0;
            while (yloglab[majoridx][++i] > 0)
            {
               int y = rrdGraph.mapper.ytr(value * yloglab[majoridx][i]);
               if (y <= im.yorigin - im.ysize)
               {
                  break;
               }
               worker.drawLine(x0 - 2, y, x0 + 2, y, mGridColor, TICK_STROKE);
               worker.drawLine(x1 - 2, y, x1 + 2, y, mGridColor, TICK_STROKE);
               worker.drawLine(x0, y, x1, y, mGridColor, GRID_STROKE);
               String graph_label = string.Format("{0,3:E0}", value * yloglab[majoridx][i]);
               int length = (int)(worker.getStringWidth(graph_label, font));
               worker.drawString(graph_label, x0 - length - PADDING_VLABEL, y + labelOffset, font, fontColor);
            }
         }
         return true;
      }
   }
}
