using System;
using System.Collections.Generic;
using System.Text;

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


    abstract public class DataSource
    {
        private readonly String name;
        protected double[] values;
        protected long[] timestamps;

        public DataSource(String name)
        {
            this.name = name;
        }

        public String getName()
        {
            return name;
        }

        public void setValues(double[] values)
        {
            this.values = values;
        }

        virtual public void setTimestamps(long[] timestamps)
        {
            this.timestamps = timestamps;
        }

        public double[] getValues()
        {
            return values;
        }

        public  long[] getTimestamps()
        {
            return timestamps;
        }

        virtual public Aggregates getAggregates(long tStart, long tEnd)
        {
            Aggregator agg = new Aggregator(timestamps, values);
            return agg.getAggregates(tStart, tEnd);
        }

        public virtual double getPercentile(long tStart, long tEnd, double percentile)
        {
            Aggregator agg = new Aggregator(timestamps, values);
            return agg.getPercentile(tStart, tEnd, percentile);
        }
    }
}
