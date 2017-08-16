/* ============================================================
 * Rrd4n : Pure c# implementation of RRDTool's functionality
 * ============================================================
 *
 * Project Info:  http://minidev.se
 * Project Lead:  Mikael Nilsson (info@minidev.se)
 *
 * (C) Copyright 2009-2010, by Mikael Nilsson.
 *
 * This library is free software; you can redistribute it and/or modify it under the terms
 * of the GNU Lesser General Public License as published by the Free Software Foundation;
 * either version 2.1 of the License, or (at your option) any later version.
 *
 * Developers:    Mikael Nilsson
 *
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
using System.Collections.Generic;
using System.Text;

namespace rrd4n.Core
{


class RrdString : RrdPrimitive {
	private String cache;

	public RrdString(RrdUpdater updater, bool isConstant)
        :base (updater, RrdPrimitive.PrimitiveType.RRD_STRING, isConstant)
    {
	}

	public RrdString(RrdUpdater updater) 
        :this(updater, false)
    {
	}

	public void set(String value) {
		if(!isCachingAllowed()) 
			writeString(value);
		else if(cache == null || cache.CompareTo(value) != 0) 
			writeString(cache = value);
	}

	public String get() {
		return (cache != null)? cache: readString();
	}
}
}
