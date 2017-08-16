using System;
using System.Collections.Generic;
using System.Text;

namespace rrd4n.Common
{
   public class CommandSplitter
   {

      const string COLON = "@#@";

      public static string[] split(string cmd)
      {
         cmd = cmd.Replace("\\:", COLON);
         String[] tokens = cmd.Split(':');
         for (int i = 0; i < tokens.Length; i++)
         {
            tokens[i] = tokens[i].Replace(COLON, ":");
         }
         return tokens;
      }
   }
}
