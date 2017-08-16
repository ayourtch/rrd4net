using System;
using System.Collections.Generic;
using System.Text;
using rrd4n.Common;

namespace rrd4n.Core
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
     * Class to represent single archive definition within the RRD.
     * Archive definition consists of the following four elements:
     *
     * <ul>
     * <li>consolidation function
     * <li>X-files factor
     * <li>number of steps
     * <li>number of rows.
     * </ul>
     * <p>For the complete explanation of all archive definition parameters, see RRDTool's
     * <a href="../../../../man/rrdcreate.html" target="man">rrdcreate man page</a>
     * </p>
     *
     * @author Mikael Nilsson
     */
    public class ArcDef
    {
        private ConsolFun consolFun;
       public int Steps { get; private set; }
       public int Rows { get; private set; }
       public double Xff { get; private set; }
        /**
         * <p>Creates new archive definition object. This object should be passed as argument to
         * {@link RrdDef#addArchive(ArcDef) addArchive()} method of
         * {@link RrdDb RrdDb} object.</p>
         *
         * <p>For the complete explanation of all archive definition parameters, see RRDTool's
         * <a href="../../../../man/rrdcreate.html" target="man">rrdcreate man page</a></p>
         *
         * @param consolFun Consolidation function. Allowed values are "AVERAGE", "MIN",
         *                  "MAX" and "LAST" (these string constants are conveniently defined in the
         *                  {@link ConsolFun} class).
         * @param xff       X-files factor, between 0 and 1.
         * @param steps     Number of archive steps.
         * @param rows      Number of archive rows.
         */
        public ArcDef(ConsolFun consolFun, double xff, int steps, int rows)
        {
            if (consolFun == null)
            {
                throw new ArgumentException("Null consolidation function specified");
            }
            if (Double.IsNaN(xff) || xff < 0.0 || xff >= 1.0)
            {
                throw new ArgumentException("Invalid xff, must be >= 0 and < 1: " + xff);
            }
            if (steps < 1 || rows < 2)
            {
                throw new ArgumentException("Invalid steps/rows settings: " + steps + "/" + rows +
                        ". Minimal values allowed are steps=1, rows=2");
            }

            this.consolFun = consolFun;
            this.Xff = xff;
            this.Steps = steps;
            this.Rows = rows;
        }

        /**
         * Returns consolidation function.
         *
         * @return Consolidation function.
         */
        public ConsolFun getConsolFun()
        {
            return consolFun;
        }

        /**
         * Returns the X-files factor.
         *
         * @return X-files factor value.
         */
        public double getXff()
        {
            return Xff;
        }

        /**
         * Returns the number of primary RRD steps which complete a single archive step.
         *
         * @return Number of steps.
         */
        public int getSteps()
        {
            return Steps;
        }

        /**
         * Returns the number of rows (aggregated values) stored in the archive.
         *
         * @return Number of rows.
         */
        public int getRows()
        {
            return Rows;
        }

        /**
         * Returns string representing archive definition (RRDTool format).
         *
         * @return String containing all archive definition parameters.
         */
        public String dump()
        {
            return "RRA:" + consolFun + ":" + Xff + ":" + Steps + ":" + Rows;
        }

        /**
         * Checks if two archive definitions are equal.
         * Archive definitions are considered equal if they have the same number of steps
         * and the same consolidation function. It is not possible to create RRD with two
         * equal archive definitions.
         *
         * @param obj Archive definition to compare with.
         *
         * @return <code>true</code> if archive definitions are equal,
         *         <code>false</code> otherwise.
         */
        public bool equals(Object obj)
        {
            if (obj.GetType() == typeof(ArcDef))
            {
                ArcDef arcObj = (ArcDef)obj;
                return consolFun.Name.CompareTo(arcObj.consolFun.Name) == 0 && Steps == arcObj.Steps;
            }
            return false;
        }

        public void setRows(int rows)
        {
            this.Rows = rows;
        }

        public bool exactlyEqual(ArcDef def)
        {
            return consolFun.CSType == def.consolFun.CSType && Xff == def.Xff && Steps == def.Steps && Rows == def.Rows;
        }
    }
}
