using System;
using rrd4n.Common;

namespace rrd4n.Data
{
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


/**
 * Simple class which holds aggregated values (MIN, MAX, FIRST, LAST, AVERAGE and TOTAL). You
 * don't need to create objects of this class directly. Objects of this class are returned from
 * <code>getAggregates()</code> method in
 * {@link org.Rrd4n.core.FetchData#getAggregates(String) FetchData} and
 * {@link DataProcessor#getAggregates(String)} DataProcessor} classes.
 */
public class Aggregates {
    double min = Double.NaN, max = Double.NaN;
    double first = Double.NaN, last = Double.NaN;
    double average = Double.NaN, total = Double.NaN;
    public long LastTimeStamp { get; set; }
    public long FirstTimeStamp { get; set; }
    public long MaxTimeStamp { get; set; }
    public long MinTimeStamp { get; set; }

    public double Min
    { 
        get { return min; }
        set { min = value; }
    }

    public double Max
    { 
        get { return max; }
        set { max = value; }
    }

    public double First
    {
        get { return first; }
        set { first = value; }
    }

    public double Last
    {
        get { return last; }
        set { last = value; }
    }

    public double Average
    {
        get {return average;}
        set {average = value;}
    }

    public double Total
    {
        get { return total; }
        set { total = value; }
    }


    public Aggregates()
    {
        // NOP;
    }

    /**
     * Returns the minimal value
     *
     * @return Minimal value
     */
    public double getMin() {
        return min;
    }

    /**
     * Returns the maximum value
     *
     * @return Maximum value
     */
    public double getMax() {
        return max;
    }

    /**
     * Returns the first falue
     *
     * @return First value
     */
    public double getFirst() {
        return first;
    }

    /**
     * Returns the last value
     *
     * @return Last value
     */
    public double getLast() {
        return last;
    }

    /**
     * Returns average
     *
     * @return Average value
     */
    public double getAverage() {
        return average;
    }

    /**
     * Returns total value
     *
     * @return Total value
     */
    public double getTotal() {
        return total;
    }

    /**
     * Returns single aggregated value for the give consolidation function
     *
     * @param aggregateFunction Aggregate function: MIN, MAX, FIRST, LAST, AVERAGE, TOTAL. These constanst
     *                  are conveniently defined in the {@link rrd4n.Common.AggregateFunction ConsolFun} interface.
     *
     * @return Aggregated value
     *
     * @throws ArgumentException Thrown if unsupported consolidation function is supplied
     */
    public double getAggregate(AggregateFunction.Type aggregateFunction)
    {
       switch (aggregateFunction)
       {
          case AggregateFunction.Type.AVERAGE:
                return average;
          case AggregateFunction.Type.FIRST:
                return first;
          case AggregateFunction.Type.LAST:
                return last;
          case AggregateFunction.Type.MAX:
                return max;
          case AggregateFunction.Type.MIN:
                return min;
          case AggregateFunction.Type.TOTAL:
                return total;
        }
       throw new ArgumentException("Unknown aggregate function: " + aggregateFunction);
    }

    public long getTimeStamp(AggregateFunction.Type aggregateFunction)
    {
       switch (aggregateFunction)
       {
          case AggregateFunction.Type.AVERAGE:
             return LastTimeStamp;
          case AggregateFunction.Type.FIRST:
             return FirstTimeStamp;
          case AggregateFunction.Type.LAST:
             return LastTimeStamp;
          case AggregateFunction.Type.MAX:
             return MaxTimeStamp;
          case AggregateFunction.Type.MIN:
             return MinTimeStamp;
          case AggregateFunction.Type.TOTAL:
             return LastTimeStamp;
       }
       throw new ArgumentException("Unknown aggregate function: " + aggregateFunction);
    }
    /**
     * Returns String representing all aggregated values. Just for debugging purposes.
     * @return String containing all aggregated values
     */
    public String dump() {
        return "MIN=" + Util.formatDouble(min) + ", MAX=" + Util.formatDouble(max) + "\n" +
                "FIRST=" + Util.formatDouble(first) + ", LAST=" + Util.formatDouble(last) + "\n" +
                "AVERAGE=" + Util.formatDouble(average) + ", TOTAL=" + Util.formatDouble(total);
	}
}
}
