using System;
using System.Collections.Generic;
using System.Diagnostics;
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


    class Aggregator
    {
        private readonly long[] timestamps;
        private readonly long step;
        private readonly double[] values;

        public Aggregator(long[] timestamps, double[] values)
        {
            Debug.Assert(timestamps.Length == values.Length, "Incompatible timestamps/values arrays (unequal lengths)");
            Debug.Assert(timestamps.Length >= 2, "At least two timestamps must be supplied");
            this.timestamps = timestamps;
            this.values = values;
            this.step = timestamps[1] - timestamps[0];
        }

        public Aggregates getAggregates(long tStart, long tEnd)
        {
            Aggregates agg = new Aggregates();
            long totalSeconds = 0;
            bool firstFound = false;
            for (int i = 0; i < timestamps.Length; i++)
            {
                long left = Math.Max(timestamps[i] - step, tStart);
                long right = Math.Min(timestamps[i], tEnd);
                long delta = right - left;
                if (delta > 0)
                {
                    double value = values[i];
                    double min = Util.min(agg.Min, value);
                    if (double.IsNaN(agg.Min) || agg.Min > min)
                   {
                      agg.Min = min;
                      agg.MinTimeStamp = timestamps[i];
                   }
                   double max = Util.max(agg.Max, value);
                   if (double.IsNaN(agg.Max) || agg.Max < max)
                   {
                      agg.Max = max;
                      agg.MaxTimeStamp = timestamps[i];
                   }
                    if (!firstFound)
                    {
                        agg.First = value;
                        agg.FirstTimeStamp = timestamps[i];
                        firstFound = true;
                    }
                    agg.Last = value;
                   agg.LastTimeStamp = timestamps[i];
                    if (!Double.IsNaN(value))
                    {
                        agg.Total = Util.sum(agg.Total, delta * value);
                        totalSeconds += delta;
                    }
                }
            }
            agg.Average = totalSeconds > 0 ? (agg.Total / totalSeconds) : Double.NaN;
            return agg;
        }

        public double getPercentile(long tStart, long tEnd, double percentile)
        {
            List<Double> valueList = new List<Double>();
            // create a list of included datasource values (different from NaN)
            for (int i = 0; i < timestamps.Length; i++)
            {
                long left = Math.Max(timestamps[i] - step, tStart);
                long right = Math.Min(timestamps[i], tEnd);
                if (right > left && !Double.IsNaN(values[i]))
                {
                    valueList.Add(values[i]);
                }
            }
            // create an array to work with
            int count = valueList.Count;
            if (count > 1)
            {
                List<double> valuesCopy = new List<double>();
                for (int i = 0; i < count; i++)
                {
                    valuesCopy.Add(valueList[i]);
                }
                // sort array
                valuesCopy.Sort();
                // skip top (100% - percentile) values
                double topPercentile = (100.0 - percentile) / 100.0;
                count -= (int)Math.Ceiling(count * topPercentile);
                // if we have anything left[]
                if (count > 0)
                {
                    return valuesCopy[count - 1];
                }
            }
            // not enough data available
            return Double.NaN;
        }
    }
}
