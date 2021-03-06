﻿using System;

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


   class Mapper
   {
      private readonly RrdGraphDef gdef;
      private readonly ImageParameters im;
      private readonly double pixieX;
      private readonly double pixieY;

      public Mapper(RrdGraph rrdGraph)
      {
         this.gdef = rrdGraph.gdef;
         this.im = rrdGraph.im;
         pixieX = (double)im.xsize / (double)(im.end - im.start);
         if (!gdef.logarithmic)
         {
            pixieY = (double)im.ysize / (im.maxval - im.minval);
         }
         else
         {
            pixieY = (double)im.ysize / (Math.Log10(im.maxval) - Math.Log10(im.minval));
         }
      }

      internal int xtr(double mytime)
      {
         return (int)((double)im.xorigin + pixieX * (mytime - im.start));
      }

      internal int ytr(double value)
      {
         double yval;
         if (!gdef.logarithmic)
         {
            yval = im.yorigin - pixieY * (value - im.minval) + 0.5;
         }
         else
         {
            if (value < im.minval)
            {
               yval = im.yorigin;
            }
            else
            {
               yval = im.yorigin - pixieY * (Math.Log10(value) - Math.Log10(im.minval)) + 0.5;
            }
         }
         if (!gdef.rigid)
         {
            return (int)yval;
         }
         else if ((int)yval > im.yorigin)
         {
            return im.yorigin + 2;
         }
         else if ((int)yval < im.yorigin - im.ysize)
         {
            return im.yorigin - im.ysize - 2;
         }
         else
         {
            return (int)yval;
         }
      }
   }
}
