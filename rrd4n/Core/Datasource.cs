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
     * Class to represent single datasource within RRD. Each datasource object holds the
     * following information: datasource definition (once set, never changed) and
     * datasource state variables (changed whenever RRD gets updated).<p>
     * <p/>
     * Normally, you don't need to manipluate Datasource objects directly, it's up to
     * Rrd4n framework to do it for you.
     *
     * @author Mikael Nilsson
     */
    public class Datasource : RrdUpdater
    {
        private static readonly double MAX_32_BIT = Math.Pow(2, 32);
        private static readonly double MAX_64_BIT = Math.Pow(2, 64);

        private readonly RrdDb parentDb;

        // definition
        private readonly RrdString dsName,dsTypeName;
        private readonly RrdLong heartbeat;
        private readonly RrdDouble minValue, maxValue;

        // state variables
        private RrdDouble lastValue;
        private RrdLong nanSeconds;
        private RrdDouble accumValue;

        private Common.DsType dsType;

        public Datasource(RrdDb parentDb, DsDef dsDef)
        {
            bool shouldInitialize = dsDef != null;
            this.parentDb = parentDb;
            dsName = new RrdString(this);
            dsTypeName = new RrdString(this);
            if (!shouldInitialize) 
               dsType = new DsType(dsTypeName.get());
            heartbeat = new RrdLong(this);
            minValue = new RrdDouble(this);
            maxValue = new RrdDouble(this);
            lastValue = new RrdDouble(this);
            accumValue = new RrdDouble(this);
            nanSeconds = new RrdLong(this);
            if (shouldInitialize)
            {
                dsName.set(dsDef.getDsName());
                dsType = dsDef.getDsType();
                dsTypeName.set(dsType.Name);
                heartbeat.set(dsDef.getHeartbeat());
                minValue.set(dsDef.getMinValue());
                maxValue.set(dsDef.getMaxValue());
                lastValue.set(Double.NaN);
                accumValue.set(0.0);
                Header header = parentDb.getHeader();
                nanSeconds.set(header.getLastUpdateTime() % header.getStep());
            }
        }

        public Datasource(RrdDb parentDb, DataImporter reader, int dsIndex)
            : this(parentDb, null)
        {

            dsName.set(reader.getDsName(dsIndex));
            dsType = new DsType(reader.getDsType(dsIndex));
            dsTypeName.set(dsType.Name);
            heartbeat.set(reader.getHeartbeat(dsIndex));
            minValue.set(reader.getMinValue(dsIndex));
            maxValue.set(reader.getMaxValue(dsIndex));
            lastValue.set(reader.getLastValue(dsIndex));
            accumValue.set(reader.getAccumValue(dsIndex));
            nanSeconds.set(reader.getNanSeconds(dsIndex));
        }

        public String dump()
        {
            return "== DATASOURCE ==\n" +
                    "DS:" + dsName.get() + ":" + dsType.Name + ":" +
                    heartbeat.get() + ":" + minValue.get() + ":" +
                    maxValue.get() + "\nlastValue:" + lastValue.get() +
                    " nanSeconds:" + nanSeconds.get() +
                    " accumValue:" + accumValue.get() + "\n";
        }

        /**
         * Returns datasource name.
         *
         * @return Datasource name
         * @ Thrown in case of I/O error
         */
        public String DsName
        {
            get {return dsName.get();}
        }

        /**
         * Returns datasource type (GAUGE, COUNTER, DERIVE, ABSOLUTE).
         *
         * @return Datasource type.
         * @ Thrown in case of I/O error
         */
        public Common.DsType DsType
        {
            get {return dsType;}
        }

        /**
         * Returns datasource heartbeat
         *
         * @return Datasource heartbeat
         * @ Thrown in case of I/O error
         */

        public long Heartbeat
        {
            get { return heartbeat.get(); }
        }

        /**
         * Returns mimimal allowed value for this datasource.
         *
         * @return Minimal value allowed.
         * @ Thrown in case of I/O error
         */
        public double MinValue
        {
            get { return minValue.get(); }
        }

        /**
         * Returns maximal allowed value for this datasource.
         *
         * @return Maximal value allowed.
         * @ Thrown in case of I/O error
         */
        public double MaxValue
        {
            get { return maxValue.get(); }
        }

        /**
         * Returns last known value of the datasource.
         *
         * @return Last datasource value.
         * @ Thrown in case of I/O error
         */
        public double LastValue
        {
            get { return lastValue.get(); }
        }

        /**
         * Returns value this datasource accumulated so far.
         *
         * @return Accumulated datasource value.
         * @ Thrown in case of I/O error
         */
        public double AccumValue
        {
            get { return accumValue.get(); }
        }

        /**
         * Returns the number of accumulated NaN seconds.
         *
         * @return Accumulated NaN seconds.
         * @ Thrown in case of I/O error
         */
        public long NanSeconds
        {
            get { return nanSeconds.get(); }
        }

        public void process(long newTime, double newValue)
        {
            Header header = parentDb.getHeader();
            long step = header.getStep();
            long oldTime = header.getLastUpdateTime();
            long startTime = Util.normalize(oldTime, step);
            long endTime = startTime + step;
            double oldValue = lastValue.get();
            double updateValue = calculateUpdateValue(oldTime, oldValue, newTime, newValue);
            if (newTime < endTime)
            {
                accumulate(oldTime, newTime, updateValue);
            }
            else
            {
                // should store something
                long boundaryTime = Util.normalize(newTime, step);
                accumulate(oldTime, boundaryTime, updateValue);
                double value = calculateTotal(startTime, boundaryTime);

                // how many updates?
                long numSteps = (boundaryTime - endTime) / step + 1L;

                // ACTION!
                parentDb.archive(this, value, numSteps);

                // cleanup
                nanSeconds.set(0);
                accumValue.set(0.0);

                accumulate(boundaryTime, newTime, updateValue);
            }
        }

        private double calculateUpdateValue(long oldTime, double oldValue,
                                            long newTime, double newValue)
        {
            double updateValue = Double.NaN;
            if (newTime - oldTime <= heartbeat.get())
            {
                if (dsType.Dt == DsType.DsTypes.GAUGE)
                {
                    updateValue = newValue;
                }
                else if (dsType.Dt == DsType.DsTypes.COUNTER)
                {
                    if (!Double.IsNaN(newValue) && !Double.IsNaN(oldValue))
                    {
                        double diff = newValue - oldValue;
                        if (diff < 0)
                        {
                            diff += MAX_32_BIT;
                        }
                        if (diff < 0)
                        {
                            diff += MAX_64_BIT - MAX_32_BIT;
                        }
                        if (diff >= 0)
                        {
                            updateValue = diff / (newTime - oldTime);
                        }
                    }
                }
                else if (dsType.Dt == DsType.DsTypes.ABSOLUTE)
                {
                    if (!Double.IsNaN(newValue))
                    {
                        updateValue = newValue / (newTime - oldTime);
                    }
                }
                else if (dsType.Dt == DsType.DsTypes.DERIVE)
                {
                    if (!Double.IsNaN(newValue) && !Double.IsNaN(oldValue))
                    {
                        updateValue = (newValue - oldValue) / (newTime - oldTime);
                    }
                }

                if (!Double.IsNaN(updateValue))
                {
                    double minVal = minValue.get();
                    double maxVal = maxValue.get();
                    if (!Double.IsNaN(minVal) && updateValue < minVal)
                    {
                        updateValue = Double.NaN;
                    }
                    if (!Double.IsNaN(maxVal) && updateValue > maxVal)
                    {
                        updateValue = Double.NaN;
                    }
                }
            }
            lastValue.set(newValue);
            return updateValue;
        }

        private void accumulate(long oldTime, long newTime, double updateValue)
        {
            if (Double.IsNaN(updateValue))
            {
                nanSeconds.set(nanSeconds.get() + (newTime - oldTime));
            }
            else
            {
                accumValue.set(accumValue.get() + updateValue * (newTime - oldTime));
            }
        }

        private double calculateTotal(long startTime, long boundaryTime)
        {
            double totalValue = Double.NaN;
            long validSeconds = boundaryTime - startTime - nanSeconds.get();
            if (nanSeconds.get() <= heartbeat.get() && validSeconds > 0)
            {
                totalValue = accumValue.get() / validSeconds;
            }
            // IMPORTANT:
            // if datasource name ends with "!", we'll send zeros instead of NaNs
            // this might be handy from time to time
            if (Double.IsNaN(totalValue) && dsName.get().EndsWith(DsDef.FORCE_ZEROS_FOR_NANS_SUFFIX))
            {
                totalValue = 0D;
            }
            return totalValue;
        }

        public void appendXml(XmlWriter writer)
        {
            writer.WriteStartElement("ds");
            writer.WriteElementString("name", dsName.get());
            writer.WriteElementString("type", dsTypeName.get());
            writer.WriteElementString("minimal_heartbeat", heartbeat.get().ToString());
            writer.WriteElementString("min", minValue.get().ToString());
            writer.WriteElementString("max", maxValue.get().ToString());
            writer.WriteComment("PDP Status");
            writer.WriteElementString("last_ds", lastValue.get().ToString());
            writer.WriteElementString("value", accumValue.get().ToString());
            writer.WriteElementString("unknown_sec", nanSeconds.get().ToString());
            writer.WriteEndElement();  // ds
        }

        /**
         * Copies object's internal state to another Datasource object.
         *
         * @param other New Datasource object to copy state to
         * @ Thrown in case of I/O error
         */
        [Obsolete]
        public void copyStateTo(RrdUpdater other)
        {
            if (!(other.GetType() == typeof(Datasource)))
            {
                throw new ArgumentException(
                        "Cannot copy Datasource object to " + other.GetType().ToString());
            }
            Datasource datasource = (Datasource)other;
            if (datasource.dsName.get().CompareTo(dsName.get()) != 0)
            {
                throw new ArgumentException("Incompatible datasource names");
            }
            if (datasource.DsType.Name.CompareTo(dsType.Name) != 0)
            {
                throw new ArgumentException("Incompatible datasource types");
            }
            datasource.lastValue.set(lastValue.get());
            datasource.nanSeconds.set(nanSeconds.get());
            datasource.accumValue.set(accumValue.get());
        }

        /**
         * Returns index of this Datasource object in the RRD.
         *
         * @return Datasource index in the RRD.
         * @ Thrown in case of I/O error
         */
        public int DsIndex
        {
            get {return parentDb.getDsIndex(dsName.get());}
        }

        /**
         * Sets datasource heartbeat to a new value.
         *
         * @param heartbeat New heartbeat value
         * @              Thrown in case of I/O error
         * @throws ArgumentException Thrown if invalid (non-positive) heartbeat value is specified.
         */
        public void setHeartbeat(long heartbeat)
        {
            if (heartbeat < 1L)
            {
                throw new ArgumentException("Invalid heartbeat specified: " + heartbeat);
            }
            this.heartbeat.set(heartbeat);
        }

        /**
         * Sets datasource name to a new value
         *
         * @param newDsName New datasource name
         * @ Thrown in case of I/O error
         */
        public void setDsName(String newDsName)
        {
            if (newDsName != null && newDsName.Length > RrdString.STRING_LENGTH)
            {
                throw new ArgumentException("Invalid datasource name specified: " + newDsName);
            }
            if (parentDb.containsDs(newDsName))
            {
                throw new ArgumentException("Datasource already defined in this RRD: " + newDsName);
            }

            this.dsName.set(newDsName);
        }

        public void setDsType(Common.DsType newDsType)
        {
            dsType = newDsType;
            dsTypeName.set(dsType.Name);

            // set datasource type
            // reset datasource status
            lastValue.set(Double.NaN);
            accumValue.set(0.0);
            // reset archive status
            int dsIndex = parentDb.getDsIndex(dsName.get());
            Archive[] archives = parentDb.getArchives();
            foreach (Archive archive in archives)
            {
                archive.getArcState(dsIndex).setAccumValue(Double.NaN);
            }
        }

        /**
         * Sets minimum allowed value for this datasource. If <code>filterArchivedValues</code>
         * argment is set to true, all archived values less then <code>minValue</code> will
         * be fixed to NaN.
         *
         * @param minValue             New minimal value. Specify <code>Double.NaN</code> if no minimal
         *                             value should be set
         * @param filterArchivedValues true, if archived datasource values should be fixed;
         *                             false, otherwise.
         * @              Thrown in case of I/O error
         * @throws ArgumentException Thrown if invalid minValue was supplied (not less then maxValue)
         */
        public void setMinValue(double minValue, bool filterArchivedValues)
        {
            double maxValue = this.maxValue.get();
            if (!Double.IsNaN(minValue) && !Double.IsNaN(maxValue) && minValue >= maxValue)
            {
                throw new ArgumentException("Invalid min/max values: " + minValue + "/" + maxValue);
            }

            this.minValue.set(minValue);
            if (!Double.IsNaN(minValue) && filterArchivedValues)
            {
                int dsIndex = DsIndex;
                Archive[] archives = parentDb.getArchives();
                foreach (Archive archive in archives)
                {
                    archive.getRobin(dsIndex).filterValues(minValue, Double.NaN);
                }
            }
        }

        /**
         * Sets maximum allowed value for this datasource. If <code>filterArchivedValues</code>
         * argment is set to true, all archived values greater then <code>maxValue</code> will
         * be fixed to NaN.
         *
         * @param maxValue             New maximal value. Specify <code>Double.NaN</code> if no max
         *                             value should be set.
         * @param filterArchivedValues true, if archived datasource values should be fixed;
         *                             false, otherwise.
         * @              Thrown in case of I/O error
         * @throws ArgumentException Thrown if invalid maxValue was supplied (not greater then minValue)
         */
        public void setMaxValue(double maxValue, bool filterArchivedValues)
        {
            double minValue = this.minValue.get();
            if (!Double.IsNaN(minValue) && !Double.IsNaN(maxValue) && minValue >= maxValue)
            {
                throw new ArgumentException("Invalid min/max values: " + minValue + "/" + maxValue);
            }

            this.maxValue.set(maxValue);
            if (!Double.IsNaN(maxValue) && filterArchivedValues)
            {
                int dsIndex = DsIndex;
                Archive[] archives = parentDb.getArchives();
                foreach (Archive archive in archives)
                {
                    archive.getRobin(dsIndex).filterValues(Double.NaN, maxValue);
                }
            }
        }

        /**
         * Sets min/max values allowed for this datasource. If <code>filterArchivedValues</code>
         * argment is set to true, all archived values less then <code>minValue</code> or
         * greater then <code>maxValue</code> will be fixed to NaN.
         *
         * @param minValue             New minimal value. Specify <code>Double.NaN</code> if no min
         *                             value should be set.
         * @param maxValue             New maximal value. Specify <code>Double.NaN</code> if no max
         *                             value should be set.
         * @param filterArchivedValues true, if archived datasource values should be fixed;
         *                             false, otherwise.
         * @              Thrown in case of I/O error
         * @throws ArgumentException Thrown if invalid min/max values were supplied
         */
        public void setMinMaxValue(double minValue, double maxValue, bool filterArchivedValues)
        {
            if (!Double.IsNaN(minValue) && !Double.IsNaN(maxValue) && minValue >= maxValue)
            {
                throw new ArgumentException("Invalid min/max values: " + minValue + "/" + maxValue);
            }
            this.minValue.set(minValue);
            this.maxValue.set(maxValue);
            if (!(Double.IsNaN(minValue) && Double.IsNaN(maxValue)) && filterArchivedValues)
            {
                int dsIndex = DsIndex;
                Archive[] archives = parentDb.getArchives();
                foreach (Archive archive in archives)
                {
                    archive.getRobin(dsIndex).filterValues(minValue, maxValue);
                }
            }
        }

        /**
         * Returns the underlying storage (backend) object which actually performs all
         * I/O operations.
         *
         * @return I/O backend object
         */
        public RrdBackend getRrdBackend()
        {
            return parentDb.getRrdBackend();
        }

        /**
         * Required to implement RrdUpdater interface. You should never call this method directly.
         *
         * @return Allocator object
         */
        public RrdAllocator getRrdAllocator()
        {
            return parentDb.getRrdAllocator();
        }
    }

}
