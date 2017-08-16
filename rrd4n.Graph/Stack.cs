using System;
using System.Drawing;
using rrd4n.Data;

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
class Stack : SourcedPlotElement {
	private readonly SourcedPlotElement parent;

   internal Stack(SourcedPlotElement parent, String srcName, Color color) 
   : base(srcName, color)
   {
		
		this.parent = parent;
	}

	override internal void  assignValues(DataProcessor dproc) {
		double[] parentValues = parent.getValues();
		double[] procValues = dproc.getValues(srcName);
		values = new double[procValues.Length];
		for(int i = 0; i < values.Length; i++) {
			values[i] = parentValues[i] + procValues[i];
		}
	}

   internal float getParentLineWidth() {
	   if (parent.GetType() == typeof (Line))
	      return ((Line) parent).width;
	   else if (parent.GetType() == typeof (Area))
	      return -1F;
	   else /* if(parent instanceof Stack) */
	      return ((Stack) parent).getParentLineWidth();
	}

   internal Color getParentColor() {
		return parent.color;
	}
}
}
