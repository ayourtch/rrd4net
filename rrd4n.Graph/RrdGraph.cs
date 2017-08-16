using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using rrd4n.Common;
using rrd4n.Data;
using rrd4n.DataAccess.Interface;

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
    * Class which actually creates Rrd4n graphs (does the hard work).
    */
   public class RrdGraph : RrdGraphConstants
   {
      private static double[] SENSIBLE_VALUES = {
            1000.0, 900.0, 800.0, 750.0, 700.0, 600.0, 500.0, 400.0, 300.0, 250.0, 200.0, 125.0, 100.0,
            90.0, 80.0, 75.0, 70.0, 60.0, 50.0, 40.0, 30.0, 25.0, 20.0, 10.0,
            9.0, 8.0, 7.0, 6.0, 5.0, 4.0, 3.5, 3.0, 2.5, 2.0, 1.8, 1.5, 1.2, 1.0,
            0.8, 0.7, 0.6, 0.5, 0.4, 0.3, 0.2, 0.1, 0.0, -1
    };

      public RrdDbAccessInterface DbAccessInterface { get; set; }

      private static char[] SYMBOLS = { 'a', 'f', 'p', 'n', 'u', 'm', ' ', 'k', 'M', 'G', 'T', 'P', 'E' };

      internal RrdGraphDef gdef;
      public ImageParameters im = new ImageParameters();
      DataProcessor dproc;
      internal ImageWorker worker;
      internal Mapper mapper;
      RrdGraphInfo info = new RrdGraphInfo();


      public RrdGraph( RrdDbAccessInterface rrdDbAccess)
      {
         DbAccessInterface = rrdDbAccess;
      }

      /**
       * Creates graph from the corresponding {@link RrdGraphDef} object.
       *
       * @param gdef Graph definition
       *
       * @ Thrown in case of I/O error
       */
      public RrdGraph(RrdGraphDef gdef, RrdDbAccessInterface rrdDbAccess)
      {
         DbAccessInterface = rrdDbAccess;
         this.gdef = gdef;
         worker = new ImageWorker(1, 1); // Dummy worker, just to start with something
         try
         {
            createGraph();
         }
         finally
         {
            worker.dispose();
            worker = null;
            dproc = null;
         }
      }


      public Dictionary<string, string> GetValues(RrdGraphDef gdef)
      {
         this.gdef = gdef;

         bool lazy = lazyCheck();
         var values = new Dictionary<string, string>();
         if (!lazy || gdef.printStatementCount() != 0)
         {
            fetchData();
            resolveTextElements();
            foreach (CommentText comment in gdef.comments)
            {
               if (comment.GetType() == typeof(PrintText))
               {
                  PrintText pt = (PrintText)comment;
                  //Todo:Need to make hash key uniqe
                  if (pt.isPrint())
                  {
                     values.Add(pt.SrcName , pt.resolvedText);
                  }
               }
            }
         }
         return values;
      }
      /**
       * Returns complete graph information in a single object.
       *
       * @return Graph information (width, height, filename, image bytes, etc...)
       */
      public RrdGraphInfo getRrdGraphInfo()
      {
         return info;
      }

      private void createGraph()
      {
         bool lazy = lazyCheck();
         if (!lazy || gdef.printStatementCount() != 0)
         {
            fetchData();
            resolveTextElements();
            if (gdef.shouldPlot() && !lazy)
            {
               calculatePlotValues();
               findMinMaxValues();
               identifySiUnit();
               expandValueRange();
               removeOutOfRangeRules();
               initializeLimits();
               placeLegends();
               createImageWorker();
               drawBackground();
               drawAxis();
               drawData();
               drawGrid();
               drawText();
               drawLegend();
               drawRules();
               gator();
               drawOverlay();
               saveImage();
            }
         }
         collectInfo();
      }

      private void collectInfo()
      {
         info.filename = gdef.filename;
         info.width = im.xgif;
         info.height = im.ygif;
         foreach (CommentText comment in gdef.comments)
         {
            if (comment.GetType() == typeof(PrintText))
            {
               PrintText pt = (PrintText)comment;
               if (pt.isPrint())
               {
                  info.addPrintLine(pt.resolvedText);
               }
            }
         }
         if (gdef.imageInfo != null)
         {
            info.imgInfo = string.Format("{0}{1}{2}{3}", gdef.imageInfo, gdef.filename, im.xgif, im.ygif);
         }
      }

      private void saveImage()
      {
         if (gdef.filename.CompareTo("-") != 0)
         {
            info.bytes = worker.saveImage(gdef.filename, gdef.imageFormat, gdef.imageQuality);
         }
         else
         {
            info.bytes = worker.getImageBytes(gdef.imageFormat, gdef.imageQuality);
         }
      }

      private void drawOverlay()
      {
         if (gdef.overlayImage != null)
         {
            worker.loadImage(gdef.overlayImage);
         }
      }

      private void gator()
      {
         if (!gdef.onlyGraph && gdef.showSignature)
         {
            Font font = new Font(DEFAULT_FONT_NAME, 9, FontStyle.Regular);
            int x = (int)(im.xgif - 2 - worker.getFontAscent(font));
            int y = 4;

            worker.transform(x, y, 45f);
            worker.drawString("Created with Rrd4n", 0, 0, font, Color.LightGray);
            worker.reset();
         }
      }

      private void drawRules()
      {
         worker.clip(im.xorigin + 1, im.yorigin - gdef.height - 1, gdef.width - 1, gdef.height + 2);
         foreach (PlotElement pe in gdef.plotElements)
         {
            if (pe.GetType() == typeof(HRule))
            {
               HRule hr = (HRule)pe;
               if (hr.value >= im.minval && hr.value <= im.maxval)
               {
                  int y = mapper.ytr(hr.value);
                  worker.drawLine(im.xorigin, y, im.xorigin + im.xsize, y, hr.color, hr.width);
               }
            }
            else if (pe.GetType() == typeof(VRule))
            {
               VRule vr = (VRule)pe;
               if (vr.timestamp >= im.start && vr.timestamp <= im.end)
               {
                  int x = mapper.xtr(vr.timestamp);
                  worker.drawLine(x, im.yorigin, x, im.yorigin - im.ysize, vr.color, vr.width);
               }
            }
         }
         worker.reset();
      }

      private void drawText()
      {
         if (!gdef.onlyGraph)
         {
            if (gdef.title != null)
            {
               int x = im.xgif / 2 - (int)(worker.getStringWidth(gdef.title, gdef.largeFont) / 2);
               int y = PADDING_TOP;// +(int)worker.getFontAscent(gdef.largeFont);
               worker.drawString(gdef.title, x, y, gdef.largeFont, gdef.colors[COLOR_FONT]);
            }
            if (gdef.verticalLabel != null)
            {
               int x = PADDING_LEFT;
               int y = im.yorigin - im.ysize / 2 + (int)worker.getStringWidth(gdef.verticalLabel, gdef.smallFont) / 2;
               int ascent = (int)worker.getFontAscent(gdef.smallFont);
               worker.transform(x, y, -90f/*-Math.PI / 2*/);
               worker.drawString(gdef.verticalLabel, 0, ascent, gdef.smallFont, gdef.colors[COLOR_FONT]);
               worker.reset();
            }
         }
      }

      private void drawGrid()
      {
         if (!gdef.onlyGraph)
         {
            Color shade1 = gdef.colors[COLOR_SHADEA], shade2 = gdef.colors[COLOR_SHADEB];
            const float borderStroke = 1;
            worker.drawLine(0, 0, im.xgif - 1, 0, shade1, borderStroke);
            worker.drawLine(1, 1, im.xgif - 2, 1, shade1, borderStroke);
            worker.drawLine(0, 0, 0, im.ygif - 1, shade1, borderStroke);
            worker.drawLine(1, 1, 1, im.ygif - 2, shade1, borderStroke);
            worker.drawLine(im.xgif - 1, 0, im.xgif - 1, im.ygif - 1, shade2, borderStroke);
            worker.drawLine(0, im.ygif - 1, im.xgif - 1, im.ygif - 1, shade2, borderStroke);
            worker.drawLine(im.xgif - 2, 1, im.xgif - 2, im.ygif - 2, shade2, borderStroke);
            worker.drawLine(1, im.ygif - 2, im.xgif - 2, im.ygif - 2, shade2, borderStroke);
            if (gdef.drawXGrid)
            {
               new TimeAxis(this).draw();
            }
            if (gdef.drawYGrid)
            {
               bool ok;
               if (gdef.altYMrtg)
               {
                  ok = new ValueAxisMrtg(this).draw();
               }
               else if (gdef.logarithmic)
               {
                  ok = new ValueAxisLogarithmic(this).draw();
               }
               else
               {
                  ok = new ValueAxis(this).draw();
               }
               if (!ok)
               {
                  String msg = "No Data Found";
                  worker.drawString(msg,
                          im.xgif / 2 - (int)worker.getStringWidth(msg, gdef.largeFont) / 2,
                          (2 * im.yorigin - im.ysize) / 2,
                          gdef.largeFont, gdef.colors[COLOR_FONT]);
               }
            }
         }
      }

      private void drawData()
      {
         worker.setAntiAliasing(gdef.antiAliasing);
         worker.clip(im.xorigin + 1, im.yorigin - gdef.height - 1, gdef.width - 1, gdef.height + 2);
         double areazero = mapper.ytr((im.minval > 0.0) ? im.minval : (im.maxval < 0.0) ? im.maxval : 0.0);
         double[] x = xtr(dproc.getTimestamps()), lastY = null;
         // draw line, area and stack
         foreach (PlotElement plotElement in gdef.plotElements)
         {
            if (plotElement.GetType() == typeof(Line)
                || plotElement.GetType() == typeof(Area)
                || plotElement.GetType() == typeof(Stack))
            {
               SourcedPlotElement source = (SourcedPlotElement)plotElement;
               double[] y = ytr(source.getValues());
               if (source.GetType() == typeof(Line))
               {
                  worker.drawPolyline(x, y, source.color, ((Line)source).width);
               }
               else if (source.GetType() == typeof(Area))
               {
                  worker.fillPolygon(x, areazero, y, source.color);
               }
               else if (source.GetType() == typeof(Stack))
               {
                  Stack stack = (Stack)source;
                  float width = stack.getParentLineWidth();
                  if (width >= 0F)
                  {
                     // line
                     worker.drawPolyline(x, y, stack.color, width);
                  }
                  else
                  {
                     // area
                     worker.fillPolygon(x, lastY, y, stack.color);
                     worker.drawPolyline(x, lastY, stack.getParentColor(), 0);
                  }
               }
               else
               {
                  // should not be here
                  throw new ApplicationException("Unknown plot source: " + source.GetType());
               }
               lastY = y;
            }
         }
         worker.reset();
         worker.setAntiAliasing(false);
      }

      private void drawAxis()
      {
         if (!gdef.onlyGraph)
         {
            Color gridColor = gdef.colors[COLOR_GRID];
            Color fontColor = gdef.colors[COLOR_FONT];
            Color arrowColor = gdef.colors[COLOR_ARROW];
            float stroke = 1;
            worker.drawLine(im.xorigin + im.xsize, im.yorigin, im.xorigin + im.xsize, im.yorigin - im.ysize,
                    gridColor, stroke);
            worker.drawLine(im.xorigin, im.yorigin - im.ysize, im.xorigin + im.xsize, im.yorigin - im.ysize,
                    gridColor, stroke);
            worker.drawLine(im.xorigin - 4, im.yorigin, im.xorigin + im.xsize + 4, im.yorigin,
                    fontColor, stroke);
            worker.drawLine(im.xorigin, im.yorigin, im.xorigin, im.yorigin - im.ysize,
                    gridColor, stroke);
            worker.drawLine(im.xorigin + im.xsize + 4, im.yorigin - 3, im.xorigin + im.xsize + 4, im.yorigin + 3,
                    arrowColor, stroke);
            worker.drawLine(im.xorigin + im.xsize + 4, im.yorigin - 3, im.xorigin + im.xsize + 9, im.yorigin,
                    arrowColor, stroke);
            worker.drawLine(im.xorigin + im.xsize + 4, im.yorigin + 3, im.xorigin + im.xsize + 9, im.yorigin,
                    arrowColor, stroke);
         }
      }

      private void drawBackground()
      {
         worker.fillRect(0, 0, im.xgif, im.ygif, gdef.colors[COLOR_BACK]);
         if (gdef.backgroundImage != null)
         {
            worker.loadImage(gdef.backgroundImage);
         }
         worker.fillRect(im.xorigin, im.yorigin - im.ysize, im.xsize, im.ysize, gdef.colors[COLOR_CANVAS]);
      }

      private void createImageWorker()
      {
         worker.resize(im.xgif, im.ygif);
      }

      private void placeLegends()
      {
         if (!gdef.noLegend && !gdef.onlyGraph)
         {
            int border = (int)(getSmallFontCharWidth() * PADDING_LEGEND);
            LegendComposer lc = new LegendComposer(this, border, im.ygif, im.xgif - 2 * border);
            im.ygif = lc.placeComments() + PADDING_BOTTOM;
         }
      }

      private void initializeLimits()
      {
         im.xsize = gdef.width;
         im.ysize = gdef.height;
         im.unitslength = gdef.unitsLength;
         if (gdef.onlyGraph)
         {
            if (im.ysize > 64)
            {
               throw new ArgumentException("Cannot create graph only, height too big: " + im.ysize);
            }
            im.xorigin = 0;
         }
         else
         {
            im.xorigin = (int)(PADDING_LEFT + im.unitslength * getSmallFontCharWidth());
         }
         if (gdef.verticalLabel != null)
         {
            im.xorigin += (int)getSmallFontHeight();
         }
         if (gdef.onlyGraph)
         {
            im.yorigin = im.ysize;
         }
         else
         {
            im.yorigin = PADDING_TOP + im.ysize;
         }
         mapper = new Mapper(this);
         if (gdef.title != null)
         {
            im.yorigin += (int)getLargeFontHeight() + PADDING_TITLE;
         }
         if (gdef.onlyGraph)
         {
            im.xgif = im.xsize;
            im.ygif = im.yorigin;
         }
         else
         {
            im.xgif = PADDING_RIGHT + im.xsize + im.xorigin;
            im.ygif = im.yorigin + (int)(PADDING_PLOT * getSmallFontHeight());
         }
      }

      private void removeOutOfRangeRules()
      {
         foreach (PlotElement plotElement in gdef.plotElements)
         {
            if (plotElement.GetType() == typeof(HRule))
            {
               ((HRule)plotElement).setLegendVisibility(im.minval, im.maxval, gdef.forceRulesLegend);
            }
            else if (plotElement.GetType() == typeof(VRule))
            {
               ((VRule)plotElement).setLegendVisibility(im.start, im.end, gdef.forceRulesLegend);
            }
         }
      }

      private void expandValueRange()
      {
         im.ygridstep = (gdef.valueAxisSetting != null) ? gdef.valueAxisSetting.gridStep : Double.NaN;
         im.ylabfact = (gdef.valueAxisSetting != null) ? gdef.valueAxisSetting.labelFactor : 0;
         if (!gdef.rigid && !gdef.logarithmic)
         {
            double scaled_min, scaled_max, adj;
            if (Double.IsNaN(im.ygridstep))
            {
               if (gdef.altYMrtg)
               { /* mrtg */
                  im.decimals = Math.Ceiling(Math.Log10(Math.Max(Math.Abs(im.maxval), Math.Abs(im.minval))));
                  im.quadrant = 0;
                  if (im.minval < 0)
                  {
                     im.quadrant = 2;
                     if (im.maxval <= 0)
                     {
                        im.quadrant = 4;
                     }
                  }
                  switch (im.quadrant)
                  {
                     case 2:
                        im.scaledstep = Math.Ceiling(50 * Math.Pow(10, -(im.decimals)) * Math.Max(Math.Abs(im.maxval),
                                Math.Abs(im.minval))) * Math.Pow(10, im.decimals - 2);
                        scaled_min = -2 * im.scaledstep;
                        scaled_max = 2 * im.scaledstep;
                        break;
                     case 4:
                        im.scaledstep = Math.Ceiling(25 * Math.Pow(10,
                                -(im.decimals)) * Math.Abs(im.minval)) * Math.Pow(10, im.decimals - 2);
                        scaled_min = -4 * im.scaledstep;
                        scaled_max = 0;
                        break;
                     default: /* quadrant 0 */
                        im.scaledstep = Math.Ceiling(25 * Math.Pow(10, -(im.decimals)) * im.maxval) *
                                Math.Pow(10, im.decimals - 2);
                        scaled_min = 0;
                        scaled_max = 4 * im.scaledstep;
                        break;
                  }
                  im.minval = scaled_min;
                  im.maxval = scaled_max;
               }
               else if (gdef.altAutoscale)
               {
                  /* measure the amplitude of the function. Make sure that
                          graph boundaries are slightly higher then max/min vals
                          so we can see amplitude on the graph */
                  double delt, fact;

                  delt = im.maxval - im.minval;
                  adj = delt * 0.1;
                  fact = 2.0 * Math.Pow(10.0,
                          Math.Floor(Math.Log10(Math.Max(Math.Abs(im.minval), Math.Abs(im.maxval)))) - 2);
                  if (delt < fact)
                  {
                     adj = (fact - delt) * 0.55;
                  }
                  im.minval -= adj;
                  im.maxval += adj;
               }
               else if (gdef.altAutoscaleMax)
               {
                  /* measure the amplitude of the function. Make sure that
                          graph boundaries are slightly higher than max vals
                          so we can see amplitude on the graph */
                  adj = (im.maxval - im.minval) * 0.1;
                  im.maxval += adj;
               }
               else
               {
                  scaled_min = im.minval / im.magfact;
                  scaled_max = im.maxval / im.magfact;
                  for (int i = 1; SENSIBLE_VALUES[i] > 0; i++)
                  {
                     if (SENSIBLE_VALUES[i - 1] >= scaled_min && SENSIBLE_VALUES[i] <= scaled_min)
                     {
                        im.minval = SENSIBLE_VALUES[i] * im.magfact;
                     }
                     if (-SENSIBLE_VALUES[i - 1] <= scaled_min && -SENSIBLE_VALUES[i] >= scaled_min)
                     {
                        im.minval = -SENSIBLE_VALUES[i - 1] * im.magfact;
                     }
                     if (SENSIBLE_VALUES[i - 1] >= scaled_max && SENSIBLE_VALUES[i] <= scaled_max)
                     {
                        im.maxval = SENSIBLE_VALUES[i - 1] * im.magfact;
                     }
                     if (-SENSIBLE_VALUES[i - 1] <= scaled_max && -SENSIBLE_VALUES[i] >= scaled_max)
                     {
                        im.maxval = -SENSIBLE_VALUES[i] * im.magfact;
                     }
                  }
               }
            }
            else
            {
               im.minval = im.ylabfact * im.ygridstep *
                       Math.Floor(im.minval / (im.ylabfact * im.ygridstep));
               im.maxval = im.ylabfact * im.ygridstep *
                       Math.Ceiling(im.maxval / (im.ylabfact * im.ygridstep));
            }

         }
      }

      private void identifySiUnit()
      {
         im.unitsexponent = gdef.unitsExponent;
         im.scaleBase = gdef.scaleBase;
         if (!gdef.logarithmic)
         {
            int symbcenter = 6;
            double digits;
            if (im.unitsexponent != int.MaxValue)
            {
               digits = Math.Floor(im.unitsexponent / 3);
            }
            else
            {
               digits = Math.Floor(Math.Log(Math.Max(Math.Abs(im.minval), Math.Abs(im.maxval))) / Math.Log(im.scaleBase));
            }
            im.magfact = Math.Pow(im.scaleBase, digits);
            if (((digits + symbcenter) < SYMBOLS.Length) && ((digits + symbcenter) >= 0))
            {
               im.symbol = SYMBOLS[(int)digits + symbcenter];
            }
            else
            {
               im.symbol = '?';
            }
         }
      }

      private void findMinMaxValues()
      {
         double minval = Double.NaN, maxval = Double.NaN;
         foreach (PlotElement pe in gdef.plotElements)
         {
            if (pe.GetType() == typeof(Line)
                || pe.GetType() == typeof(Area)
                || pe.GetType() == typeof(Stack))
            {
               minval = Util.min(((SourcedPlotElement)pe).getMinValue(), minval);
               maxval = Util.max(((SourcedPlotElement)pe).getMaxValue(), maxval);
            }
         }
         if (Double.IsNaN(minval))
         {
            minval = 0D;
         }
         if (Double.IsNaN(maxval))
         {
            maxval = 1D;
         }
         im.minval = gdef.minValue;
         im.maxval = gdef.maxValue;
         /* adjust min and max values */
         if (Double.IsNaN(im.minval) || ((!gdef.logarithmic && !gdef.rigid) && im.minval > minval))
         {
            im.minval = minval;
         }
         if (Double.IsNaN(im.maxval) || (!gdef.rigid && im.maxval < maxval))
         {
            if (gdef.logarithmic)
            {
               im.maxval = maxval * 1.1;
            }
            else
            {
               im.maxval = maxval;
            }
         }
         /* make sure min is smaller than max */
         if (im.minval > im.maxval)
         {
            im.minval = 0.99 * im.maxval;
         }
         /* make sure min and max are not equal */
         if (im.minval == im.maxval)
         {
            im.maxval *= 1.01;
            if (!gdef.logarithmic)
            {
               im.minval *= 0.99;
            }
            /* make sure min and max are not both zero */
            if (im.maxval == 0.0)
            {
               im.maxval = 1.0;
            }
         }
      }

      private void calculatePlotValues()
      {
         foreach (PlotElement pe in gdef.plotElements)
         {
            if (pe.GetType() == typeof(Line)
                || pe.GetType() == typeof(Area)
                || pe.GetType() == typeof(Stack))
            {
               pe.assignValues(dproc);
            }
         }
      }

      private void resolveTextElements()
      {
         ValueScaler valueScaler = new ValueScaler(gdef.scaleBase);
         foreach (CommentText comment in gdef.comments)
         {
            comment.resolveText(dproc, valueScaler);
         }
      }

      private void fetchData()
      {
         dproc = new DataProcessor(gdef.startTime, gdef.endTime, DbAccessInterface);

         //dproc.DbAccessInterface = new rrd4n.DataAccess.LocalFile.FileAccessor();

         if (gdef.step > 0)
         {
            dproc.setStep(gdef.step);
         }
         foreach (Source src in gdef.sources)
         {
            src.requestData(dproc);
         }
         dproc.processData();
         //long[] t = dproc.getTimestamps();
         //im.start = t[0];
         //im.end = t[t.length - 1];
         im.start = gdef.startTime;
         im.end = gdef.endTime;
      }

      private bool lazyCheck()
      {
         // redraw if lazy option is not set or file does not exist
         if (!gdef.lazy || !Util.fileExists(gdef.filename))
         {
            return false; // 'false' means 'redraw'
         }
         // redraw if not enough time has passed
         long secPerPixel = (gdef.endTime - gdef.startTime) / gdef.width;
         long elapsed = Util.getTimestamp() - Util.getLastModified(gdef.filename);
         return elapsed <= secPerPixel;
      }

      private void drawLegend()
      {
         if (!gdef.onlyGraph && !gdef.noLegend)
         {
            //int ascent = (int) worker.getFontAscent(gdef.smallFont);
            int height = (int)worker.getFontHeight(gdef.smallFont);
            foreach (CommentText c in gdef.comments)
            {
               if (c.isValidGraphElement())
               {
                  int x = c.x, y = c.y + height;
                  if (c.GetType() == typeof(LegendText))
                  {
                     // draw with BOX
                     int box = (int)getBox();
                     int boxSpace = (int)(getBoxSpace());
                     worker.fillRect(x, y - box, box, box, gdef.colors[COLOR_FRAME]);
                     worker.fillRect(x + 1, y - box + 1, box - 2, box - 2, ((LegendText)c).legendColor);
                     worker.drawString(c.resolvedText, c.x + boxSpace, c.y, gdef.smallFont, gdef.colors[COLOR_FONT]);
                  }
                  else
                  {
                     worker.drawString(c.resolvedText, x, y - height, gdef.smallFont, gdef.colors[COLOR_FONT]);
                  }
               }
            }
         }
      }

      // helper methods

      internal double getSmallFontHeight()
      {
         return worker.getFontHeight(gdef.smallFont);
      }

      private double getLargeFontHeight()
      {
         return worker.getFontHeight(gdef.largeFont);
      }

      private double getSmallFontCharWidth()
      {
         return worker.getStringWidth("a", gdef.smallFont);
      }

      internal double getInterlegendSpace()
      {
         return getSmallFontCharWidth() * LEGEND_INTERSPACING;
      }

      internal double getLeading()
      {
         return getSmallFontHeight() * LEGEND_LEADING;
      }

      internal double getSmallLeading()
      {
         return getSmallFontHeight() * LEGEND_LEADING_SMALL;
      }

      internal double getBoxSpace()
      {
         return Math.Ceiling(getSmallFontHeight() * LEGEND_BOX_SPACE);
      }

      private double getBox()
      {
         return getSmallFontHeight() * LEGEND_BOX;
      }

      double[] xtr(long[] timestamps)
      {
         /*
           double[] timestampsDev = new double[timestamps.length];
           for (int i = 0; i < timestamps.length; i++) {
               timestampsDev[i] = mapper.xtr(timestamps[i]);
           }
           return timestampsDev;
           */
         double[] timestampsDev = new double[2 * timestamps.Length - 1];
         for (int i = 0, j = 0; i < timestamps.Length; i += 1, j += 2)
         {
            timestampsDev[j] = mapper.xtr(timestamps[i]);
            if (i < timestamps.Length - 1)
            {
               timestampsDev[j + 1] = timestampsDev[j];
            }
         }
         return timestampsDev;
      }

      double[] ytr(double[] values)
      {
         /*
           double[] valuesDev = new double[values.length];
           for (int i = 0; i < values.length; i++) {
               if (Double.IsNaN(values[i])) {
                   valuesDev[i] = Double.NaN;
               }
               else {
                   valuesDev[i] = mapper.ytr(values[i]);
               }
           }
           return valuesDev;
           */
         double[] valuesDev = new double[2 * values.Length - 1];
         for (int i = 0, j = 0; i < values.Length; i += 1, j += 2)
         {
            if (Double.IsNaN(values[i]))
            {
               valuesDev[j] = Double.NaN;
            }
            else
            {
               valuesDev[j] = mapper.ytr(values[i]);
            }
            if (j > 0)
            {
               valuesDev[j - 1] = valuesDev[j];
            }
         }
         return valuesDev;
      }

      /**
       * Renders this graph onto graphing device
       * @param g Graphics handle
       */
      public void render(Graphics g)
      {
         byte[] imageData = getRrdGraphInfo().getBytes();
         MemoryStream ms = new MemoryStream(imageData);
         Image bitmap = Image.FromStream(ms);
         //ImageIcon image = new ImageIcon(imageData);
         //image.paintIcon(null, g, 0, 0);
      }
   }
}
