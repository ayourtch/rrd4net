﻿/* ============================================================
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
using rrd4n.Common;

namespace rrd4n.Core
{

    /**
     * Class to represent single data source definition within the RRD.
     * Datasource definition consists of the following five elements:
     * <p/>
     * <ul>
     * <li>data source name
     * <li>data soruce type
     * <li>heartbeat
     * <li>minimal value
     * <li>maximal value
     * </ul>
     * <p>For the complete explanation of all source definition parameters, see RRDTool's
     * <a href="../../../../man/rrdcreate.html" target="man">rrdcreate man page</a>.</p>
     *
     * @author Mikael Nilsson
     */
    public class DsDef
    {
        public static readonly String FORCE_ZEROS_FOR_NANS_SUFFIX = "!";

        private DsType dsType;
        private long heartbeat;
        private double minValue, maxValue;
       public string DsName { get; set; }
        /**
         * <p>Creates new data source definition object. This object should be passed as argument
         * to {@link RrdDef#addDatasource(DsDef) addDatasource()}
         * method of {@link RrdDb RrdDb} object.</p>
         * <p/>
         * <p>For the complete explanation of all source definition parameters, see RRDTool's
         * <a href="../../../../man/rrdcreate.html" target="man">rrdcreate man page</a></p>
         * <p/>
         * <p><b>IMPORTANT NOTE:</b> If datasource name ends with '!', corresponding archives will never
         * store NaNs as datasource values. In that case, NaN datasource values will be silently
         * replaced with zeros by the framework.</p>
         *
         * @param dsName    Data source name.
         * @param dsType    Data source type. Valid values are "COUNTER", "GAUGE", "DERIVE"
         *                  and "ABSOLUTE" (these string constants are conveniently defined in the
         *                  {@link org.Rrd4n.DsType} class).
         * @param heartbeat Hearbeat
         * @param minValue  Minimal value. Use <code>Double.NaN</code> if unknown.
         * @param maxValue  Maximal value. Use <code>Double.NaN</code> if unknown.
         */
        public DsDef(String dsName, DsType dsType, long heartbeat, double minValue, double maxValue)
        {
            if (dsName == null)
            {
                throw new ArgumentException("Null datasource name specified");
            }
            if (dsName.Length == 0)
            {
                throw new ArgumentException("Datasource name length equal to zero");
            }
            if (dsName.Length > RrdPrimitive.STRING_LENGTH)
            {
                throw new ArgumentException("Datasource name [" + dsName + "] to long (" +
                        dsName.Length + " chars found, only " + RrdPrimitive.STRING_LENGTH + " allowed");
            }
            if (dsType == null)
            {
                throw new ArgumentException("Null datasource type specified");
            }
            if (heartbeat <= 0)
            {
                throw new ArgumentException("Invalid heartbeat, must be positive: " + heartbeat);
            }
            if (!Double.IsNaN(minValue) && !Double.IsNaN(maxValue) && minValue >= maxValue)
            {
                throw new ArgumentException("Invalid min/max values specified: " +
                        minValue + "/" + maxValue);
            }

            this.DsName = dsName;
            this.dsType = dsType;
            this.heartbeat = heartbeat;
            this.minValue = minValue;
            this.maxValue = maxValue;
        }

        /**
         * Returns data source name.
         *
         * @return Data source name.
         */
        public String getDsName()
        {
            return DsName;
        }

        /**
         * Returns source type.
         *
         * @return Source type ("COUNTER", "GAUGE", "DERIVE" or "ABSOLUTE").
         */
        public DsType getDsType()
        {
            return dsType;
        }

        /**
         * Returns source heartbeat.
         *
         * @return Source heartbeat.
         */
        public long getHeartbeat()
        {
            return heartbeat;
        }

        /**
         * Returns minimal calculated source value.
         *
         * @return Minimal value.
         */
        public double getMinValue()
        {
            return minValue;
        }

        /**
         * Returns maximal calculated source value.
         *
         * @return Maximal value.
         */
        public double getMaxValue()
        {
            return maxValue;
        }

        /**
         * Returns string representing source definition (RRDTool format).
         *
         * @return String containing all data source definition parameters.
         */
        public String dump()
        {
            return "DS:" + DsName + ":" + dsType + ":" + heartbeat +
                    ":" + Util.formatDouble(minValue, "U", false) +
                    ":" + Util.formatDouble(maxValue, "U", false);
        }

        /**
         * Checks if two datasource definitions are equal.
         * Source definitions are treated as equal if they have the same source name.
         * It is not possible to create RRD with two equal archive definitions.
         *
         * @param obj Archive definition to compare with.
         * @return <code>true</code> if archive definitions are equal,
         *         <code>false</code> otherwise.
         */
        public bool equals(Object obj) 
        {
            if (obj.GetType() == typeof(DsDef) )
            {
                DsDef dsObj = (DsDef) obj;
                return DsName.CompareTo(dsObj.DsName) == 0;
            }
            return false;
        }

        public bool exactlyEqual(DsDef def)
        {
            return (DsName.CompareTo(def.DsName) == 0) && dsType.Dt == def.dsType.Dt &&
                    heartbeat == def.heartbeat && Util.equal(minValue, def.minValue) &&
                    Util.equal(maxValue, def.maxValue);
        }
    }
}
