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

   public class ImageParameters
    {
       internal long start;
       internal long end;
       internal double minval;
       internal double maxval;
      public double unitsexponent;
      internal double scaleBase;
       internal double magfact;
       public char symbol;
       internal double ygridstep;
       internal int ylabfact;
      internal double decimals;
       internal int quadrant;
       internal double scaledstep;
       internal int xsize;
       internal int ysize;
       internal int xorigin;
       internal int yorigin;
      internal int unitslength;
      internal int xgif;
      internal int ygif;
      public String unit;
    }
}
