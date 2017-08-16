using System;
using System.Collections.Generic;
using System.Text;
using rrd4n.Common;

namespace rrd4n.Core
{
/* ============================================================
 * Rrd4j : Pure java implementation of RRDTool's functionality
 * ============================================================
 *
 * Project Info:  http://www.rrd4j.org
 * Project Lead:  Mathias Bogaert (m.bogaert@memenco.com)
 *
 * (C) Copyright 2003-2007, by Sasa Markovic.
 *
 * Developers:    Sasa Markovic
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
 * <p>Class to represent data source values for the given timestamp. Objects of this
 * class are never created directly (no public constructor is provided). To learn more how
 * to update RRDs, see RRDTool's
 * <a href="../../../../man/rrdupdate.html" target="man">rrdupdate man page</a>.
 * <p/>
 * <p>To update a RRD with Rrd4j use the following procedure:</p>
 * <p/>
 * <ol>
 * <li>Obtain empty Sample object by calling method {@link RrdDb#createSample(long)
 * createSample()} on respective {@link RrdDb RrdDb} object.
 * <li>Adjust Sample timestamp if necessary (see {@link #setTime(long) setTime()} method).
 * <li>Supply data source values (see {@link #setValue(String, double) setValue()}).
 * <li>Call Sample's {@link #update() update()} method.
 * </ol>
 * <p/>
 * <p>Newly created Sample object contains all data source values set to 'unknown'.
 * You should specifify only 'known' data source values. However, if you want to specify
 * 'unknown' values too, use <code>Double.NaN</code>.</p>
 *
 * @author Sasa Markovic
 */
public class Sample {
    private  RrdDb parentDb;
    private long time;
    private  String[] dsNames;
    private  double[] values;

    public Sample(RrdDb parentDb, long time) {
        this.parentDb = parentDb;
        this.time = time;

        this.dsNames = parentDb.getDsNames();
        clearValues();
    }

    public Sample(string[] dsNames, long time)
    {
       this.parentDb = null;
       this.time = time;

       this.dsNames = dsNames;
       clearValues();
    }

   private Sample clearValues()
    {

        values = Util.FillArray(double.NaN, dsNames.Length);
        return this;
    }

    /**
     * Sets single data source value in the sample.
     *
     * @param dsName Data source name.
     * @param value  Data source value.
     * @return This <code>Sample</code> object
     * @throws ArgumentException Thrown if invalid data source name is supplied.
     */
    public Sample setValue(String dsName, double value) {
        for (int i = 0; i < values.Length; i++) {
            if (dsNames[i].CompareTo(dsName) == 0) {
                values[i] = value;
                return this;
            }
        }
        throw new ArgumentException("Datasource " + dsName + " not found");
    }

    /**
     * Sets single datasource value using data source index. Data sources are indexed by
     * the order specified during RRD creation (zero-based).
     *
     * @param i     Data source index
     * @param value Data source values
     * @return This <code>Sample</code> object
     * @throws ArgumentException Thrown if data source index is invalid.
     */
    public Sample setValue(int i, double value) {
        if (i < values.Length) {
            values[i] = value;
            return this;
        }
        throw new ArgumentException("Sample datasource index " + i + " out of bounds");
    }

    /**
     * Sets some (possibly all) data source values in bulk. Data source values are
     * assigned in the order of their definition inside the RRD.
     *
     * @param values Data source values.
     * @return This <code>Sample</code> object
     * @throws ArgumentException Thrown if the number of supplied values is zero or greater
     *                                  than the number of data sources defined in the RRD.
     */
    public Sample setValues(double[] values) {
        if (values.Length <= this.values.Length) {
            for (int i = 0; i < values.Length; i++)
                this.values[i] = values[i];
            return this;
        }
        throw new ArgumentException("Invalid number of values specified (found " +
                values.Length + ", only " + dsNames.Length + " allowed)");
    }

    /**
     * Returns all current data source values in the sample.
     *
     * @return Data source values.
     */
    public double[] getValues() {
        return values;
    }

    /**
     * Returns sample timestamp (in seconds, without milliseconds).
     *
     * @return Sample timestamp.
     */
    public long getTime() {
        return time;
    }

    /**
     * Sets sample timestamp. Timestamp should be defined in seconds (without milliseconds).
     *
     * @param time New sample timestamp.
     * @return This <code>Sample</code> object
     */
    public Sample setTime(long time) {
        this.time = time;
        return this;
    }

    public Sample setDateTime(DateTime timeStamp)
    {
       this.time = Util.getTimestamp(timeStamp);
       return this;
    }
    /**
     * Returns an array of all data source names. If you try to set value for the data source
     * name not in this array, an exception is thrown.
     *
     * @return Acceptable data source names.
     */
   [Obsolete]
    private  String[] getDsNames() {
        return dsNames;
    }

    /**
     * <p>Sets sample timestamp and data source values in a fashion similar to RRDTool.
     * Argument string should be composed in the following way:
     * <code>timestamp:value1:value2:[]:valueN</code>.</p>
     * <p/>
     * <p>You don't have to supply all datasource values. Unspecified values will be treated
     * as unknowns. To specify unknown value in the argument string, use letter 'U'
     *
     * @param timeAndValues String made by concatenating sample timestamp with corresponding
     *                      data source values delmited with colons. For example:<p>
     *                      <pre>
     *                      1005234132:12.2:35.6:U:24.5
     *                      NOW:12.2:35.6:U:24.5
     *                      </pre>
     *                      'N' stands for the current timestamp (can be replaced with 'NOW')<p>
     *                      Method will throw an exception if timestamp is invalid (cannot be parsed as Long, and is not 'N'
     *                      or 'NOW'). Datasource value which cannot be parsed as 'double' will be silently set to NaN.<p>
     * @return This <code>Sample</code> object
     * @throws ArgumentException Thrown if too many datasource values are supplied
     */
   [Obsolete]
    private Sample set(String timeAndValues) {
        string[] argumentTokens = timeAndValues.Split(':');
        //StringTokenizer tokenizer = new StringTokenizer(timeAndValues, ":", false);
        //int n = tokenizer.countTokens();
        if (argumentTokens.Length > values.Length + 1)
        {
            throw new ArgumentException("Invalid number of values specified (found " +
                    values.Length + ", " + dsNames.Length + " allowed)");
        }
        String timeToken = argumentTokens[0];
        try {
            time = long.Parse(timeToken);
        }
        catch (FormatException nfe) {
            if (timeToken.ToUpper().CompareTo("N") == 0 || timeToken.ToUpper().CompareTo("NOW") == 0) {
                time = Util.getTimestamp();
            }
            else {
                throw new ArgumentException("Invalid sample timestamp: " + timeToken, nfe);
            }
        }
        // Startt from index 1 [0] = timestamp
        for (int i = 1; i < argumentTokens.Length; i++)
        {
            try {
                values[i - 1] = Double.Parse(argumentTokens[i]);
            }
            catch (FormatException ) {
                // NOP, value is already set to NaN
            }
        }
        return this;
    }

    /**
     * Stores sample in the corresponding RRD. If the update operation succeedes,
     * all datasource values in the sample will be set to Double.NaN (unknown) values.
     *
     * @Thrown in case of I/O error.
     */
    public void update() {
        parentDb.store(this);
        clearValues();
    }

    /**
     * <p>Creates sample with the timestamp and data source values supplied
     * in the argument string and stores sample in the corresponding RRD.
     * This method is just a shortcut for:</p>
     * <pre>
     *     set(timeAndValues);
     *     update();
     * </pre>
     *
     * @param timeAndValues String made by concatenating sample timestamp with corresponding
     *                      data source values delmited with colons. For example:<br>
     *                      <code>1005234132:12.2:35.6:U:24.5</code><br>
     *                      <code>NOW:12.2:35.6:U:24.5</code>
     * @Thrown in case of I/O error.
     */
   [Obsolete]
    private void setAndUpdate(String timeAndValues) {
        set(timeAndValues);
        update();
    }

    /**
     * Dumps sample content using the syntax of RRDTool's update command.
     *
     * @return Sample dump.
     */
    public String dump() {
        StringBuilder buffer = new StringBuilder("update \"");
        buffer.Append(parentDb.getRrdBackend().getPath()).Append("\" ").Append(time);
        foreach (double value in values) {
            buffer.Append(":");
            buffer.Append(Util.formatDouble(value, "U", false));
        }
        return buffer.ToString();
    }

   [Obsolete]
    private String getRrdToolCommand() {
        return dump();
    }
}
}
