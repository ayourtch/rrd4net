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
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using rrd4n.Common;

namespace rrd4n.Core
{


    /**
     * Class defines various utility functions used in Rrd4j.
     *
     * @author Sasa Markovic
     */
    public class Util
    {
        public static readonly long ticksFromEpoc = 621355968000000000L;
        public static readonly long MAX_LONG = long.MaxValue;
        public static readonly long MIN_LONG = -long.MaxValue;

        public static readonly double MAX_DOUBLE = Double.MaxValue;
        public static readonly double MIN_DOUBLE = -Double.MaxValue;

        // pattern RRDTool uses to format doubles in XML files
        static readonly String PATTERN = "0.0000000000E00";
        // directory under $USER_HOME used for demo graphs storing
        static readonly String RRD4J_DIR = "rrd4j-demo";

        static readonly CultureInfo df = CultureInfo.CreateSpecificCulture("en-GB");
        static Util()
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NaNSymbol = "NaN";
            df.NumberFormat = nfi;
        }



        //df = 
        // static {
        //     df = (DecimalFormat) NumberFormat.getNumberInstance(Locale.ENGLISH);
        //     df.applyPattern(PATTERN);
        //     df.setPositivePrefix("+");
        // }

        /**
         * Converts an array of long primitives to an array of doubles.
         *
         * @return Same array but with all values as double.
         */
        public static double[] toDoubleArray(long[] array)
        {
            double[] values = new double[array.Length];
            for (int i = 0; i < array.Length; i++)
                values[i] = array[i];
            return values;
        }

        /**
         * Returns current timestamp in seconds (without milliseconds). Returned timestamp
         * is obtained with the following expression: <p>
         * <p/>
         * <code>(System.currentTimeMillis() + 500L) / 1000L</code>
         *
         * @return Current timestamp
         */
        public static long getTime()
        {
            throw new ApplicationException("Check this upp. Why do we end up here?");
            //return (DateTime.Now.Ticks + 500L) / 1000L;
        }

        /**
         * Just an alias for {@link #getTime()} method.
         *
         * @return Current timestamp (without milliseconds)
         */
        public static long getTimestamp()
        {
            return getTime();
        }

        /**
         * Rounds the given timestamp to the nearest whole &quote;step&quote;. Rounded value is obtained
         * from the following expression:<p>
         * <code>timestamp - timestamp % step;</code><p>
         *
         * @param timestamp Timestamp in seconds
         * @param step      Step in seconds
         * @return "Rounded" timestamp
         */
        public static long normalize(long timestamp, long step)
        {
            return timestamp - timestamp % step;
        }

        /**
         * Returns the greater of two double values, but treats NaN as the smallest possible
         * value. Note that <code>Math.Max()</code> behaves differently for NaN arguments.
         *
         * @param x an argument
         * @param y another argument
         * @return the lager of arguments
         */
        public static double max(double x, double y)
        {
            return Double.IsNaN(x) ? y : Double.IsNaN(y) ? x : Math.Max(x, y);
        }

        /**
         * Returns the smaller of two double values, but treats NaN as the greatest possible
         * value. Note that <code>Math.Min()</code> behaves differently for NaN arguments.
         *
         * @param x an argument
         * @param y another argument
         * @return the smaller of arguments
         */
        public static double min(double x, double y)
        {
            return Double.IsNaN(x) ? y : Double.IsNaN(y) ? x : Math.Min(x, y);
        }

        /**
         * Calculates sum of two doubles, but treats NaNs as zeros.
         *
         * @param x First double
         * @param y Second double
         * @return Sum(x,y) calculated as <code>Double.IsNaN(x)? y: Double.IsNaN(y)? x: x + y;</code>
         */
        public static double sum(double x, double y)
        {
            return Double.IsNaN(x) ? y : Double.IsNaN(y) ? x : x + y;
        }

        public static String formatDouble(double x, String nanString, bool forceExponents)
        {
            if (Double.IsNaN(x))
            {
                return nanString;
            }
            if (forceExponents)
            {
                return x.ToString(df);
            }
            return "" + x;
        }

        public static String formatDouble(double x, bool forceExponents)
        {
            return formatDouble(x, "" + Double.NaN, forceExponents);
        }

        /**
         * Formats double as a string using exponential notation (RRDTool like). Used for debugging
         * throught the project.
         *
         * @param x value to be formatted
         * @return string like "+1.234567E+02"
         */
        public static String formatDouble(double x)
        {
            return formatDouble(x, true);
        }

        /**
         * Returns <code>Date</code> object for the given timestamp (in seconds, without
         * milliseconds)
         *
         * @param timestamp Timestamp in seconds.
         * @return Corresponding Date object.
         */
        public static DateTime getDate(long timestamp)
        {
           return new DateTime((timestamp * TimeSpan.TicksPerSecond) + ticksFromEpoc);
        }

        ///**
        // * Returns <code>Calendar</code> object for the given timestamp
        // * (in seconds, without milliseconds)
        // *
        // * @param timestamp Timestamp in seconds.
        // * @return Corresponding Calendar object.
        // */
        //public static Calendar getCalendar(long timestamp) {
        //    Calendar calendar = Calendar.getInstance();
        //    calendar.setTimeInMillis(timestamp * 1000L);
        //    return calendar;
        //}

        ///**
        // * Returns <code>Calendar</code> object for the given Date object
        // *
        // * @param date Date object
        // * @return Corresponding Calendar object.
        // */
        //public static Calendar getCalendar(Date date) {
        //    Calendar calendar = Calendar.getInstance();
        //    calendar.setTime(date);
        //    return calendar;
        //}

        /**
         * Returns timestamp (unix epoch) for the given Date object
         *
         * @param date Date object
         * @return Corresponding timestamp (without milliseconds)
         */
        public static long getTimestamp(DateTime date)
        {
            // round to whole seconds, ignore milliseconds
            // ToDo: Check for rounding
            return (date.Ticks - ticksFromEpoc) / TimeSpan.TicksPerSecond;
        }

        ///**
        // * Returns timestamp (unix epoch) for the given Calendar object
        // *
        // * @param gc Calendar object
        // * @return Corresponding timestamp (without milliseconds)
        // */
        //public static long getTimestamp(Calendar gc) {
        //    return getTimestamp(gc.getTime());
        //}

        /**
         * Returns timestamp (unix epoch) for the given year, month, day, hour and minute.
         *
         * @param year  Year
         * @param month Month (zero-based)
         * @param day   Day in month
         * @param hour  Hour
         * @param min   Minute
         * @return Corresponding timestamp
         */
        public static long getTimestamp(int year, int month, int day, int hour, int min)
        {
            long ti = new DateTime(year, month, day, hour, min, 0).Ticks;
            return (ti - ticksFromEpoc) / TimeSpan.TicksPerSecond;
            //return (new DateTime(year, month, day, hour, min, 0).Ticks - ticksFromEpoc) / 1000000;
        }

        /**
         * Returns timestamp (unix epoch) for the given year, month and day.
         *
         * @param year  Year
         * @param month Month (zero-based)
         * @param day   Day in month
         * @return Corresponding timestamp
         */
        public static long getTimestamp(int year, int month, int day)
        {
            return getTimestamp(year, month, day, 0, 0);
        }

        /**
         * Parses at-style time specification and returns the corresponding timestamp. For example:<p>
         * <pre>
         * long t = Util.getTimestamp("now-1d");
         * </pre>
         *
         * @param atStyleTimeSpec at-style time specification. For the complete explanation of the syntax
         *                        allowed see RRDTool's <code>rrdfetch</code> man page.<p>
         * @return timestamp in seconds since epoch.
         */
        //public static long getTimestamp(String atStyleTimeSpec) {
        //    TimeSpec timeSpec = new TimeParser(atStyleTimeSpec).parse();
        //    return timeSpec.getTimestamp();
        //}

        /**
         * Parses two related at-style time specifications and returns corresponding timestamps. For example:<p>
         * <pre>
         * long[] t = Util.getTimestamps("end-1d","now");
         * </pre>
         *
         * @param atStyleTimeSpec1 Starting at-style time specification. For the complete explanation of the syntax
         *                         allowed see RRDTool's <code>rrdfetch</code> man page.<p>
         * @param atStyleTimeSpec2 Ending at-style time specification. For the complete explanation of the syntax
         *                         allowed see RRDTool's <code>rrdfetch</code> man page.<p>
         * @return An array of two longs representing starting and ending timestamp in seconds since epoch.
         */
        //public static long[] getTimestamps(String atStyleTimeSpec1, String atStyleTimeSpec2) {
        //    TimeSpec timeSpec1 = new TimeParser(atStyleTimeSpec1).parse();
        //    TimeSpec timeSpec2 = new TimeParser(atStyleTimeSpec2).parse();
        //    return TimeSpec.getTimestamps(timeSpec1, timeSpec2);
        //}

        /**
         * Parses input string as a double value. If the value cannot be parsed, Double.NaN
         * is returned (FormatException is never thrown).
         *
         * @param valueStr String representing double value
         * @return a double corresponding to the input string
         */
        public static double parseDouble(String valueStr)
        {
            double value;
            try
            {
                value = double.Parse(valueStr);
            }
            catch (FormatException )
            {
                value = Double.NaN;
            }
            return value;
        }

        /**
         * Checks if a string can be parsed as double.
         *
         * @param s Input string
         * @return <code>true</code> if the string can be parsed as double, <code>false</code> otherwise
         */
        public static bool isDouble(String s)
        {
            double tmp;
            return double.TryParse(s,out tmp);
        }

        /**
         * Parses input string as a bool value. The parser is case insensitive.
         *
         * @param valueStr String representing bool value
         * @return <code>true</code>, if valueStr equals to 'true', 'on', 'yes', 'y' or '1';
         *         <code>false</code> in all other cases.
         */
        public static bool parsebool(String valueStr)
        {
            return valueStr.ToLower().CompareTo("true") == 0 ||
                    valueStr.ToLower().CompareTo("on") == 0 ||
                    valueStr.ToLower().CompareTo("yes") == 0 ||
                    valueStr.ToLower().CompareTo("y") == 0 ||
                    valueStr.ToLower().CompareTo("1") == 0;
        }

        /**
         * Parses input string as color. The color string should be of the form #RRGGBB (no alpha specified,
         * opaque color) or #RRGGBBAA (alpa specified, transparent colors). Leading character '#' is
         * optional.
         *
         * @param valueStr Input string, for example #FFAA24, #AABBCC33, 010203 or ABC13E4F
         * @return Color object
         * @throws ArgumentException If the input string is not 6 or 8 characters long (without optional '#')
         */
        public static Color parseColor(String valueStr)
        {
            String c = valueStr.StartsWith("#") ? valueStr.Substring(1) : valueStr;
            if (c.Length != 6 && c.Length != 8)
            {
                throw new ArgumentException("Invalid color specification: " + valueStr);
            }
            string r = c.Substring(0, 2);
            string g = c.Substring(2, 2);
            string b = c.Substring(4, 2);

            int red = (Int32.Parse(c.Substring(0, 2), NumberStyles.AllowHexSpecifier));
            int green = (Int32.Parse(c.Substring(2, 2), NumberStyles.AllowHexSpecifier));
            int blue = (Int32.Parse(c.Substring(4, 2), NumberStyles.AllowHexSpecifier));

            if (c.Length == 6)
                return Color.FromArgb(red, green, blue);

            String a = c.Substring(6);
            return Color.FromArgb(Int16.Parse(a), red, green, blue);
        }

        /**
         * Returns file system separator string.
         *
         * @return File system separator ("/" on Unix, "\" on Windows)
         */
        public static String getFileSeparator()
        {
            return @"\";
        }

        /**
         * Returns path to user's home directory.
         *
         * @return Path to users home directory, with file separator appended.
         */
        public static String getUserHomeDirectory()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + getFileSeparator();
        }

        /**
         * Returns path to directory used for placement of Rrd4j demo graphs and creates it
         * if necessary.
         *
         * @return Path to demo directory (defaults to $HOME/rrd4j/) if directory exists or
         *         was successfully created. Null if such directory could not be created.
         */
        public static String getRrd4jDemoDirectory()
        {
            String homeDirPath = getUserHomeDirectory() + RRD4J_DIR + getFileSeparator();
            if (Directory.Exists(homeDirPath))
                Directory.CreateDirectory(homeDirPath);
            return homeDirPath;
        }

        /**
         * Returns full path to the file stored in the demo directory of Rrd4j
         *
         * @param filename Partial path to the file stored in the demo directory of Rrd4j
         *                 (just name and extension, without parent directories)
         * @return Full path to the file
         */
        public static String getRrd4jDemoPath(String filename)
        {
            String demoDir = getRrd4jDemoDirectory();
            if (demoDir != null)
            {
                return demoDir + filename;
            }
            else
            {
                return null;
            }
        }

        static bool sameFilePath(String path1, String path2)
        {
            return Path.GetDirectoryName(path1).CompareTo(Path.GetDirectoryName(path2)) == 0;
        }

        public static int getMatchingDatasourceIndex(RrdDb rrd1, int dsIndex, RrdDb rrd2)
        {
            String dsName = rrd1.getDatasource(dsIndex).DsName;
            try
            {
                return rrd2.getDsIndex(dsName);
            }
            catch (ArgumentException e)
            {
                return -1;
            }
        }

        public static int getMatchingArchiveIndex(RrdDb rrd1, int arcIndex, RrdDb rrd2)
        {
            Archive archive = rrd1.getArchive(arcIndex);
            ConsolFun consolFun = archive.getConsolFun();
            int steps = archive.getSteps();
            try
            {
                return rrd2.getArcIndex(consolFun, steps);
            }
            catch (ArgumentException e)
            {
                return -1;
            }
        }

        static String getTmpFilename()
        {
            return Path.GetTempFileName();
        }

        static readonly String ISO_DATE_FORMAT = "yyyy-MM-dd HH:mm:ss";   // ISO


        static public double[] Arraycopy(double[] srcValues, int srcIndex, int count)
        {
            double[] dtsValues = new double[count];
            for(int i = 0; i < count; i++)
                dtsValues[i] = srcValues[i + srcIndex];
            return dtsValues;
        }
        static public double[] FillArray( double fillValue, int count)
        {
    		double[] values = new double[count];
            for (int i = 0; i < count; i++)
                values[i] = fillValue;
            return values;
        }


        
        /**
         * Creates Calendar object from a string. The string should represent
         * either a long integer (UNIX timestamp in seconds without milliseconds,
         * like "1002354657") or a human readable date string in the format "yyyy-MM-dd HH:mm:ss"
         * (like "2004-02-25 12:23:45").
         *
         * @param timeStr Input string
         * @return Calendar object
         */
        //public static Calendar getCalendar(String timeStr)
        //{
        //    // try to parse it as long
        //    try
        //    {
        //        long timestamp = long.Parse(timeStr);
        //        return Util.getCalendar(timestamp);
        //    }
        //    catch (FormatException e)
        //    {
        //    }
        //    // not a long timestamp, try to parse it as data
        //    SimpleDateFormat df = new SimpleDateFormat(ISO_DATE_FORMAT);
        //    df.setLenient(false);
        //    try
        //    {
        //        Date date = df.parse(timeStr);
        //        return Util.getCalendar(date);
        //    }
        //    catch (ParseException e)
        //    {
        //        throw new ArgumentException("Time/date not in " + ISO_DATE_FORMAT +
        //                " format: " + timeStr);
        //    }
        //}

        /**
         * Various DOM utility functions.
         */
        //public static class Xml
        //{
        //    public static Node[] getChildNodes(Node parentNode)
        //    {
        //        return getChildNodes(parentNode, null);
        //    }

        //    public static Node[] getChildNodes(Node parentNode, String childName)
        //    {
        //        ArrayList<Node> nodes = new ArrayList<Node>();
        //        NodeList nodeList = parentNode.getChildNodes();
        //        for (int i = 0; i < nodeList.getLength(); i++)
        //        {
        //            Node node = nodeList.item(i);
        //            if (node.getNodeType() == Node.ELEMENT_NODE && (childName == null || node.getNodeName().equals(childName)))
        //            {
        //                nodes.add(node);
        //            }
        //        }
        //        return nodes.toArray(new Node[0]);
        //    }

        //    public static Node getFirstChildNode(Node parentNode, String childName)
        //    {
        //        Node[] childs = getChildNodes(parentNode, childName);
        //        if (childs.Length > 0)
        //        {
        //            return childs[0];
        //        }
        //        throw new ArgumentException("XML Error, no such child: " + childName);
        //    }

        //    public static bool hasChildNode(Node parentNode, String childName)
        //    {
        //        Node[] childs = getChildNodes(parentNode, childName);
        //        return childs.Length > 0;
        //    }

        //    // -- Wrapper around getChildValue with trim
        //    public static String getChildValue(Node parentNode, String childName)
        //    {
        //        return getChildValue(parentNode, childName, true);
        //    }

        //    public static String getChildValue(Node parentNode, String childName, bool trim)
        //    {
        //        NodeList children = parentNode.getChildNodes();
        //        for (int i = 0; i < children.getLength(); i++)
        //        {
        //            Node child = children.item(i);
        //            if (child.getNodeName().equals(childName))
        //            {
        //                return getValue(child, trim);
        //            }
        //        }
        //        throw new ApplicationException("XML Error, no such child: " + childName);
        //    }

        //    // -- Wrapper around getValue with trim
        //    public static String getValue(Node node)
        //    {
        //        return getValue(node, true);
        //    }

        //    public static String getValue(Node node, bool trimValue)
        //    {
        //        String value = null;
        //        Node child = node.getFirstChild();
        //        if (child != null)
        //        {
        //            value = child.getNodeValue();
        //            if (value != null && trimValue)
        //            {
        //                value = value.trim();
        //            }
        //        }
        //        return value;
        //    }

        //    public static int getChildValueAsInt(Node parentNode, String childName)
        //    {
        //        String valueStr = getChildValue(parentNode, childName);
        //        return Integer.parseInt(valueStr);
        //    }

        //    public static int getValueAsInt(Node node)
        //    {
        //        String valueStr = getValue(node);
        //        return Integer.parseInt(valueStr);
        //    }

        //    public static long getChildValueAsLong(Node parentNode, String childName)
        //    {
        //        String valueStr = getChildValue(parentNode, childName);
        //        return long.Parse(valueStr);
        //    }

        //    public static long getValueAsLong(Node node)
        //    {
        //        String valueStr = getValue(node);
        //        return long.Parse(valueStr);
        //    }

        //    public static double getChildValueAsDouble(Node parentNode, String childName)
        //    {
        //        String valueStr = getChildValue(parentNode, childName);
        //        return Util.parseDouble(valueStr);
        //    }

        //    public static double getValueAsDouble(Node node)
        //    {
        //        String valueStr = getValue(node);
        //        return Util.parseDouble(valueStr);
        //    }

        //    public static bool getChildValueAsbool(Node parentNode, String childName)
        //    {
        //        String valueStr = getChildValue(parentNode, childName);
        //        return Util.parsebool(valueStr);
        //    }

        //    public static bool getValueAsbool(Node node)
        //    {
        //        String valueStr = getValue(node);
        //        return Util.parsebool(valueStr);
        //    }

        //    public static Element getRootElement(InputSource inputSource)
        //    {
        //        DocumentBuilderFactory factory = DocumentBuilderFactory.newInstance();
        //        factory.setValidating(false);
        //        factory.setNamespaceAware(false);
        //        try
        //        {
        //            DocumentBuilder builder = factory.newDocumentBuilder();
        //            Document doc = builder.parse(inputSource);
        //            return doc.getDocumentElement();
        //        }
        //        catch (ParserConfigurationException e)
        //        {
        //            throw new RuntimeException(e.getMessage(), e);
        //        }
        //        catch (SAXException e)
        //        {
        //            throw new RuntimeException(e.getMessage(), e);
        //        }
        //    }

        //    public static Element getRootElement(String xmlString)
        //    {
        //        return getRootElement(new InputSource(new StringReader(xmlString)));
        //    }

        //    public static Element getRootElement(File xmlFile)
        //    {
        //        Reader reader = null;
        //        try
        //        {
        //            reader = new FileReader(xmlFile);
        //            return getRootElement(new InputSource(reader));
        //        }
        //        finally
        //        {
        //            if (reader != null)
        //            {
        //                reader.close();
        //            }
        //        }
        //    }
        //}

        private static long lastLap = DateTime.Now.Millisecond;

        /**
         * Function used for debugging purposes and performance bottlenecks detection.
         * Probably of no use for end users of Rrd4j.
         *
         * @return String representing time in seconds since last
         *         <code>getLapTime()</code> method call.
         */
        public static String getLapTime()
        {
            long newLap = DateTime.Now.Millisecond;
            double seconds = (newLap - lastLap) / 1000.0;
            lastLap = newLap;
            return "[" + seconds + " sec]";
        }

        /**
         * Returns the root directory of the Rrd4j distribution. Useful in some demo applications,
         * probably of no use anywhere else.<p>
         * <p/>
         * The function assumes that all Rrd4j .class files are placed under
         * the &lt;root&gt;/classes subdirectory and that all jars (libraries) are placed in the
         * &lt;root&gt;/lib subdirectory (the original Rrd4j directory structure).<p>
         *
         * @return absolute path to Rrd4j's home directory
         */
        //public static String getRrd4jHomeDirectory() {
        //    String className = Util.class.getName().replace('.', '/');
        //    String uri = Util.class.getResource("/" + className + ".class").ToString();
        //    //System.out.println(uri);
        //    if (uri.startsWith("file:/")) {
        //        uri = uri.substring(6);
        //        File file = new File(uri);
        //        // let's go 5 steps backwards
        //        for (int i = 0; i < 5; i++) {
        //            file = file.getParentFile();
        //        }
        //        uri = file.getAbsolutePath();
        //    }
        //    else if (uri.startsWith("jar:file:/")) {
        //        uri = uri.substring(9, uri.lastIndexOf('!'));
        //        File file = new File(uri);
        //        // let's go 2 steps backwards
        //        for (int i = 0; i < 2; i++) {
        //            file = file.getParentFile();
        //        }
        //        uri = file.getAbsolutePath();
        //    }
        //    else {
        //        uri = null;
        //    }
        //    return uri;
        //}

        /**
         * Compares two doubles but treats all NaNs as equal.
         * In Java (by default) Double.NaN == Double.NaN always returns <code>false</code>
         *
         * @param x the first value
         * @param y the second value
         * @return <code>true</code> if x and y are both equal to Double.NaN, or if x == y. <code>false</code> otherwise
         */
        public static bool equal(double x, double y)
        {
            return (Double.IsNaN(x) && Double.IsNaN(y)) || (x == y);
        }

        /**
         * Returns canonical file path for the given file path
         *
         * @param path Absolute or relative file path
         * @return Canonical file path
         * @ Thrown if canonical file path could not be resolved
         */
        public static String getCanonicalPath(String path)
        {
            return Path.GetFullPath(path);
        }

        /**
         * Returns last modification time for the given file.
         *
         * @param file File object representing file on the disk
         * @return Last modification time in seconds (without milliseconds)
         */
        public static long getLastModified(String file)
        {
            FileInfo fi = new FileInfo(file);

            return (File.GetLastAccessTime(file).Ticks / +50000L) / 100000L;
        }

        /**
         * Checks if the file with the given file name exists
         *
         * @param filename File name
         * @return <code>true</code> if file exists, <code>false</code> otherwise
         */
        public static bool fileExists(String filename)
        {
            return File.Exists(filename);
        }

        /**
         * Finds max value for an array of doubles (NaNs are ignored). If all values in the array
         * are NaNs, NaN is returned.
         *
         * @param values Array of double values
         * @return max value in the array (NaNs are ignored)
         */
        public static double max(double[] values)
        {
            double max = Double.NaN;
            foreach (double value in values)
            {
                max = Util.max(max, value);
            }
            return max;
        }

        /**
         * Finds min value for an array of doubles (NaNs are ignored). If all values in the array
         * are NaNs, NaN is returned.
         *
         * @param values Array of double values
         * @return min value in the array (NaNs are ignored)
         */
        public static double min(double[] values)
        {
            double min = Double.NaN;
            foreach (double value in values)
            {
                min = Util.min(min, value);
            }
            return min;
        }

        public static DateTime ConvertToDateTime(long seconds)
        {
            DateTime dt = new DateTime((seconds * TimeSpan.TicksPerSecond));
            return dt.Add(new TimeSpan(ticksFromEpoc));
        }
        public static TimeSpan ConvertToTimeSpan(long seconds)
        {
           return new TimeSpan(seconds * TimeSpan.TicksPerSecond);
        }


        /**
         * Equivalent of the C-style sprintf function. Sorry, it works only in Java5.
         *
         * @param format Format string
         * @param args   Arbitrary list of arguments
         * @return Formatted string
         */
        public static String sprintf(String format, Object[] args)
        {
           String fmt = format.Replace("([^%]|^)%([^a-zA-Z%]*)l(f|g|e)", "$1%$2$3");
           return String.Format(fmt, args);
        }
    }
}
