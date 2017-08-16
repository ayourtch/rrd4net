/* ============================================================
 * Rrd4n : Pure c# implementation of RRDTool's functionality
 * ============================================================
 *
 * Project Info:  http://minidev.se
 * Project Lead:  Mikael Nilsson (info@minidev.se)
 *
 * (C) Copyright 2009-2010, by Mikael Nilsson.
 *
 * Developers:    Mikael Nilsson
 *
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
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

namespace rrd4n.Core
{

    class RrdDoubleArray : RrdPrimitive
    {
        private int length;

        public RrdDoubleArray(RrdUpdater updater, int length)
            : base(updater, RrdPrimitive.PrimitiveType.RRD_DOUBLE, length, false)
        {
            this.length = length;
        }

        public void set(int index, double value)
        {
            set(index, value, 1);
        }

        public void set(int index, double value, int count)
        {
            // rollovers not allowed!
            Debug.Assert(index + count <= length, "Invalid robin index supplied: index=" + index + ", count=" + count + ", length=" + length);
            writeDouble(index, value, count);
        }

        public double get(int index)
        {
            Debug.Assert(index < length, "Invalid index supplied: " + index + ", length=" + length);
            return readDouble(index);
        }

        public double[] get(int index, int count)
        {
            Debug.Assert(index + count <= length, "Invalid index/count supplied: " + index + "/" + count + " (length=" + length + ")");
            return readDouble(index, count);
        }

    }
}
