using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using rrd4n.Common;

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

    class ImageWorker
    {
        private const string DUMMY_TEXT = "Dummy";

        private Image img;
        private Graphics gd;
        private int imgWidth, imgHeight;
        private System.Drawing.Drawing2D.Matrix aftInitial;

        public ImageWorker(int width, int height)
        {
            resize(width, height);
        }

        public void resize(int width, int height)
        {
            this.imgWidth = width;
            this.imgHeight = height;
            this.img = new Bitmap(width, height);

            img = new Bitmap(width, height);
            gd = Graphics.FromImage(img);
            this.aftInitial = gd.Transform;
            this.setAntiAliasing(false);
        }

        public void clip(int x, int y, int width, int height)
        {
            gd.SetClip(new RectangleF(x, y, width, height));
        }

        public void transform(int x, int y, double angle)
        {
            gd.TranslateTransform(x, y);
            gd.RotateTransform((float)angle);
        }

        public void reset()
        {
            gd.Transform = aftInitial;
            gd.SetClip(new Rectangle(0, 0, imgWidth, imgHeight));
        }

        public void fillRect(int x, int y, int width, int height, Color paint)
        {

            gd.FillRectangle(new SolidBrush(paint), x, y, width, height);
        }

        public void fillPolygon(int[] x, int[] y, Color paint)
        {
            Point[] points = new Point[x.Length];
            for (int i = 0; i < points.Length; i++)
            {
                points[i] = new Point(x[i], y[i]);
            }
            gd.FillPolygon(new SolidBrush(paint), points);
        }

        public void fillPolygon(double[] x, double yBottom, double[] yTop, Color paint)
        {
            // create graphics path for the bar chart
            GraphicsPath barGraph = new GraphicsPath();
            //Debug.WriteLine(paint.ToString());
            //for (int i = 0; i < x.Length - 1; i++)
            //{
            //    Debug.WriteLine(x[i].ToString() + "\t" + yTop[i].ToString());
            //}

            // Have to add initial line from base line to first y point if first y point not on base line?
            // to enable closing of the polygon
            if (!double.IsNaN(yTop[0]) && yTop[0] != yBottom)
               barGraph.AddLine((float)x[0], (float)yBottom, (float)x[0], (float)yTop[0]);

            for (int i = 0; i < x.Length - 1; i += 2)
            {
                // Don't draw 'non' data points
                if (double.IsNaN(yTop[i])) yTop[i] = yBottom;
                if (double.IsNaN(yTop[i + 1])) yTop[i + 1] = yBottom;

                barGraph.AddLine((float)x[i], (float)yTop[i], (float)x[i + 1], (float)yTop[i + 1]);
            }


            // Fill in last part of polygon to get the line down to base line and close polygon from that last point
            double lastX = x[x.Length - 1];
            double lastY = yTop[x.Length - 1];

            if (lastY != yBottom && !double.IsNaN(lastY))
                barGraph.AddLine((float)lastX, (float)lastY, (float)lastX, (float)yBottom);

            barGraph.CloseFigure();
            gd.FillPath(new SolidBrush(paint), barGraph);
        }

        public void fillPolygon(double[] x, double[] yBottom, double[] yTop, Color paint)
        {
            // create graphics path for the bar chart
            GraphicsPath barGraph = new GraphicsPath();
            //Debug.WriteLine(paint.ToString());

            //for (int i = 0; i < x.Length - 1; i++)
            //{
            //    Debug.WriteLine(x[i].ToString() + "\t" + yBottom[i].ToString() + "\t" + yTop[i].ToString());
            //}

            // ToDo: Handle NaN values in array
            for (int i = 0; i < x.Length - 1; i += 2)
            {
                barGraph.AddLine((float)x[i], (float)yTop[i], (float)x[i + 1], (float)yTop[i + 1]);
            }

            // Fill in last part of polygon to get the line down to base line and close polygon from that last point
            double lastX = x[x.Length - 1];
            double lastY = yTop[x.Length - 1];
            double yBase = Util.max(yTop);

            if (lastY != yBase && !double.IsNaN(lastY))
                barGraph.AddLine((float)lastX, (float)lastY, (float)lastX, (float)yBase);

            // ToDo: Handle NaN values in array
            for (int i = 0; i < x.Length - 1; i += 2)
            {
                barGraph.AddLine((float)x[i], (float)yBottom[i], (float)x[i + 1], (float)yBottom[i + 1]);
            }

            // Fill in last part of polygon to get the line down to base line and close polygon from that last point
            lastY = yBottom[x.Length - 1];
            yBase = Util.max(yBottom);

            if (lastY != yBase && !double.IsNaN(lastY))
                barGraph.AddLine((float)lastX, (float)lastY, (float)lastX, (float)yBase);

            barGraph.CloseFigure();
            gd.FillPath(new SolidBrush(paint), barGraph);
        }


        public void drawLine(int x1, int y1, int x2, int y2, Color paint, float stroke)
        {
            gd.DrawLine(new Pen(paint, stroke), x1, y1, x2, y2);
        }

        public void drawPolyline(int[] x, int[] y, Color paint, float stroke)
        {
            var points = new Point[x.Length];
            for (var i = 0; i < points.Length; i++)
                points[i] = new Point(x[i], y[i]);

            gd.DrawPolygon(new Pen(paint, stroke), points);
        }

        public void drawPolyline(double[] x, double[] y, Color paint, float stroke)
        {
            var points = new PointF[x.Length];
            List<PointF> pts = new List<PointF>();
            for (var i = 0; i < points.Length; i++)
            {
                if (double.IsNaN(y[i]))
                    continue;
                pts.Add(new PointF((float)x[i], (float)y[i]));

                points[i] = new PointF((float)x[i], (float)y[i]);
            }
            if (pts.Count > 1)
               gd.DrawLines(new Pen(paint, stroke), pts.ToArray());
            //gd.setColor(paint);
            //gd.setStroke(stroke);
            //PathIterator path = new PathIterator(y);
            //for (int[] pos = path.getNextPath(); pos != null; pos = path.getNextPath()) {
            //    int start = pos[0], end = pos[1];
            //    int[] xDev = new int[end - start], yDev = new int[end - start];
            //    for (int i = start; i < end; i++) {
            //        xDev[i - start] = (int) x[i];
            //        yDev[i - start] = (int) y[i];
            //    }
            //    gd.drawPolyline(xDev, yDev, xDev.length);
            //}
        }

        public void drawString(String text, int x, int y, Font font, Color paint)
        {
            gd.DrawString(text, font, new SolidBrush(paint), x, y);
        }

        public double getFontAscent(Font font)
        {
            int ascent = (int)(font.FontFamily.GetCellAscent(font.Style) *
                                 font.Size / font.FontFamily.GetEmHeight(font.Style));
            return ascent;
        }

        public double getFontHeight(Font font)
        {
            int descent = (int)(font.FontFamily.GetCellDescent(font.Style) *
                                 font.Size / font.FontFamily.GetEmHeight(font.Style));
            double ascent = getFontAscent(font);
            return ascent + descent;
        }

        public double getStringWidth(String text, Font font)
        {
            SizeF sizeF = gd.MeasureString(text, font);
            return sizeF.Width;
        }

        public void setAntiAliasing(bool enable)
        {
            gd.SmoothingMode = enable ? SmoothingMode.AntiAlias : SmoothingMode.None;
        }

        public void dispose()
        {
            gd.Dispose();
        }

        public void saveImage(Stream stream, String type, float quality)
        {

            if (type.ToLower().CompareTo("png") == 0)
            {
                img.Save(stream, ImageFormat.Png);
            }
            else if (type.ToLower().CompareTo("gif") == 0)
            {
                img.Save(stream, ImageFormat.Gif);
            }
            else if (type.ToLower().CompareTo("jpg") == 0 || type.ToLower().CompareTo("jpeg") == 0)
            {
                // ToDo: handle quality using an ImageCodecInfo
                img.Save(stream, ImageFormat.Jpeg);
            }
            else
            {
                throw new IOException("Unsupported image format: " + type);
            }
            stream.Flush();
        }

        public byte[] saveImage(String path, String type, float quality)
        {
            byte[] bytes = getImageBytes(type, quality);

            using (Stream sw = File.Create(path))
            {
                sw.Write(bytes, 0, bytes.Length);
                sw.Close();

            }
            return bytes;
        }

        protected internal byte[] getImageBytes(String type, float quality)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                saveImage(stream, type, quality);
                stream.Close();
                return stream.ToArray();
            }
        }

        public void loadImage(String imageFile)
        {
            img = Image.FromFile(imageFile);
            gd = Graphics.FromImage(img);
        }
    }
}
