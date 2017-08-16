using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using rrd4n;
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
     * <p>Class to represent definition of new Round Robin Database (RRD).
     * Object of this class is used to create
     * new RRD from scratch - pass its reference as a <code>RrdDb</code> constructor
     * argument (see documentation for {@link org.Rrd4n.core.RrdDb RrdDb} class). <code>RrdDef</code>
     * object <b>does not</b> actually create new RRD. It just holds all necessary
     * information which will be used during the actual creation process</p>
     *
     * <p>RRD definition (RrdDef object) consists of the following elements:</p>
     *
     * <ul>
     * <li> path to RRD that will be created
     * <li> starting timestamp
     * <li> step
     * <li> one or more datasource definitions
     * <li> one or more archive definitions
     * </ul>
     * <p>RrdDef provides API to set all these elements. For the complete explanation of all
     * RRD definition parameters, see RRDTool's
     * <a href="../../../../man/rrdcreate.html" target="man">rrdcreate man page</a>.</p>
     *
     * @author Mikael Nilsson
     */
    public class RrdDef
    {
        /** Default RRD step to be used if not specified in constructor (300 seconds). */
        public static readonly long DEFAULT_STEP = 300L;

        /** If not specified in constructor, starting timestamp will be set to the
         * current timestamp plus DEFAULT_INITIAL_SHIFT seconds (-10). */
        public static readonly long DEFAULT_INITIAL_SHIFT = -10L;

        private long startTime;
        private long step;

        private List<DsDef> dsDefs = new List<DsDef>();
        private List<ArcDef> arcDefs = new List<ArcDef>();
        public String Path { get; set; }


        /**
         * Creates new RRD definition object with the given path, starting timestamp
         * and step.
         *
         * @param path Path to new RRD.
         * @param startTime RRD starting timestamp.
         * @param step RRD step.
         */
        public RrdDef(String path, DateTime startTime, long step)
            : this(path, Util.getTimestamp(startTime), step)
        {
        }

        
        /**
         * Creates new RRD definition object with the given path, starting timestamp
         * and step.
         *
         * @param path Path to new RRD.
         * @param startTime RRD starting timestamp.
         * @param step RRD step.
         */
        public RrdDef(String path, long startTime, long step)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("No path specified");

            this.Path = path;
            if (startTime < 0)
                throw new ArgumentException("Invalid RRD start time specified: " + startTime);
            this.step = step;
            this.startTime = startTime;
        }

        /**
         * Returns path for the new RRD
         * @return path to the new RRD which should be created
         */
        public String getPath()
        {
            return Path;
        }

        /**
         * Returns starting timestamp for the RRD that should be created.
         * @return RRD starting timestamp
         */
        public long getStartTime()
        {
            return startTime;
        }

        public DateTime getStartDateTime()
        {
           return Util.ConvertToDateTime(startTime);
        }
        /**
         * Returns time step for the RRD that will be created.
         * @return RRD step
         */
        public long getStep()
        {
            return step;
        }

        public TimeSpan getStepTimeSpan()
        {
           return Util.ConvertToTimeSpan(getStep());
        }
        /**
         * Sets path to RRD.
         * @param path to new RRD.
         */
        public void setPath(String path)
        {
            this.Path = path;
        }

        /**
         * Sets RRD's starting timestamp.
         * @param startTime starting timestamp.
         */
        public void setStartTime(long startTime)
        {
            this.startTime = startTime;
        }

        /**
         * Sets RRD's starting timestamp.
         * @param date starting date
         */
        public void setStartTime(DateTime date)
        {
            this.startTime = Util.getTimestamp(date);
        }


        /**
         * Sets RRD's time step.
         * @param step RRD time step.
         */
        public void setStep(long step)
        {
            this.step = step;
        }

        public void setStepTimeSpan(TimeSpan step)
        {
           this.step = (long)step.TotalSeconds;
        }
        /**
         * Adds single datasource definition represented with object of class <code>DsDef</code>.
         * @param dsDef Datasource definition.
         */
        public void addDatasource(DsDef dsDef)
        {
            if (dsDefs.Contains(dsDef))
            {
                throw new ArgumentException("Datasource already defined: " + dsDef.dump());
            }
            dsDefs.Add(dsDef);
        }

        /**
         * <p>Adds single datasource to RRD definition by specifying its data source name, source type,
         * heartbeat, minimal and maximal value. For the complete explanation of all data
         * source definition parameters see RRDTool's
         * <a href="../../../../man/rrdcreate.html" target="man">rrdcreate man page</a>.</p>
         *
         * <p><b>IMPORTANT NOTE:</b> If datasource name ends with '!', corresponding archives will never
         * store NaNs as datasource values. In that case, NaN datasource values will be silently
         * replaced with zeros by the framework.</p>
         *
         * @param dsName Data source name.
         * @param dsType Data source type. Valid types are "COUNTER",
         * "GAUGE", "DERIVE" and "ABSOLUTE" (these string constants are conveniently defined in
         * the {@link DsType} class).
         * @param heartbeat Data source heartbeat.
         * @param minValue Minimal acceptable value. Use <code>Double.NaN</code> if unknown.
         * @param maxValue Maximal acceptable value. Use <code>Double.NaN</code> if unknown.
         * @throws ArgumentException Thrown if new datasource definition uses already used data
         * source name.
         */
        public void addDatasource(String dsName, DsType dsType, long heartbeat, double minValue, double maxValue)
        {
            addDatasource(new DsDef(dsName, dsType, heartbeat, minValue, maxValue));
        }

        /**
         * Adds single datasource to RRD definition from a RRDTool-like
         * datasource definition string. The string must have six elements separated with colons
         * (:) in the following order:<p>
         * <pre>
         * DS:name:type:heartbeat:minValue:maxValue
         * </pre>
         * For example:</p>
         * <pre>
         * DS:input:COUNTER:600:0:U
         * </pre>
         * For more information on datasource definition parameters see <code>rrdcreate</code>
         * man page.<p>
         * @param rrdToolDsDef Datasource definition string with the syntax borrowed from RRDTool.
         * @throws ArgumentException Thrown if invalid string is supplied.
         */
        public void addDatasource(String rrdToolDsDef)
        {
            String[] tokens = rrdToolDsDef.Split(':');
            if (tokens.Length != 6)
                throw new ArgumentException("Wrong rrdtool-like datasource definition: " + rrdToolDsDef + ". Wrong number of elements");

            if (tokens[0].ToUpper().CompareTo("DS") != 0)
            {
                throw new ArgumentException("Wrong rrdtool-like datasource definition: " + rrdToolDsDef + ". No data source");
            }
            String dsName = tokens[1];
            DsType dsType = new DsType(DsType.ValueOf(tokens[2]));
            long dsHeartbeat;
            try
            {
                dsHeartbeat = long.Parse(tokens[3]);
            }
            catch (FormatException nfe)
            {
                throw new ArgumentException("Wrong rrdtool-like datasource definition: " + rrdToolDsDef,nfe);
            }
            double minValue = Double.NaN;
            if (tokens[4].ToUpper().CompareTo("U") != 0)
            {
                try
                {
                    minValue = Double.Parse(tokens[4]);
                }
                catch (FormatException nfe)
                {
                    throw new ArgumentException("Wrong rrdtool-like datasource definition: " + rrdToolDsDef, nfe);
                }
            }
            double maxValue = Double.NaN;
            if (tokens[5].ToUpper().CompareTo("U") != 0)
            {
                try
                {
                    maxValue = Double.Parse(tokens[5]);
                }
                catch (FormatException nfe)
                {
                    throw new ArgumentException("Wrong rrdtool-like datasource definition: " + rrdToolDsDef, nfe);
                }
            }
            addDatasource(new DsDef(dsName, dsType, dsHeartbeat, minValue, maxValue));
        }

        /**
         * Adds data source definitions to RRD definition in bulk.
         * @param dsDefs Array of data source definition objects.
         */
        public void addDatasource(DsDef[] dsDefs)
        {
            foreach (DsDef dsDef in dsDefs)
            {
                addDatasource(dsDef);
            }
        }

        /**
         * Adds single archive definition represented with object of class <code>ArcDef</code>.
         * @param arcDef Archive definition.
         * @throws ArgumentException Thrown if archive with the same consolidation function
         * and the same number of steps is already added.
         */
        public void addArchive(ArcDef arcDef)
        {
            if (arcDefs.Contains(arcDef))
            {
                throw new ArgumentException("Archive already defined: " + arcDef.dump());
            }
            arcDefs.Add(arcDef);
        }

        /**
         * Adds archive definitions to RRD definition in bulk.
         * @param arcDefs Array of archive definition objects
         * @throws ArgumentException Thrown if RRD definition already contains archive with
         * the same consolidation function and the same number of steps.
         */
        public void addArchive(ArcDef[] arcDefs)
        {
            foreach (ArcDef arcDef in arcDefs)
            {
                addArchive(arcDef);
            }
        }

        /**
         * Adds single archive definition by specifying its consolidation function, X-files factor,
         * number of steps and rows. For the complete explanation of all archive
         * definition parameters see RRDTool's
         * <a href="../../../../man/rrdcreate.html" target="man">rrdcreate man page</a>.</p>
         * @param consolFun Consolidation function.
         * @param xff X-files factor. Valid values are between 0 and 1.
         * @param steps Number of archive steps
         * @param rows Number of archive rows
         * @throws ArgumentException Thrown if archive with the same consolidation function
         * and the same number of steps is already added.
         */
        public void addArchive(ConsolFun consolFun, double xff, int steps, int rows)
        {
            addArchive(new ArcDef(consolFun, xff, steps, rows));
        }

        /**
         * Adds single archive to RRD definition from a RRDTool-like
         * archive definition string. The string must have five elements separated with colons
         * (:) in the following order:<p>
         * <pre>
         * RRA:consolidationFunction:XFilesFactor:steps:rows
         * </pre>
         * For example:</p>
         * <pre>
         * RRA:AVERAGE:0.5:10:1000
         * </pre>
         * For more information on archive definition parameters see <code>rrdcreate</code>
         * man page.<p>
         * @param rrdToolArcDef Archive definition string with the syntax borrowed from RRDTool.
         * @throws ArgumentException Thrown if invalid string is supplied.
         */
        public void addArchive(String rrdToolArcDef)
        {

            String[] tokens = rrdToolArcDef.Split(':');
            if (tokens.Length != 5)
                throw new ArgumentException("Wrong rrdtool-like archive definition: " + rrdToolArcDef);

            if (tokens[0].ToUpper().CompareTo("RRA") != 0)
                throw new ArgumentException("Wrong rrdtool-like archive definition: " + rrdToolArcDef);

            ConsolFun consolFun = new ConsolFun(ConsolFun.ValueOf(tokens[1]));
            double xff;
            try
            {
                xff = Double.Parse(tokens[2]);
            }
            catch (FormatException nfe)
            {
                throw new ArgumentException("Wrong rrdtool-like archive definition: " + rrdToolArcDef, nfe);
            }
            int steps;
            try
            {
                steps = int.Parse(tokens[3]);
            }
            catch (FormatException nfe)
            {
                throw new ArgumentException("Wrong rrdtool-like archive definition: " + rrdToolArcDef, nfe);
            }
            int rows;
            try
            {
                rows = int.Parse(tokens[4]);
            }
            catch (FormatException nfe)
            {
                throw new ArgumentException("Wrong rrdtool-like archive definition: " + rrdToolArcDef, nfe);
            }
            addArchive(new ArcDef(consolFun, xff, steps, rows));
        }

        /**
         * Returns all data source definition objects specified so far.
         * @return Array of data source definition objects
         */
        public DsDef[] getDsDefs()
        {
            return dsDefs.ToArray();
        }

        /**
         * Returns all archive definition objects specified so far.
         * @return Array of archive definition objects.
         */
        public ArcDef[] getArcDefs()
        {
           ArcDef[] archives = new ArcDef[arcDefs.Count];
           for (var i = 0; i < arcDefs.Count; i++ )
           {
              archives[i] = arcDefs[i];
           }
           return archives;
        }

        /**
         * Returns number of defined datasources.
         * @return Number of defined datasources.
         */
        public int getDsCount()
        {
            return dsDefs.Count;
        }

        /**
         * Returns number of defined archives.
         * @return Number of defined archives.
         */
        public int getArcCount()
        {
            return arcDefs.Count;
        }

        /**
         * Returns string that represents all specified RRD creation parameters. Returned string
         * has the syntax of RRDTool's <code>create</code> command.
         * @return Dumped content of <code>RrdDb</code> object.
         */
        public String dump()
        {
            StringBuilder buffer = new StringBuilder("create ");
            buffer.AppendLine(System.IO.Path.GetFileNameWithoutExtension(Path));
            buffer.Append(" --start ").AppendFormat("\"{0}\"",Util.ConvertToDateTime(getStartTime())).AppendLine();
            buffer.Append(" --step ").Append(getStep().ToString()).AppendLine(" ");
            foreach (DsDef dsDef in dsDefs)
            {
                buffer.Append(dsDef.dump()).AppendLine(" ");
            }
            foreach (ArcDef arcDef in arcDefs)
            {
                buffer.Append(arcDef.dump()).AppendLine(" ");
            }
            return buffer.ToString().Trim();
        }

        String getRrdToolCommand()
        {
            return dump();
        }

        void removeDatasource(String dsName)
        {
            for (int i = 0; i < dsDefs.Count; i++)
            {
                DsDef dsDef = dsDefs[i];
                if (dsDef.getDsName().CompareTo(dsName) == 0)
                {
                    dsDefs.RemoveAt(i);
                    return;
                }
            }
            throw new ArgumentException("Could not find datasource named '" + dsName + "'");
        }

        void saveSingleDatasource(String dsName)
        {
            List<DsDef> keepList = new List<DsDef>();
            foreach (var ds in dsDefs)
            {
                if (ds.getDsName().CompareTo(dsName) == 0)
                    keepList.Add(ds);
            }
            dsDefs = keepList;
        }

        void removeArchive(ConsolFun consolFun, int steps)
        {
            ArcDef arcDef = findArchive(consolFun, steps);
            if (!arcDefs.Remove(arcDef))
            {
                throw new ArgumentException("Could not remove archive " + consolFun + "/" + steps);
            }
        }
        public bool ContainsArchive(ArcDef otherArcDef)
        {
           foreach (ArcDef arcDef in arcDefs)
           {
              if (arcDef.getConsolFun().CSType == otherArcDef.getConsolFun().CSType 
                 && arcDef.getSteps() == otherArcDef.getSteps())
                 return true;
           }
           return false;
        }

        public ArcDef findArchive(ConsolFun consolFun, int steps)
        {
            foreach (ArcDef arcDef in arcDefs)
            {
                if (arcDef.getConsolFun().Name.CompareTo(consolFun.Name) == 0 && arcDef.getSteps() == steps)
                {
                    return arcDef;
                }
            }
            throw new ArgumentException("Could not find archive " + consolFun + "/" + steps);
        }

        /**
         * Exports RrdDef object to output stream in XML format. Generated XML code can be parsed
         * with {@link RrdDefTemplate} class.
         * @param out Output stream
         */
        //public void exportXmlTemplate(OutputStream outStream)
        //{
            //XmlWriter xml = new XmlWriter(outStream);
            //xml.startTag("rrd_def");
            //xml.writeTag("path", getPath());
            //xml.writeTag("step", getStep());
            //xml.writeTag("start", getStartTime());
            //// datasources
            //DsDef[] dsDefs = getDsDefs();
            //for (DsDef dsDef : dsDefs) {
            //    xml.startTag("datasource");
            //    xml.writeTag("name", dsDef.getDsName());
            //    xml.writeTag("type", dsDef.getDsType());
            //    xml.writeTag("heartbeat", dsDef.getHeartbeat());
            //    xml.writeTag("min", dsDef.getMinValue(), "U");
            //    xml.writeTag("max", dsDef.getMaxValue(), "U");
            //    xml.closeTag(); // datasource
            //}
            //ArcDef[] arcDefs = getArcDefs();
            //for (ArcDef arcDef : arcDefs) {
            //    xml.startTag("archive");
            //    xml.writeTag("cf", arcDef.getConsolFun());
            //    xml.writeTag("xff", arcDef.getXff());
            //    xml.writeTag("steps", arcDef.getSteps());
            //    xml.writeTag("rows", arcDef.getRows());
            //    xml.closeTag(); // archive
            //}
            //xml.closeTag(); // rrd_def
            //xml.flush();
        //}

        /**
         * Exports RrdDef object to string in XML format. Generated XML string can be parsed
         * with {@link RrdDefTemplate} class.
         * @return XML formatted string representing this RrdDef object
         */
        public String exportXmlTemplate()
        {
            //ByteArrayOutputStream out = new ByteArrayOutputStream();
            //exportXmlTemplate(out);
            //return out.ToString();
            return null;
        }

        /**
         * Exports RrdDef object to a file in XML format. Generated XML code can be parsed
         * with {@link RrdDefTemplate} class.
         * @param filePath Path to the file
         */
        public void exportXmlTemplate(String filePath)
        {
            //FileOutputStream out = new FileOutputStream(filePath, false);
            //exportXmlTemplate(out);
            //out.close();
        }

        /**
         * Returns the number of storage bytes required to create RRD from this
         * RrdDef object.
         * @return Estimated byte count of the underlying RRD storage.
         */
        public long getEstimatedSize()
        {
            int dsCount = dsDefs.Count;
            int arcCount = arcDefs.Count;
            int rowsCount = 0;
            foreach (ArcDef arcDef in arcDefs)
            {
                rowsCount += arcDef.getRows();
            }
            return calculateSize(dsCount, arcCount, rowsCount);
        }

        public static long calculateSize(int dsCount, int arcCount, int rowsCount)
        {
            // return 64L + 128L * dsCount + 56L * arcCount +
            //	20L * dsCount * arcCount + 8L * dsCount * rowsCount;
            return (24L + 48L * dsCount + 16L * arcCount +
                20L * dsCount * arcCount + 8L * dsCount * rowsCount) +
                (1L + 2L * dsCount + arcCount) * 2L * RrdPrimitive.STRING_LENGTH;
        }

        /**
         * Compares the current RrdDef with another. RrdDefs are considered equal if:<p>
         *<ul>
         * <li>RRD steps match
         * <li>all datasources have exactly the same definition in both RrdDef objects (datasource names,
         * types, heartbeat, min and max values must match)
         * <li>all archives have exactly the same definition in both RrdDef objects (archive consolidation
         * functions, X-file factors, step and row counts must match)
         * </ul>
         * @param obj The second RrdDef object
         * @return true if RrdDefs match exactly, false otherwise
         */
        public bool equals(Object obj) {
		if(obj == null || (obj.GetType() !=  typeof (RrdDef))) {
			return false;
		}
		RrdDef rrdDef2 = (RrdDef) obj;
		// check primary RRD step
		if(step != rrdDef2.step) {
			return false;
		}
		// check datasources
		DsDef[] dsDefs = getDsDefs(), dsDefs2 = rrdDef2.getDsDefs();
		if(dsDefs.Length != dsDefs2.Length) {
			return false;
		}
        foreach (DsDef dsDef in dsDefs) {
            bool matched = false;
            foreach (DsDef aDsDefs2 in dsDefs2) {
                if (dsDef.exactlyEqual(aDsDefs2)) {
                    matched = true;
                    break;
                }
            }
            // this datasource could not be matched
            if (!matched) {
                return false;
            }
        }
		// check archives
		ArcDef[] arcDefs = getArcDefs(), arcDefs2 = rrdDef2.getArcDefs();
		if(arcDefs.Length != arcDefs2.Length) {
			return false;
		}
        foreach (ArcDef arcDef in arcDefs) {
            bool matched = false;
            foreach (ArcDef anArcDefs2 in arcDefs2) {
                if (arcDef.exactlyEqual(anArcDefs2)) {
                    matched = true;
                    break;
                }
            }
            // this archive could not be matched
            if (!matched) {
                return false;
            }
        }
		// everything matches
		return true;
	}

        public bool hasDatasources()
        {
            return dsDefs.Count != 0;
        }

        public bool hasArchives()
        {
            return arcDefs.Count != 0;
        }

        /**
         * Removes all datasource definitions.
         */
        public void removeDatasources()
        {
            dsDefs.Clear();
        }

        /**
         * Removes all RRA archive definitions.
         */
        public void removeArchives()
        {
            arcDefs.Clear();
        }
    }
}
