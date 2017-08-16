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
using System;
using System.Text;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using rrd4n.Common.Time;

namespace rrd4n.Common
{
   public class Util
   {
      public static readonly long ticksFromEpoc = 621355968000000000L;
      public static readonly long MAX_LONG = long.MaxValue;
      public static readonly long MIN_LONG = -long.MaxValue;

      public static readonly double MAX_DOUBLE = Double.MaxValue;
      public static readonly double MIN_DOUBLE = -Double.MaxValue;

      static readonly CultureInfo df = CultureInfo.CreateSpecificCulture("en-GB");
      static Util()
      {
         NumberFormatInfo nfi = new NumberFormatInfo();
         nfi.NaNSymbol = "NaN";
         df.NumberFormat = nfi;
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

      public static long getTimestamp()
      {
         // round to whole seconds, ignore milliseconds
         // ToDo: Check for rounding
         return (DateTime.Now.Ticks - ticksFromEpoc) / TimeSpan.TicksPerSecond;
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
      public static long getTimestamp(String atStyleTimeSpec)
      {
         TimeSpec timeSpec = new TimeParser(atStyleTimeSpec).parse();
         return timeSpec.getTimestamp();
      }

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
      public static long[] getTimestamps(String atStyleTimeSpec1, String atStyleTimeSpec2)
      {
         TimeSpec timeSpec1 = new TimeParser(atStyleTimeSpec1).parse();
         TimeSpec timeSpec2 = new TimeParser(atStyleTimeSpec2).parse();
         return TimeSpec.getTimestamps(timeSpec1, timeSpec2);
      }


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
            value = double.Parse(valueStr,df);
         }
         catch (FormatException)
         {
            value = Double.NaN;
         }
         return value;
      }

      public static long ParseDateTime(string timeText)
      {
         if (timeText.Contains("-")
            || timeText.Contains(":"))
         {
            DateTime dateTime = DateTime.Parse(timeText);
            return getTimestamp(dateTime);
         }
         return long.Parse(timeText);
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
         return double.TryParse(s,NumberStyles.AllowDecimalPoint,df, out tmp);
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



      static bool sameFilePath(String path1, String path2)
      {
         return Path.GetDirectoryName(path1).CompareTo(Path.GetDirectoryName(path2)) == 0;
      }


      static String getTmpFilename()
      {
         return Path.GetTempFileName();
      }

      static readonly String ISO_DATE_FORMAT = "yyyy-MM-dd HH:mm:ss";   // ISO


      static public double[] Arraycopy(double[] srcValues, int srcIndex, int count)
      {
         double[] dtsValues = new double[count];
         for (int i = 0; i < count; i++)
            dtsValues[i] = srcValues[i + srcIndex];
         return dtsValues;
      }
      static public double[] FillArray(double fillValue, int count)
      {
         double[] values = new double[count];
         for (int i = 0; i < count; i++)
            values[i] = fillValue;
         return values;
      }




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
         String fmt = format.Replace("([^%]|^)%([^a-zA-Z%]*)l(f|g|e|d)", "$1%$2$3");
         fmt = Regex.Replace(format, "([^%]|^)%([^a-zA-Z%]*)l(f|g|e|d)", "$1%$2$3");
         Match match = Regex.Match(fmt, "([^%]*)(%([^a-zA-Z%]*)(f|g|e|d))(.*)");
         StringBuilder sb = new StringBuilder();
         for (var i = 1; i < match.Groups.Count; i++)
         {
            string caption = match.Groups[i].Value;
            if (!caption.StartsWith("%"))
            {
               sb.Append(caption);
            }
            else
            {
               i++;
               string[] formatspecs = match.Groups[i].Value.Split('.');
               i++;
               sb.AppendFormat("{{0,{0}:{1}{2}}}", formatspecs[0], match.Groups[i], formatspecs[1]);
            }
         }
         return String.Format(sb.ToString(), args);
      }

   }
}
