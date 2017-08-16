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
using System.Collections.Generic;
using System.Text;
using System.Xml;
using rrd4n.Common;

namespace rrd4n.Core
{


    /**
     * <p>
     * Class to represent internal RRD archive state for a single datasource. Objects of this
     * class are never manipulated directly, it's up to Rrd4n framework to manage
     * internal arcihve states.</p>
     *
     * @author Mikael Nilsson
     */
    public class ArcState : RrdUpdater {
	private Archive parentArc;

	private RrdDouble accumValue;
	private RrdLong nanSteps;

	public ArcState(Archive parentArc, bool shouldInitialize) {
		this.parentArc = parentArc;
		accumValue = new RrdDouble(this);
		nanSteps = new RrdLong(this);
		if(shouldInitialize) {
			Header header = parentArc.getParentDb().getHeader();
			long step = header.getStep();
			long lastUpdateTime = header.getLastUpdateTime();
			long arcStep = parentArc.getArcStep();
			long initNanSteps = (Util.normalize(lastUpdateTime, step) -
				Util.normalize(lastUpdateTime, arcStep)) / step;
			accumValue.set(Double.NaN);
			nanSteps.set(initNanSteps);
		}
	}

	public String dump() {
		return "accumValue:" + accumValue.get() + " nanSteps:" + nanSteps.get() + "\n";
	}

	public void setNanSteps(long value) {
		nanSteps.set(value);
	}

	/**
	 * Returns the number of currently accumulated NaN steps.
	 *
	 * @return Number of currently accumulated NaN steps.
	 * @Thrown in case of I/O error
	 */
	public long getNanSteps() {
		return nanSteps.get();
	}

	public void setAccumValue(double value) {
		accumValue.set(value);
	}

	/**
	 * Returns the value accumulated so far.
	 *
	 * @return Accumulated value
	 * @Thrown in case of I/O error
	 */
	public double getAccumValue() {
		return accumValue.get();
	}

	/**
	 * Returns the Archive object to which this ArcState object belongs.
	 *
	 * @return Parent Archive object.
	 */
	public Archive getParent() {
		return parentArc;
	}

   public void appendXml(XmlWriter writer)
   {
      writer.WriteStartElement("ds");
      writer.WriteElementString("value", accumValue.get().ToString());
      writer.WriteElementString("unknown_datapoints", nanSteps.get().ToString());
      writer.WriteEndElement(); // ds
   }

	/**
	 * Copies object's internal state to another ArcState object.
	 * @param other New ArcState object to copy state to
	 * @Thrown in case of I/O error
	 */
	public void copyStateTo(RrdUpdater other) {
		if((other.GetType() != typeof(ArcState))) {
			throw new ArgumentException("Cannot copy ArcState object to " + other.GetType().ToString());
		}
		ArcState arcState = (ArcState) other;
		arcState.accumValue.set(accumValue.get());
		arcState.nanSteps.set(nanSteps.get());
	}

	/**
	 * Returns the underlying storage (backend) object which actually performs all
	 * I/O operations.
	 * @return I/O backend object
	 */
	public RrdBackend getRrdBackend() {
		return parentArc.getRrdBackend();
	}

	/**
	 * Required to implement RrdUpdater interface. You should never call this method directly.
	 * @return Allocator object
	 */
	public RrdAllocator getRrdAllocator() {
		return parentArc.getRrdAllocator();
	}
}
}
