﻿using System;

namespace rrd4n.Common.Time
{
   public class TimeToken
   {
      public const int MIDNIGHT = 1;
      public const int NOON = 2;
      public const int TEATIME = 3;
      public const int PM = 4;
      public const int AM = 5;
      public const int YESTERDAY = 6;
      public const int TODAY = 7;
      public const int TOMORROW = 8;
      public const int NOW = 9;
      public const int START = 10;
      public const int END = 11;
      public const int SECONDS = 12;
      public const int MINUTES = 13;
      public const int HOURS = 14;
      public const int DAYS = 15;
      public const int WEEKS = 16;
      public const int MONTHS = 17;
      public const int YEARS = 18;
      public const int MONTHS_MINUTES = 19;
      public const int NUMBER = 20;
      public const int PLUS = 21;
      public const int MINUS = 22;
      public const int DOT = 23;
      public const int COLON = 24;
      public const int SLASH = 25;
      public const int ID = 26;
      public const int JUNK = 27;
      public const int JAN = 28;
      public const int FEB = 29;
      public const int MAR = 30;
      public const int APR = 31;
      public const int MAY = 32;
      public const int JUN = 33;
      public const int JUL = 34;
      public const int AUG = 35;
      public const int SEP = 36;
      public const int OCT = 37;
      public const int NOV = 38;
      public const int DEC = 39;
      public const int SUN = 40;
      public const int MON = 41;
      public const int TUE = 42;
      public const int WED = 43;
      public const int THU = 44;
      public const int FRI = 45;
      public const int SAT = 46;
      public const int EOF = -1;

      internal readonly String value; /* token name */
      internal readonly int id;   /* token id */

      public TimeToken(String value, int id)
      {
         this.value = value;
         this.id = id;
      }

      public override string ToString()
      {
         return value + " [" + id + "]";
      }
   }
}