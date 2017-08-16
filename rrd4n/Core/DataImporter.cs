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

    abstract public class DataImporter
    {

        // header
        abstract public String getVersion();
        abstract public long getLastUpdateTime();
        abstract public long getStep();
        abstract public int getDsCount();
        abstract public int getArcCount();

        // datasource
        abstract public String getDsName(int dsIndex);
        abstract public String getDsType(int dsIndex);
        abstract public long getHeartbeat(int dsIndex);
        abstract public double getMinValue(int dsIndex);
        abstract public double getMaxValue(int dsIndex);

        // datasource state
        abstract public double getLastValue(int dsIndex);
        abstract public double getAccumValue(int dsIndex);
        abstract public long getNanSeconds(int dsIndex);

        // archive
        abstract public ConsolFun getConsolFun(int arcIndex);
        abstract public double getXff(int arcIndex);
        abstract public int getSteps(int arcIndex);
        abstract public int getRows(int arcIndex);

        // archive state
        abstract public double getStateAccumValue(int arcIndex, int dsIndex);
        abstract public int getStateNanSteps(int arcIndex, int dsIndex);
        abstract public double[] getValues(int arcIndex, int dsIndex);

        public long getEstimatedSize()
        {
            int dsCount = getDsCount();
            int arcCount = getArcCount();
            int rowCount = 0;
            for (int i = 0; i < arcCount; i++)
            {
                rowCount += getRows(i);
            }
            return RrdDef.calculateSize(dsCount, arcCount, rowCount);
        }

        public void release()
        {
            // NOP
        }

    }
}
