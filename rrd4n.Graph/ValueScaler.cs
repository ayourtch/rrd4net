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

   public class ValueScaler
    {
        static readonly String UNIT_UNKNOWN = "?";
        static readonly String[] UNIT_SYMBOLS = {
		"a", "f", "p", "n", "u", "m", " ", "k", "M", "G", "T", "P", "E"
	};
        static readonly int SYMB_CENTER = 6;

        private readonly double scaleBase;
        private double magfact = -1; // nothing scaled before, rescale
        private String unit;

        public ValueScaler(double scaleBase)
        {
            this.scaleBase = scaleBase;
        }

        public Scaled scale(double value, bool mustRescale)
        {
            Scaled scaled;
            if (mustRescale)
            {
                scaled = rescale(value);
            }
            else if (magfact >= 0)
            {
                // already scaled, need not rescale
                scaled = new Scaled(value / magfact, unit);
            }
            else
            {
                // scaling not requested, but never scaled before - must rescale anyway
                scaled = rescale(value);
                // if zero, scale again on the next try
                if (scaled.value == 0.0 || Double.IsNaN(scaled.value))
                {
                    magfact = -1.0;
                }
            }
            return scaled;
        }

        private Scaled rescale(double value)
        {
            int sindex;
            if (value == 0.0 || Double.IsNaN(value))
            {
                sindex = 0;
                magfact = 1.0;
            }
            else
            {
                sindex = (int)(Math.Floor(Math.Log(Math.Abs(value)) / Math.Log(scaleBase)));
                magfact = Math.Pow(scaleBase, sindex);
            }
            if (sindex <= SYMB_CENTER && sindex >= -SYMB_CENTER)
            {
                unit = UNIT_SYMBOLS[sindex + SYMB_CENTER];
            }
            else
            {
                unit = UNIT_UNKNOWN;
            }
            return new Scaled(value / magfact, unit);
        }


    }
}
