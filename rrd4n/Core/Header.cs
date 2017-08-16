using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
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


    /**
     * Class to represent RRD header. Header information is mainly static (once set, it
     * cannot be changed), with the exception of last update time (this value is changed whenever
     * RRD gets updated).<p>
     *
     * Normally, you don't need to manipulate the Header object directly - Rrd4n framework
     * does it for you.<p>
     *
     * @author Mikael Nilsson*
     */
    public class Header : RrdUpdater
    {
        static readonly int SIGNATURE_LENGTH = 5;
        static readonly String SIGNATURE = "RRD4N";
        static readonly String J_SIGNATURE = "RRD4J";

        static readonly String DEFAULT_SIGNATURE = SIGNATURE + ", version 0.1";
        static readonly String RRDTOOL_VERSION = "0001";

        private RrdDb parentDb;

        private RrdString signature;
        private RrdLong step;
        private RrdInt dsCount, arcCount;
        private RrdLong lastUpdateTime;

        public Header(RrdDb parentDb, RrdDef rrdDef)
            : this(parentDb, rrdDef, DEFAULT_SIGNATURE)
        {
        }

        public Header(RrdDb parentDb, RrdDef rrdDef, String initSignature)
        {
            this.parentDb = parentDb;

            signature = new RrdString(this);	 		// NOT constant, may be cached
            step = new RrdLong(this, true); 			// constant, may be cached
            dsCount = new RrdInt(this, true); 			// constant, may be cached
            arcCount = new RrdInt(this, true); 			// constant, may be cached
            lastUpdateTime = new RrdLong(this);

            if (rrdDef != null)
            {
                signature.set(initSignature);
                step.set(rrdDef.getStep());
                dsCount.set(rrdDef.getDsCount());
                arcCount.set(rrdDef.getArcCount());
                lastUpdateTime.set(rrdDef.getStartTime());
            }
        }

       public Header(RrdDb parentDb, DataImporter reader)
            : this(parentDb, (RrdDef)null)
        {
            String version = reader.getVersion();
            if (RRDTOOL_VERSION.CompareTo(version) != 0)
            {
                throw new ArgumentException("Could not unserialize xml version " + version);
            }
            signature.set(DEFAULT_SIGNATURE);
            step.set(reader.getStep());
            dsCount.set(reader.getDsCount());
            arcCount.set(reader.getArcCount());
            lastUpdateTime.set(reader.getLastUpdateTime());
        }

        /**
         * Returns RRD signature. Initially, the returned string will be
         * of the form <b><i>Rrd4n, version x.x</i></b>. Note: RRD format did not
         * change since Rrd4n 1.0.0 release (and probably never will).
         *
         * @return RRD signature
         * @Thrown in case of I/O error
         */
        public String getSignature()
        {
            return signature.get();
        }

        public String getInfo()
        {
            return getSignature().Substring(0,SIGNATURE_LENGTH);
        }

        public void setInfo(String info)
        {
            if (info != null && info.Length > 0)
            {
                signature.set(SIGNATURE + info);
            }
            else
            {
                signature.set(SIGNATURE);
            }
        }

        /**
         * Returns the last update time of the RRD.
         *
         * @return Timestamp (Unix epoch, no milliseconds) corresponding to the last update time.
         * @Thrown in case of I/O error
         */
        public long getLastUpdateTime()
        {
            return lastUpdateTime.get();
        }

        /**
         * Returns primary RRD time step.
         *
         * @return Primary time step in seconds
         * @Thrown in case of I/O error
         */
        public long getStep()
        {
            return step.get();
        }

        /**
         * Returns the number of datasources defined in the RRD.
         *
         * @return Number of datasources defined
         * @Thrown in case of I/O error
         */
        public int getDsCount()
        {
            return dsCount.get();
        }

        /**
         * Returns the number of archives defined in the RRD.
         *
         * @return Number of archives defined
         * @Thrown in case of I/O error
         */
        public int getArcCount()
        {
            return arcCount.get();
        }

        public void setLastUpdateTime(long lastUpdateTime)
        {
            this.lastUpdateTime.set(lastUpdateTime);
        }

        public String dump()
        {
            return "== HEADER ==\n" +
                "signature:" + getSignature() +
                " lastUpdateTime:" + getLastUpdateTime() +
                " step:" + getStep() +
                " dsCount:" + getDsCount() +
                " arcCount:" + getArcCount() + "\n";
        }

        public void appendXml(XmlWriter writer)
        {
           writer.WriteComment(signature.get());
           writer.WriteElementString("version", RRDTOOL_VERSION);
           writer.WriteComment("Seconds");
           writer.WriteElementString("step", step.get().ToString());
           writer.WriteComment(Util.getDate(lastUpdateTime.get()).ToString());
           writer.WriteElementString("lastupdate", lastUpdateTime.get().ToString());
        }

        /**
         * Copies object's internal state to another Header object.
         * @param other New Header object to copy state to
         * @Thrown in case of I/O error
         */
        public void copyStateTo(RrdUpdater other)
        {
            if (other.GetType() != typeof(Header))
            {
                throw new ArgumentException(
                    "Cannot copy Header object to " + other.GetType().ToString());
            }
            Header header = (Header)other;
            header.signature.set(signature.get());
            header.lastUpdateTime.set(lastUpdateTime.get());
        }

        /**
         * Returns the underlying storage (backend) object which actually performs all
         * I/O operations.
         * @return I/O backend object
         */
        public RrdBackend getRrdBackend()
        {
            return parentDb.getRrdBackend();
        }

        bool isRrd4nHeader()
        {
           return signature.get().StartsWith(SIGNATURE) || signature.get().StartsWith(J_SIGNATURE) || signature.get().StartsWith("JR"); // backwards compatible with JRobin
        }

        public void validateHeader()
        {
            if (!isRrd4nHeader())
            {
                throw new System.IO.IOException("Invalid file header. File [" + parentDb.getCanonicalPath() + "] is not a Rrd4n RRD file");
            }
        }

        /**
         * Required to implement RrdUpdater interface. You should never call this method directly.
         * @return Allocator object
         */
        public RrdAllocator getRrdAllocator()
        {
            return parentDb.getRrdAllocator();
        }
    }
}
