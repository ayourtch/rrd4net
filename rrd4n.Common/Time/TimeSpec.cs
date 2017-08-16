using System;

namespace rrd4n.Common.Time
{
   public class TimeSpec
   {
      static readonly int TYPE_ABSOLUTE = 0;
      internal static readonly int TYPE_START = 1;
      internal static readonly int TYPE_END = 2;

      internal int type = TYPE_ABSOLUTE;
      internal int year;
      internal int month;
      internal int day;
      internal int hour;
      internal int min;
      internal int sec;
      internal int wday;
      internal int dyear;
      internal int dmonth;
      internal int dday;
      internal int dhour;
      internal int dmin;
      public int dsec;

      String dateString;

      TimeSpec context;

      internal TimeSpec(String dateString)
      {
         this.dateString = dateString;
      }

      internal void localtime(long timestamp)
      {

         DateTime date = new DateTime(timestamp * TimeSpan.TicksPerSecond);
         year = date.Year - 1900;
         month = date.Month;
         day = date.Day;
         hour = date.Hour;
         min = date.Minute;
         sec = date.Second;
         wday = date.DayOfWeek - DayOfWeek.Sunday;
      }

      DateTime getTime()
      {
         DateTime gc;
         // absoulte time, this is easy
         if (type == TYPE_ABSOLUTE)
         {
            gc = new DateTime(year + 1900, month, day, hour, min, sec);
         }
            // relative time, we need a context to evaluate it
         else if (context != null && context.type == TYPE_ABSOLUTE)
         {
            gc = context.getTime();
         }
            // how would I guess what time it was?
         else
            throw new ApplicationException("Relative times like '" + dateString +
                                           "' require proper absolute context to be evaluated");
         gc = gc.AddYears(dyear);
         gc = gc.AddMonths(dmonth);
         gc = gc.AddDays(dday);
         gc = gc.AddHours(dhour);
         gc = gc.AddMinutes(dmin);
         gc = gc.AddSeconds(dsec);
         return gc;
      }
      public void RemoveDays(int days)
      {
         DateTime dt = getTime().AddDays(-days);
         localtime(dt.Ticks / TimeSpan.TicksPerSecond);
      }

      public void AddDays(int days)
      {
         DateTime dt = getTime().AddDays(days);
         localtime(dt.Ticks/TimeSpan.TicksPerSecond);
      }

      /**
       * Returns the corresponding timestamp (seconds since Epoch). Example:<p>
       * <pre>
       * TimeParser p = new TimeParser("now-1day");
       * TimeSpec ts = p.parse();
       * System.out.println("Timestamp was: " + ts.getTimestamp();
       * </pre>
       * @return Timestamp (in seconds, no milliseconds)
       */
      public long getTimestamp()
      {
         return Util.getTimestamp(getTime());
      }

      public String dump()
      {
         return (type == TYPE_ABSOLUTE ? "ABSTIME" : type == TYPE_START ? "START" : "END") +
                ": " + year + "/" + month + "/" + day +
                "/" + hour + "/" + min + "/" + sec + " (" +
                dyear + "/" + dmonth + "/" + dday +
                "/" + dhour + "/" + dmin + "/" + dsec + ")";
      }

      /**
       * Use this static method to resolve relative time references and obtain the corresponding
       * Calendar objects. Example:<p>
       * <pre>
       * TimeParser pStart = new TimeParser("now-1month"); // starting time
       * TimeParser pEnd = new TimeParser("start+1week");  // ending time
       * TimeSpec specStart = pStart.parse();
       * TimeSpec specEnd = pEnd.parse();
       * GregorianCalendar[] gc = TimeSpec.getTimes(specStart, specEnd);
       * </pre>
       * @param spec1 Starting time specification
       * @param spec2 Ending time specification
       * @return Two element array containing Calendar objects
       */
      public static DateTime[] getTimes(TimeSpec spec1, TimeSpec spec2)
      {
         if (spec1.type == TYPE_START || spec2.type == TYPE_END)
         {
            throw new ArgumentException("Recursive time specifications not allowed");
         }
         spec1.context = spec2;
         spec2.context = spec1;
         return new[] {
                         spec1.getTime(),
                         spec2.getTime()
                      };
      }

      /**
       * Use this static method to resolve relative time references and obtain the corresponding
       * timestamps (seconds since epoch). Example:<p>
       * <pre>
       * TimeParser pStart = new TimeParser("now-1month"); // starting time
       * TimeParser pEnd = new TimeParser("start+1week");  // ending time
       * TimeSpec specStart = pStart.parse();
       * TimeSpec specEnd = pEnd.parse();
       * long[] ts = TimeSpec.getTimestamps(specStart, specEnd);
       * </pre>
       * @param spec1 Starting time specification
       * @param spec2 Ending time specification
       * @return array containing two timestamps (in seconds since epoch)
       */
      public static long[] getTimestamps(TimeSpec spec1, TimeSpec spec2)
      {
         DateTime[] gcs = getTimes(spec1, spec2);
         return new long[] {
                              Util.getTimestamp(gcs[0]), Util.getTimestamp(gcs[1])
                           };
      }
   }
}