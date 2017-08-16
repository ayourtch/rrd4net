using System;

namespace rrd4n.Common.Time
{
   //As usual, [ X ] means that X is optional, { X } means that X may
   //42  * be either omitted or specified as many times as needed,
   //43  * alternatives are separated by |, brackets are used for grouping.
   //44  * (# marks the beginning of comment that extends to the end of line)
   //45  *
   //46  * TIME-SPECIFICATION ::= TIME-REFERENCE [ OFFSET-SPEC ] |
   //47  *			                   OFFSET-SPEC   |
   //48  *			   ( START | END ) OFFSET-SPEC
   //49  *
   //50  * TIME-REFERENCE ::= NOW | TIME-OF-DAY-SPEC [ DAY-SPEC-1 ] |
   //51  *                        [ TIME-OF-DAY-SPEC ] DAY-SPEC-2
   //52  *
   //53  * TIME-OF-DAY-SPEC ::= NUMBER (':') NUMBER [am|pm] | # HH:MM
   //54  *                     'noon' | 'midnight' | 'teatime'
   //55  *
   //56  * DAY-SPEC-1 ::= NUMBER '/' NUMBER '/' NUMBER |  # MM/DD/[YY]YY
   //57  *                NUMBER '.' NUMBER '.' NUMBER |  # DD.MM.[YY]YY
   //58  *                NUMBER                          # Seconds since 1970
   //59  *                NUMBER                          # YYYYMMDD
   //60  *
   //61  * DAY-SPEC-2 ::= MONTH-NAME NUMBER [NUMBER] |    # Month DD [YY]YY
   //62  *                'yesterday' | 'today' | 'tomorrow' |
   //63  *                DAY-OF-WEEK
   //64  *
   //65  *
   //66  * OFFSET-SPEC ::= '+'|'-' NUMBER TIME-UNIT { ['+'|'-'] NUMBER TIME-UNIT }
   //67  *
   //68  * TIME-UNIT ::= SECONDS | MINUTES | HOURS |
   //69  *               DAYS | WEEKS | MONTHS | YEARS
   //70  *
   //71  * NOW ::= 'now' | 'n'
   //72  *
   //73  * START ::= 'start' | 's'
   //74  * END   ::= 'end' | 'e'
   //75  *
   //76  * SECONDS ::= 'seconds' | 'second' | 'sec' | 's'
   //77  * MINUTES ::= 'minutes' | 'minute' | 'min' | 'm'
   //78  * HOURS   ::= 'hours' | 'hour' | 'hr' | 'h'
   //79  * DAYS    ::= 'days' | 'day' | 'd'
   //80  * WEEKS   ::= 'weeks' | 'week' | 'wk' | 'w'
   //81  * MONTHS  ::= 'months' | 'month' | 'mon' | 'm'
   //82  * YEARS   ::= 'years' | 'year' | 'yr' | 'y'
   //83  *
   //84  * MONTH-NAME ::= 'jan' | 'january' | 'feb' | 'february' | 'mar' | 'march' |
   //85  *                'apr' | 'april' | 'may' | 'jun' | 'june' | 'jul' | 'july' |
   //86  *                'aug' | 'august' | 'sep' | 'september' | 'oct' | 'october' |
   //87  *		  'nov' | 'november' | 'dec' | 'december'
   //88  *
   //89  * DAY-OF-WEEK ::= 'sunday' | 'sun' | 'monday' | 'mon' | 'tuesday' | 'tue' |
   //90  *                 'wednesday' | 'wed' | 'thursday' | 'thu' | 'friday' | 'fri' |
   //91  *                 'saturday' | 'sat'
   //92  *

   public class TimeParser
   {
      private const int PREVIOUS_OP = -1;

      TimeToken token;
      readonly TimeScanner scanner;
      readonly TimeSpec spec;

      int op = TimeToken.PLUS;
      int prev_multiplier = -1;

      /**
       * Constructs TimeParser instance from the given input string.
       * @param dateString at-style time specification (read rrdfetch man page
       * for the complete explanation)
       */
      public TimeParser(String dateString)
      {
         scanner = new TimeScanner(dateString);
         spec = new TimeSpec(dateString);
      }

      private void expectToken(int desired, String errorMessage)
      {
         token = scanner.nextToken();
         if (token.id != desired)
         {
            throw new ArgumentException(errorMessage);
         }
      }

      private void plusMinus(int doop)
      {
         if (doop >= 0)
         {
            op = doop;
            expectToken(TimeToken.NUMBER, "There should be number after " +
                                          (op == TimeToken.PLUS ? '+' : '-'));
            prev_multiplier = -1; /* reset months-minutes guessing mechanics */
         }
         int delta = int.Parse(token.value);
         token = scanner.nextToken();
         if (token.id == TimeToken.MONTHS_MINUTES)
         {
            /* hard job to guess what does that -5m means: -5mon or -5min? */
            switch (prev_multiplier)
            {
               case TimeToken.DAYS:
               case TimeToken.WEEKS:
               case TimeToken.MONTHS:
               case TimeToken.YEARS:
                  token = scanner.resolveMonthsMinutes(TimeToken.MONTHS);
                  break;
               case TimeToken.SECONDS:
               case TimeToken.MINUTES:
               case TimeToken.HOURS:
                  token = scanner.resolveMonthsMinutes(TimeToken.MINUTES);
                  break;
               default:
                  token = delta < 6 ? scanner.resolveMonthsMinutes(TimeToken.MONTHS) : scanner.resolveMonthsMinutes(TimeToken.MINUTES);
                  break;
            }
         }
         prev_multiplier = token.id;
         delta *= (op == TimeToken.PLUS) ? +1 : -1;
         switch (token.id)
         {
            case TimeToken.YEARS:
               spec.dyear += delta;
               return;
            case TimeToken.MONTHS:
               spec.dmonth += delta;
               return;
            case TimeToken.WEEKS:
               delta *= 7;
               spec.dday += delta;
               break;
               /* FALLTHRU */
            case TimeToken.DAYS:
               spec.dday += delta;
               return;
            case TimeToken.HOURS:
               spec.dhour += delta;
               return;
            case TimeToken.MINUTES:
               spec.dmin += delta;
               return;
            default: // default is 'seconds'
               spec.dsec += delta;
               break;
         }
      }

      private void timeOfDay()
      {
         int minute = 0;
         /* save token status in case we must abort */
         scanner.saveState();
         /* first pick out the time of day - we assume a HH (COLON|DOT) MM time */
         if (token.value.Length > 2)
         {
            return;
         }
         int hour = int.Parse(token.value);
         token = scanner.nextToken();
         if (token.id == TimeToken.SLASH || token.id == TimeToken.DOT)
         {
            /* guess we are looking at a date */
            token = scanner.restoreState();
            return;
         }
         if (token.id == TimeToken.COLON)
         {
            expectToken(TimeToken.NUMBER, "Parsing HH:MM syntax, expecting MM as number, got none");
            minute = int.Parse(token.value);
            if (minute > 59)
            {
               throw new ArgumentException("Parsing HH:MM syntax, got MM = " + minute + " (>59!)");
            }
            token = scanner.nextToken();
         }
         /* check if an AM or PM specifier was given */
         if (token.id == TimeToken.AM || token.id == TimeToken.PM)
         {
            if (hour > 12)
            {
               throw new ArgumentException("There cannot be more than 12 AM or PM hours");
            }
            if (token.id == TimeToken.PM)
            {
               if (hour != 12)
               {
                  /* 12:xx PM is 12:xx, not 24:xx */
                  hour += 12;
               }
            }
            else
            {
               if (hour == 12)
               {
                  /* 12:xx AM is 00:xx, not 12:xx */
                  hour = 0;
               }
            }
            token = scanner.nextToken();
         }
         else if (hour > 23)
         {
            /* guess it was not a time then ... */
            token = scanner.restoreState();
            return;
         }
         spec.hour = hour;
         spec.min = minute;
         spec.sec = 0;
         if (spec.hour == 24)
         {
            spec.hour = 0;
            spec.AddDays(1);
         }
      }

      private void assignDate(long mday, long mon, long year)
      {
         if (year > 138)
         {
            if (year > 1970)
            {
               year -= 1900;
            }
            else
            {
               throw new ArgumentException("Invalid year " + year + " (should be either 00-99 or >1900)");
            }
         }
         else if (year >= 0 && year < 38)
         {
            year += 100;	     /* Allow year 2000-2037 to be specified as   */
         }					     /* 00-37 until the problem of 2038 year will */
         /* arise for unices with 32-bit time_t     */
         if (year < 70)
         {
            throw new ArgumentException("Won't handle dates before epoch (01/01/1970), sorry");
         }
         spec.year = (int)year;
         spec.month = (int)mon;
         spec.day = (int)mday;
      }

      private void day()
      {
         long mday = 0;
         long mon, year = spec.year;
         switch (token.id)
         {
            case TimeToken.YESTERDAY:
               spec.RemoveDays(1);
               token = scanner.nextToken();
               break;
               /* FALLTRHU */
            case TimeToken.TODAY:	/* force ourselves to stay in today - no further processing */
               token = scanner.nextToken();
               break;
            case TimeToken.TOMORROW:
               spec.AddDays(1);
               token = scanner.nextToken();
               break;
            case TimeToken.JAN:
            case TimeToken.FEB:
            case TimeToken.MAR:
            case TimeToken.APR:
            case TimeToken.MAY:
            case TimeToken.JUN:
            case TimeToken.JUL:
            case TimeToken.AUG:
            case TimeToken.SEP:
            case TimeToken.OCT:
            case TimeToken.NOV:
            case TimeToken.DEC:
               /* do month mday [year] */
               mon = (token.id - TimeToken.JAN);
               expectToken(TimeToken.NUMBER, "the day of the month should follow month name");
               mday = long.Parse(token.value);
               token = scanner.nextToken();
               if (token.id == TimeToken.NUMBER)
               {
                  year = long.Parse(token.value);
                  token = scanner.nextToken();
               }
               else
               {
                  year = spec.year;
               }
               assignDate(mday, mon, year);
               break;
            case TimeToken.SUN:
            case TimeToken.MON:
            case TimeToken.TUE:
            case TimeToken.WED:
            case TimeToken.THU:
            case TimeToken.FRI:
            case TimeToken.SAT:
               /* do a particular day of the week */
               int wday = (token.id - TimeToken.SUN);
               spec.AddDays(wday - spec.wday);
               token = scanner.nextToken();
               break;
            case TimeToken.NUMBER:
               /* get numeric <sec since 1970>, MM/DD/[YY]YY, or DD.MM.[YY]YY */
               // int tlen = token.value.length();
               mon = long.Parse(token.value);
               if (mon > 10L * 365L * 24L * 60L * 60L)
               {
                  spec.localtime(mon);
                  token = scanner.nextToken();
                  break;
               }
               if (mon > 19700101 && mon < 24000101)
               { /*works between 1900 and 2400 */
                  year = mon / 10000;
                  mday = mon % 100;
                  mon = (mon / 100) % 100;
                  token = scanner.nextToken();
               }
               else
               {
                  token = scanner.nextToken();
                  if (mon <= 31 && (token.id == TimeToken.SLASH || token.id == TimeToken.DOT))
                  {
                     int sep = token.id;
                     expectToken(TimeToken.NUMBER, "there should be " +
                                                   (sep == TimeToken.DOT ? "month" : "day") +
                                                   " number after " +
                                                   (sep == TimeToken.DOT ? '.' : '/'));
                     mday = long.Parse(token.value);
                     token = scanner.nextToken();
                     if (token.id == sep)
                     {
                        expectToken(TimeToken.NUMBER, "there should be year number after " +
                                                      (sep == TimeToken.DOT ? '.' : '/'));
                        year = long.Parse(token.value);
                        token = scanner.nextToken();
                     }
                     /* flip months and days for European timing */
                     if (sep == TimeToken.DOT)
                     {
                        long x = mday;
                        mday = mon;
                        mon = x;
                     }
                  }
               }
               //mon--;
               if (mon < 1 || mon > 12)
               {
                  throw new ArgumentException("Did you really mean month " + (mon));
               }
               if (mday < 1 || mday > 31)
               {
                  throw new ArgumentException("I'm afraid that " + mday + " is not a valid day of the month");
               }
               assignDate(mday, mon, year);
               break;
         }
      }

      /**
       * Parses the input string specified in the constructor.
       * @return Object representing parsed date/time.
       */
      public TimeSpec parse()
      {
         long now = DateTime.Now.Ticks / TimeSpan.TicksPerSecond;
         int hr = 0;
         int time_reference;
         /* this MUST be initialized to zero for midnight/noon/teatime */
         /* establish the default time reference */
         spec.localtime(now);
         token = scanner.nextToken();
         switch (token.id)
         {
            case TimeToken.PLUS:
            case TimeToken.MINUS:
               break; /* jump to OFFSET-SPEC part */
            case TimeToken.START:
               spec.type = TimeSpec.TYPE_START;
               if (spec.type != TimeSpec.TYPE_START)
               {
                  spec.type = TimeSpec.TYPE_END;
               }
               spec.year = spec.month = spec.day = spec.hour = spec.min = spec.sec = 0;
               time_reference = token.id;
               token = scanner.nextToken();
               if (token.id == TimeToken.PLUS || token.id == TimeToken.MINUS)
               {
                  break;
               }
               if (time_reference != TimeToken.NOW) throw new ArgumentException("Words 'start' or 'end' MUST be followed by +|- offset");
               if (token.id != TimeToken.EOF) throw new ArgumentException("If 'now' is followed by a token it must be +|- offset");
               break;
               /* FALLTHRU */
            case TimeToken.END:
               if (spec.type != TimeSpec.TYPE_START)
               {
                  spec.type = TimeSpec.TYPE_END;
               }
               spec.year = spec.month = spec.day = spec.hour = spec.min = spec.sec = 0;
               time_reference = token.id;
               token = scanner.nextToken();
               if (token.id == TimeToken.PLUS || token.id == TimeToken.MINUS)
                  break;

               if (time_reference != TimeToken.NOW) throw new ArgumentException("Words 'start' or 'end' MUST be followed by +|- offset");
               if (token.id != TimeToken.EOF) throw new ArgumentException("If 'now' is followed by a token it must be +|- offset");
               break;
               /* FALLTHRU */
            case TimeToken.NOW:
               time_reference = token.id;
               token = scanner.nextToken();
               if (token.id == TimeToken.PLUS || token.id == TimeToken.MINUS)
                  break;
               if (time_reference != TimeToken.NOW) throw new ArgumentException("Words 'start' or 'end' MUST be followed by +|- offset");
               if (token.id != TimeToken.EOF) throw new ArgumentException("If 'now' is followed by a token it must be +|- offset");
               break;
               /* Only absolute time specifications below */
            case TimeToken.NUMBER:
               timeOfDay();
               if (token.id == TimeToken.PLUS || token.id == TimeToken.MINUS)
               {
                  break;
               }
               day();
               if (token.id != TimeToken.NUMBER)
               {
                  break;
               }
               timeOfDay();
               break;
               /* fix month parsing */
            case TimeToken.JAN:
            case TimeToken.FEB:
            case TimeToken.MAR:
            case TimeToken.APR:
            case TimeToken.MAY:
            case TimeToken.JUN:
            case TimeToken.JUL:
            case TimeToken.AUG:
            case TimeToken.SEP:
            case TimeToken.OCT:
            case TimeToken.NOV:
            case TimeToken.DEC:
               day();
               if (token.id != TimeToken.NUMBER)
               {
                  break;
               }
               timeOfDay();
               break;

               /* evil coding for TEATIME|NOON|MIDNIGHT - we've initialized
             * hr to zero up above, then fall into this case in such a
             * way so we add +12 +4 hours to it for teatime, +12 hours
             * to it for noon, and nothing at all for midnight, then
             * set our rettime to that hour before leaping into the
             * month scanner
             */
            case TimeToken.TEATIME:
               hr += 4;
               hr += 12;
               spec.hour = hr;
               spec.min = 0;
               spec.sec = 0;
               token = scanner.nextToken();
               day();
               break;
               /* FALLTHRU */
            case TimeToken.NOON:
               hr += 12;
               spec.hour = hr;
               spec.min = 0;
               spec.sec = 0;
               token = scanner.nextToken();
               day();
               break;
               /* FALLTHRU */
            case TimeToken.MIDNIGHT:
               spec.hour = hr;
               spec.min = 0;
               spec.sec = 0;
               token = scanner.nextToken();
               day();
               break;
            default:
               throw new ArgumentException("Unparsable time: " + token.value);
         }
         /*
          * the OFFSET-SPEC part
          *
          * (NOTE, the sc_tokid was prefetched for us by the previous code)
          */
         if (token.id == TimeToken.PLUS || token.id == TimeToken.MINUS)
         {
            scanner.setContext(false);
            while (token.id == TimeToken.PLUS || token.id == TimeToken.MINUS ||
                   token.id == TimeToken.NUMBER)
            {
               if (token.id == TimeToken.NUMBER)
               {
                  plusMinus(PREVIOUS_OP);
               }
               else
               {
                  plusMinus(token.id);
               }
               token = scanner.nextToken();
               /* We will get EOF eventually but that's OK, since
               token() will return us as many EOFs as needed */
            }
         }
         /* now we should be at EOF */
         if (token.id != TimeToken.EOF) throw new ArgumentException("Unparsable trailing text: " + token.value);
         return spec;
      }
   }
}