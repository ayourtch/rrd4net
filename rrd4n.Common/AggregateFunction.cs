using System;

namespace rrd4n.Common
{
   public static class AggregateFunction
   {
      public enum Type
      {
         AVERAGE,
         MIN,
         MAX,
         LAST,
         FIRST,
         TOTAL
      }

      public static Type Create(string functionName)
      {
         switch (functionName)
         {
            case "AVERAGE":
               return Type.AVERAGE;
            case "MIN":
               return Type.MIN;
            case "MAX":
               return Type.MAX;
            case "LAST":
               return Type.LAST;
            case "FIRST":
               return Type.FIRST;
            case "TOTAL":
               return Type.TOTAL;
            default:
               throw new ApplicationException("Invalid aggregate function name");
         }
      }
   }
}
