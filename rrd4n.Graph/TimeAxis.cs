using System;
using System.Globalization;
using System.Drawing;
using System.Text.RegularExpressions;
using rrd4n.Common;

namespace rrd4n.Graph
{
   /* ============================================================
    * Rrd4n : Pure c# implementation of RRDTool's functionality
    * ============================================================
    *
    * Project Info:  http://minidev.se
    * Project Lead:  Mikael Nilsson (info@minidev.se)
    *
    * Developers:    Mikael Nilsson
    *
    *
    * (C) Copyright 2009-2010, by Mikael Nilsson.
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

   class TimeAxis : RrdGraphConstants
   {
      private static TimeAxisSetting[] tickSettings = {
		new TimeAxisSetting(0, SECOND, 30, MINUTE, 5, MINUTE, 5, 0, "HH:mm"),
		new TimeAxisSetting(2, MINUTE, 1, MINUTE, 5, MINUTE, 5, 0, "HH:mm"),
		new TimeAxisSetting(5, MINUTE, 2, MINUTE, 10, MINUTE, 10, 0, "HH:mm"),
		new TimeAxisSetting(10, MINUTE, 5, MINUTE, 20, MINUTE, 20, 0, "HH:mm"),
		new TimeAxisSetting(30, MINUTE, 10, HOUR, 1, HOUR, 1, 0, "HH:mm"),
		new TimeAxisSetting(60, MINUTE, 30, HOUR, 2, HOUR, 2, 0, "HH:mm"),
		new TimeAxisSetting(180, HOUR, 1, HOUR, 6, HOUR, 6, 0, "HH:mm"),
		new TimeAxisSetting(600, HOUR, 6, DAY, 1, DAY, 1, 24 * 3600, "EEE"),
		new TimeAxisSetting(1800, HOUR, 12, DAY, 1, DAY, 2, 24 * 3600, "EEE"),
		new TimeAxisSetting(3600, DAY, 1, WEEK, 1, WEEK, 1, 7 * 24 * 3600, "'Week 'w"),
		new TimeAxisSetting(3 * 3600, WEEK, 1, MONTH, 1, WEEK, 2, 7 * 24 * 3600, "'Week 'w"),
		new TimeAxisSetting(6 * 3600, MONTH, 1, MONTH, 1, MONTH, 1, 30 * 24 * 3600, "MMM"),
		new TimeAxisSetting(48 * 3600, MONTH, 1, MONTH, 3, MONTH, 3, 30 * 24 * 3600, "MMM"),
		new TimeAxisSetting(10 * 24 * 3600, YEAR, 1, YEAR, 1, YEAR, 1, 365 * 24 * 3600, "yy"),
		new TimeAxisSetting(-1, MONTH, 0, MONTH, 0, MONTH, 0, 0, "")
	};

      private TimeAxisSetting tickSetting;
      private RrdGraph rrdGraph;
      private double secPerPix;
      private DateTime calendar;

      internal TimeAxis(RrdGraph rrdGraph)
      {
         this.rrdGraph = rrdGraph;
         this.secPerPix = (rrdGraph.im.end - rrdGraph.im.start) / (double)rrdGraph.im.xsize;
         //this.calendar = Calendar.getInstance(Locale.getDefault());
         //this.calendar.setFirstDayOfWeek(rrdGraph.gdef.firstDayOfWeek);
      }

      internal void draw()
      {
         chooseTickSettings();
         // early return, avoid exceptions
         if (tickSetting == null) return;

         drawMinor();
         drawMajor();
         drawLabels();
      }

      private void drawMinor()
      {
         if (!rrdGraph.gdef.noMinorGrid)
         {
            adjustStartingTime(tickSetting.minorUnit, tickSetting.minorUnitCount);
            Color color = rrdGraph.gdef.colors[COLOR_GRID];
            int y0 = rrdGraph.im.yorigin, y1 = y0 - rrdGraph.im.ysize;
            for (int status = getTimeShift(); status <= 0; status = getTimeShift())
            {
               if (status == 0)
               {
                  long time = Util.getTimestamp(calendar);// calendar.Ticks / 100000L;
                  int x = rrdGraph.mapper.xtr(time);
                  rrdGraph.worker.drawLine(x, y0 - 1, x, y0 + 1, color, TICK_STROKE);
                  rrdGraph.worker.drawLine(x, y0, x, y1, color, GRID_STROKE);
               }
               calendar = findNextTime(tickSetting.minorUnit, tickSetting.minorUnitCount);
            }
         }
      }

      private void drawMajor()
      {
         adjustStartingTime(tickSetting.majorUnit, tickSetting.majorUnitCount);
         Color color = rrdGraph.gdef.colors[COLOR_MGRID];
         int y0 = rrdGraph.im.yorigin, y1 = y0 - rrdGraph.im.ysize;
         for (int status = getTimeShift(); status <= 0; status = getTimeShift())
         {
            if (status == 0)
            {
               long time = Util.getTimestamp(calendar);// calendar.Ticks / 1000L;
               int x = rrdGraph.mapper.xtr(time);
               rrdGraph.worker.drawLine(x, y0 - 2, x, y0 + 2, color, TICK_STROKE);
               rrdGraph.worker.drawLine(x, y0, x, y1, color, GRID_STROKE);
            }
            calendar = findNextTime(tickSetting.majorUnit, tickSetting.majorUnitCount);
         }
      }

      private void drawLabels()
      {
         // escape strftime like format string
         // Todo: Check regex replace
         String labelFormat = Regex.Replace(tickSetting.format, "([^%]|^)%([^%t])", "$1%t$2");
         Font font = rrdGraph.gdef.smallFont;
         Color color = rrdGraph.gdef.colors[COLOR_FONT];
         adjustStartingTime(tickSetting.labelUnit, tickSetting.labelUnitCount);
         int y = rrdGraph.im.yorigin + 2;
         for (int status = getTimeShift(); status <= 0; status = getTimeShift())
         {
            String label = formatLabel(tickSetting.format, calendar);
            long time = Util.getTimestamp(calendar);// calendar.Ticks / 100000L;
            int x1 = rrdGraph.mapper.xtr(time);
            int x2 = rrdGraph.mapper.xtr(time + tickSetting.labelSpan);
            int labelWidth = (int)rrdGraph.worker.getStringWidth(label, font);
            int x = x1 + (x2 - x1 - labelWidth) / 2;
            if (x >= rrdGraph.im.xorigin && x + labelWidth <= rrdGraph.im.xorigin + rrdGraph.im.xsize)
            {
               rrdGraph.worker.drawString(label, x, y, font, color);
            }
            calendar = findNextTime(tickSetting.labelUnit, tickSetting.labelUnitCount);
         }
      }

      private static String formatLabel(String format, DateTime date)
      {
         if (format.Contains("%"))
         {
            // strftime like format string
            return date.ToString(format);
            //return String.Format(format, date);
         }
         else
         {
            // simple date format
            Calendar myCal = CultureInfo.InvariantCulture.Calendar;
            switch (format)
            {
               case "HH:mm":
                  return string.Format("{0:D2}:{1,2:D2}", date.Hour, date.Minute);
               case "EEE":
                  return myCal.GetDayOfWeek(date).ToString();
               //return string.Format("{0:d2}{1:D2}{2:D2}", date.ToString("yy"), date.Month, date.Day);
               case "'Week 'w":
                  int weekNo = myCal.GetWeekOfYear(date, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);
                  string yearNo = date.ToString("yy");
                  return string.Format("w {00}-{1,2:D2}", yearNo, weekNo);
               case "MMM":
                  return string.Format("{0:D2}", date.Month);
               case "yy":
                  return string.Format("{0:D4}", date.Year);
            }
            return date.ToShortTimeString();
         }
      }

      private DateTime findNextTime(int timeUnit, int timeUnitCount)
      {
         DateTime startTime = calendar;
         TimeSpan increment;

         switch (timeUnit)
         {
            case SECOND:
               increment = new TimeSpan(0, 0, timeUnitCount);
               break;
            case MINUTE:
               increment = new TimeSpan(0, timeUnitCount, 0);
               break;
            case HOUR:
               increment = new TimeSpan(timeUnitCount, 0, 0);
               break;
            case DAY:
               increment = new TimeSpan(timeUnitCount, 0, 0, 0);
               break;
            case WEEK:
               increment = new TimeSpan(7 * timeUnitCount, 0, 0, 0);
               break;
            case MONTH:
               return calendar.AddMonths(timeUnitCount);
            case YEAR:
               return calendar.AddYears(timeUnitCount);
            default:
               throw new ArgumentException("Invalid time unit " + timeUnit.ToString());
         }
         return startTime.Add(increment);
      }

      private int getTimeShift()
      {
         long time = Util.getTimestamp(calendar);
         //            calendar.Ticks / 1000000L;
         return (time < rrdGraph.im.start) ? -1 : (time > rrdGraph.im.end) ? +1 : 0;
      }

      private void adjustStartingTime(int timeUnit, int timeUnitCount)
      {
         calendar = Util.ConvertToDateTime(rrdGraph.im.start);// new DateTime(rrdGraph.im.start * 1000000L);
         switch (timeUnit)
         {
            case SECOND:
               calendar.AddSeconds(-(calendar.Second % timeUnitCount));
               break;
            case MINUTE:
               calendar.AddSeconds(-calendar.Second);
               calendar.AddMinutes(-(calendar.Minute) % timeUnitCount);
               break;
            case HOUR:
               calendar.AddSeconds(-calendar.Second);
               calendar.AddMinutes(calendar.Minute);
               calendar.AddHours(-(calendar.Hour) % timeUnitCount);
               break;
            case DAY:
               calendar.AddSeconds(-calendar.Second);
               calendar.AddMinutes(calendar.Minute);
               calendar.AddHours(-calendar.Hour);
               break;
            case WEEK:
               calendar.AddSeconds(-calendar.Second);
               calendar.AddMinutes(calendar.Minute);
               calendar.AddHours(-calendar.Hour);
               DayOfWeek dow = calendar.DayOfWeek;
               int diffDays = 0;
               switch (dow)
               {
                  case DayOfWeek.Monday:
                     diffDays = 0;
                     break;
                  case DayOfWeek.Tuesday:
                     diffDays = 1;
                     break;
                  case DayOfWeek.Wednesday:
                     diffDays = 2;
                     break;
                  case DayOfWeek.Thursday:
                     diffDays = 3;
                     break;
                  case DayOfWeek.Friday:
                     diffDays = 4;
                     break;
                  case DayOfWeek.Saturday:
                     diffDays = 5;
                     break;
                  case DayOfWeek.Sunday:
                     diffDays = 6;
                     break;
               }
               calendar.AddDays(-diffDays);
               break;
            case MONTH:
               calendar.AddSeconds(-calendar.Second);
               calendar.AddMinutes(calendar.Minute);
               calendar.AddHours(-calendar.Hour);
               calendar.AddDays(-calendar.Day + 1);
               calendar.AddMonths(calendar.Month % timeUnitCount);
               break;
            case YEAR:
               calendar = new DateTime(calendar.Year - (calendar.Year % timeUnitCount), 1, 1);
               break;
         }
      }


      private void chooseTickSettings()
      {
         if (rrdGraph.gdef.timeAxisSetting != null)
         {
            tickSetting = new TimeAxisSetting(rrdGraph.gdef.timeAxisSetting);
         }
         else
         {
            for (int i = 0; tickSettings[i].secPerPix >= 0 && secPerPix > tickSettings[i].secPerPix; i++)
            {
               tickSetting = tickSettings[i];
            }
         }
      }

   }
}
