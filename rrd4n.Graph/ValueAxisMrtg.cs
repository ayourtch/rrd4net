using System;
using System.Drawing;

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

   class ValueAxisMrtg : RrdGraphConstants
   {
      private ImageParameters im;
      private ImageWorker worker;
      private RrdGraphDef gdef;

      internal ValueAxisMrtg(RrdGraph rrdGraph)
      {
         this.im = rrdGraph.im;
         this.gdef = rrdGraph.gdef;
         this.worker = rrdGraph.worker;
         im.unit = gdef.unit;
      }

      internal bool draw()
      {
         Font font = gdef.smallFont;
         Color mGridColor = gdef.colors[COLOR_MGRID];
         Color fontColor = gdef.colors[COLOR_FONT];
         int labelOffset = (int)(worker.getFontAscent(font) / 2);

         if (Double.IsNaN((im.maxval - im.minval) / im.magfact))
         {
            return false;
         }

         int xLeft = im.xorigin;
         int xRight = im.xorigin + im.xsize;
         String labfmt;
         if (im.scaledstep / im.magfact * Math.Max(Math.Abs(im.quadrant), Math.Abs(4 - im.quadrant)) <= 1.0)
         {
            labfmt = "{0,5:f2}";
         }
         else
         {
            labfmt = string.Format("{0,4:f}", 1 - ((im.scaledstep / im.magfact > 10.0 || Math.Ceiling(im.scaledstep / im.magfact) == im.scaledstep / im.magfact) ? 1 : 0));
         }
         if (im.symbol != ' ' || im.unit != null)
         {
            labfmt += " ";
         }
         if (im.symbol != ' ')
         {
            labfmt += im.symbol;
         }
         if (im.unit != null)
         {
            labfmt += im.unit;
         }
         for (int i = 0; i <= 4; i++)
         {
            int y = im.yorigin - im.ysize * i / 4;
            if (y >= im.yorigin - im.ysize && y <= im.yorigin)
            {
               String graph_label = string.Format(labfmt, im.scaledstep / im.magfact * (i - im.quadrant));
               int length = (int)(worker.getStringWidth(graph_label, font));
               worker.drawString(graph_label, xLeft - length - PADDING_VLABEL, y + labelOffset, font, fontColor);
               worker.drawLine(xLeft - 2, y, xLeft + 2, y, mGridColor, TICK_STROKE);
               worker.drawLine(xRight - 2, y, xRight + 2, y, mGridColor, TICK_STROKE);
               worker.drawLine(xLeft, y, xRight, y, mGridColor, GRID_STROKE);
            }
         }
         return true;
      }

   }
}
