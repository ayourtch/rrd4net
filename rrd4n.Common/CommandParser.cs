using System;
using System.Collections.Generic;
using System.Text;
namespace rrd4n.Common
{
   public class CommandParser
   {
      protected const string DEFAULT_START = "now";
      protected String DEFAULT_STEP = "300";
      private LinkedList<String> words = new LinkedList<String>();
      private StringBuilder buff;


      public long GetTime(string shortForm, string longForm)
      {
         string timeParameter = getOptionValue(shortForm, longForm, DEFAULT_START);

         DateTime startDateTime;
         if (timeParameter == DEFAULT_START)
            startDateTime = DateTime.Now.AddSeconds(-10);
         else
            startDateTime = DateTime.Parse(timeParameter);
         return Util.getTimestamp(startDateTime);
      }

      protected void TokenizeCommand(string command)
      {

         String cmd = command.Trim();
         // parse words
         char activeQuote = '\0';
         for (int i = 0; i < cmd.Length; i++)
         {
            char c = cmd[i];
            if ((c == '"' || c == '\'') && activeQuote == 0)
            {
               // opening double or single quote
               initWord();
               activeQuote = c;
               continue;
            }
            if (c == activeQuote)
            {
               // closing quote
               activeQuote = '\0';
               continue;
            }
            if (isSeparator(c) && activeQuote == 0)
            {
               // separator encountered
               finishWord();
               continue;
            }
            if (c == '\\' && activeQuote == '"' && i + 1 < cmd.Length)
            {
               // check for \" and \\ inside double quotes
               char c2 = cmd[i + 1];
               if (c2 == '\\' || c2 == '"')
               {
                  appendWord(c2);
                  i++;
                  continue;
               }
            }
            // ordinary character
            appendWord(c);
         }
         if (activeQuote != 0)
         {
            throw new ArgumentException("End of command reached but " + activeQuote + " expected");
         }
         finishWord();
      }

      protected string getOptionValue(String shortForm, String longForm, String defaultValue)
      {
         String value = null;
         if (shortForm != null)
         {
            value = getOptionValue("-" + shortForm);
         }
         if (value == null && longForm != null)
         {
            value = getOptionValue("--" + longForm);
         }
         if (value == null)
         {
            value = defaultValue;
         }
         return value;
      }

      protected String getOptionValue(String shortForm, String longForm)
      {
         return getOptionValue(shortForm, longForm, null);
      }

      private String getOptionValue(String fullForm)
      {
         LinkedListNode<string> node = words.First;
         while (node != null)
         {
            String word = node.Value;
            if (word.CompareTo(fullForm) == 0)
            {
               // full match, the value is in the next word
               if (node.Next == null) throw new ArgumentException("Value for option " + fullForm + " expected but not found");

               LinkedListNode<string> nextNode = node.Next;
               words.Remove(node);
               node = nextNode;
               String value = node.Value;
               words.Remove(node);
               return value;
            }
            if (word.StartsWith(fullForm))
            {
               int pos = fullForm.Length;
               if (word[pos] == '=')
               {
                  // skip '=' if present
                  pos++;
               }
               words.Remove(node);
               return word.Substring(pos);
            }
            node = node.Next;
         }
         return null;
      }

      protected bool getBooleanOption(String shortForm, String longForm)
      {
         LinkedListNode<string> node = words.First;
         while (node != null)
         {
            String word = node.Value;
            if ((shortForm != null && word.CompareTo("-" + shortForm) == 0) ||
                  (longForm != null && word.CompareTo("--" + longForm) == 0))
            {
               words.Remove(node);
               return true;
            }
            node = node.Next;
         }
         return false;
      }
      protected String[] getMultipleOptionValues(String shortForm, String longForm)
      {
         return getMultipleOptions(shortForm, longForm);
      }

      protected String[] getMultipleOptions(String shortForm, String longForm)
      {
         List<String> values = new List<String>();
         for (; ; )
         {
            String value = getOptionValue(shortForm, longForm, null);
            if (value == null)
            {
               break;
            }
            values.Add(value);
         }
         return values.ToArray();
      }

      protected String[] getRemainingWords()
      {
         string[] remainingWords = new string[words.Count];
         var i = 0;
         foreach (var word in words)
         {
            remainingWords[i] = word;
            i++;
         }

         return remainingWords;
      }

      protected bool isSeparator(char c)
      {
         return char.IsWhiteSpace(c);
      }
      private void appendWord(char c)
      {
         if (buff == null)
         {
            buff = new StringBuilder("");
         }
         buff.Append(c);
      }

      private void finishWord()
      {
         if (buff != null)
         {
            words.AddLast(buff.ToString());
            buff = null;
         }
      }

      private void initWord()
      {
         if (buff == null)
         {
            buff = new StringBuilder("");
         }
      }

   }
}
