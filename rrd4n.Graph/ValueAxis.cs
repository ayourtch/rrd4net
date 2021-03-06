﻿using System;
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

   class ValueAxis : RrdGraphConstants
   {
      private static readonly YLab[] ylab = {
		new YLab(0.1, 1, 2, 5, 10),
		new YLab(0.2, 1, 5, 10, 20),
		new YLab(0.5, 1, 2, 4, 10),
		new YLab(1.0, 1, 2, 5, 10),
		new YLab(2.0, 1, 5, 10, 20),
		new YLab(5.0, 1, 2, 4, 10),
		new YLab(10.0, 1, 2, 5, 10),
		new YLab(20.0, 1, 5, 10, 20),
		new YLab(50.0, 1, 2, 4, 10),
		new YLab(100.0, 1, 2, 5, 10),
		new YLab(200.0, 1, 5, 10, 20),
		new YLab(500.0, 1, 2, 4, 10),
		new YLab(0.0, 0, 0, 0, 0)
	};

      private RrdGraph rrdGraph;
      private ImageParameters im;
      private ImageWorker worker;
      private RrdGraphDef gdef;

      internal ValueAxis(RrdGraph rrdGraph)
      {
         this.rrdGraph = rrdGraph;
         this.im = rrdGraph.im;
         this.gdef = rrdGraph.gdef;
         this.worker = rrdGraph.worker;
      }

      internal bool draw() {
		Font font = gdef.smallFont;
		Color gridColor = gdef.colors[COLOR_GRID];
		Color mGridColor = gdef.colors[COLOR_MGRID];
		Color fontColor = gdef.colors[COLOR_FONT];
		int fontHeight = (int) Math.Ceiling(rrdGraph.getSmallFontHeight());
        int labelOffset = (int)(worker.getFontHeight(font) / 2);
        labelOffset += (int)(worker.getFontAscent(font) / 2);
        int labfact = 2, gridind = -1;
		double range = im.maxval - im.minval;
		double scaledrange = range / im.magfact;
		double gridstep;
		if (Double.IsNaN(scaledrange)) {
			return false;
		}
		int pixel = 1;
		String labfmt = null;
		if (Double.IsNaN(im.ygridstep)) {
			if (gdef.altYGrid) {
				/* find the value with max number of digits. Get number of digits */
			   int decimals = (int) Math.Ceiling(Math.Log10(Math.Max(Math.Abs(im.maxval),
			                                                         Math.Abs(im.minval))));
				if (decimals <= 0) /* everything is small. make place for zero */ {
					decimals = 1;
				}
				int fractionals = (int) Math.Floor(Math.Log10(range));
				if (fractionals < 0) /* small amplitude. */ {
					labfmt = string.Format("[0].[1]", decimals - fractionals + 1, -fractionals + 1);
				}
				else {
					labfmt = string.Format("[0]", decimals + 1);
				}
				gridstep = Math.Pow(10, fractionals);
				if (gridstep == 0) /* range is one -> 0.1 is reasonable scale */ {
					gridstep = 0.1;
				}
				/* should have at least 5 lines but no more then 15 */
				if (range / gridstep < 5) {
					gridstep /= 10;
				}
				if (range / gridstep > 15) {
					gridstep *= 10;
				}
				if (range / gridstep > 5) {
					labfact = 1;
					if (range / gridstep > 8) {
						labfact = 2;
					}
				}
				else {
					gridstep /= 5;
					labfact = 5;
				}
			}
			else {
				for (int i = 0; ylab[i].grid > 0; i++) {
					pixel = (int) (im.ysize / (scaledrange / ylab[i].grid));
					if (gridind == -1 && pixel > 5) {
						gridind = i;
						break;
					}
				}
				for (int i = 0; i < 4; i++) {
					if (pixel * ylab[gridind].lfac[i] >= 2 * fontHeight) {
						labfact = ylab[gridind].lfac[i];
						break;
					}
				}
				gridstep = ylab[gridind].grid * im.magfact;
			}
		}
		else {
			gridstep = im.ygridstep;
			labfact = im.ylabfact;
		}
		int x0 = im.xorigin, x1 = x0 + im.xsize;
		int sgrid = (int) (im.minval / gridstep - 1);
		int egrid = (int) (im.maxval / gridstep + 1);
		double scaledstep = gridstep / im.magfact;
		for (int i = sgrid; i <= egrid; i++) {
			int y = rrdGraph.mapper.ytr(gridstep * i);
			if (y >= im.yorigin - im.ysize && y <= im.yorigin) {
				if (i % labfact == 0) {
					String graph_label;
					if (i == 0 || im.symbol == ' ') {
						if (scaledstep < 1) {
							if (i != 0 && gdef.altYGrid) {
								graph_label = string.Format(labfmt, scaledstep * i);
							}
							else {
								graph_label = string.Format("{0,4:F1}", scaledstep * i);
							}
						}
						else {
							graph_label = string.Format("{0,4:F0}" , scaledstep * i);
						}
					}
					else {
						if (scaledstep < 1) {
							graph_label = string.Format("{0,4:F1} {1}", scaledstep * i, im.symbol);
						}
						else {
							graph_label  = string.Format("{0,4:F0} {1}", scaledstep * i, im.symbol);
						}
					}
                    int length = (int)(worker.getStringWidth(graph_label, font));
					worker.drawString(graph_label, x0 - length - PADDING_VLABEL, y - labelOffset, font, fontColor);
					worker.drawLine(x0 - 2, y, x0 + 2, y, mGridColor, TICK_STROKE);
					worker.drawLine(x1 - 2, y, x1 + 2, y, mGridColor, TICK_STROKE);
					worker.drawLine(x0, y, x1, y, mGridColor, GRID_STROKE);
				}
				else if (!(gdef.noMinorGrid)) {
					worker.drawLine(x0 - 1, y, x0 + 1, y, gridColor, TICK_STROKE);
					worker.drawLine(x1 - 1, y, x1 + 1, y, gridColor, TICK_STROKE);
					worker.drawLine(x0, y, x1, y, gridColor, GRID_STROKE);
				}
			}
		}
		return true;
	}

      internal class YLab
      {
         internal double grid;
         internal int[] lfac;

         public YLab(double grid, int lfac1, int lfac2, int lfac3, int lfac4)
         {
            this.grid = grid;
            lfac = new int[] { lfac1, lfac2, lfac3, lfac4 };
         }
      }
   }
}
