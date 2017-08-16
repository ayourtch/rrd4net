using System;
using rrd4n.Common;

namespace rrd4n.Data
{
    /* ============================================================
     * Rrd4n : Pure c# implementation of RRDTool's functionality
     * ============================================================
     *
     * Project Info:  http://minidev.se
     * Project Lead:  Mikael Nilsson (info@minidev.se)
     *
     * (C) Copyright 2009-2010, by Mikael Nilsson.
     *
     * Developers:    Mikael Nilsson
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


    class RpnCalculator
    {
        private const int TKN_VAR = 0;
        private const int TKN_NUM = 1;
        private const int TKN_PLUS = 2;
        private const int TKN_MINUS = 3;
        private const int TKN_MULT = 4;
        private const int TKN_DIV = 5;
        private const int TKN_MOD = 6;
        private const int TKN_SIN = 7;
        private const int TKN_COS = 8;
        private const int TKN_LOG = 9;
        private const int TKN_EXP = 10;
        private const int TKN_FLOOR = 11;
        private const int TKN_CEIL = 12;
        private const int TKN_ROUND = 13;
        private const int TKN_POW = 14;
        private const int TKN_ABS = 15;
        private const int TKN_SQRT = 16;
        private const int TKN_RANDOM = 17;
        private const int TKN_LT = 18;
        private const int TKN_LE = 19;
        private const int TKN_GT = 20;
        private const int TKN_GE = 21;
        private const int TKN_EQ = 22;
        private const int TKN_IF = 23;
        private const int TKN_MIN = 24;
        private const int TKN_MAX = 25;
        private const int TKN_LIMIT = 26;
        private const int TKN_DUP = 27;
        private const int TKN_EXC = 28;
        private const int TKN_POP = 29;
        private const int TKN_UN = 30;
        private const int TKN_UNKN = 31;
        private const int TKN_NOW = 32;
        private const int TKN_TIME = 33;
        private const int TKN_PI = 34;
        private const int TKN_E = 35;
        private const int TKN_AND = 36;
        private const int TKN_OR = 37;
        private const int TKN_XOR = 38;
        private const int TKN_PREV = 39;
        private const int TKN_INF = 40;
        private const int TKN_NEGINF = 41;
        private const int TKN_STEP = 42;
        private const int TKN_YEAR = 43;
        private const int TKN_MONTH = 44;
        private const int TKN_DATE = 45;
        private const int TKN_HOUR = 46;
        private const int TKN_MINUTE = 47;
        private const int TKN_SECOND = 48;
        private const int TKN_WEEK = 49;
        private const int TKN_SIGN = 50;
        private const int TKN_RND = 51;
        private const int TKN_TREND = 52;

        private String rpnExpression;
        private String sourceName;
        private DataProcessor dataProcessor;

        private Token[] tokens;
        private RpnStack stack = new RpnStack();
        private double[] calculatedValues;
        private long[] timestamps;
        private double timeStep;

        public RpnCalculator(String rpnExpression, String sourceName, DataProcessor dataProcessor)
        {
            this.rpnExpression = rpnExpression;
            this.sourceName = sourceName;
            this.dataProcessor = dataProcessor;
            this.timestamps = dataProcessor.getTimestamps();
            this.timeStep = this.timestamps[1] - this.timestamps[0];
            this.calculatedValues = new double[this.timestamps.Length];
            char[] splitters = new char[2];
            splitters[0] = ',';
            splitters[1] = ' ';

            string[] st = rpnExpression.Split(splitters);

            tokens = new Token[st.Length];
            for (int i = 0; i < st.Length; i++)
            {
                tokens[i] = createToken(st[i]);
            }
        }

        private Token createToken(String parsedText)
        {
            Token token = new Token();
            if (Util.isDouble(parsedText))
            {
                token.id = TKN_NUM;
                token.number = Util.parseDouble(parsedText);
            }
            else if (parsedText.CompareTo("+") == 0)
            {
                token.id = TKN_PLUS;
            }
            else if (parsedText.CompareTo("-") == 0)
            {
                token.id = TKN_MINUS;
            }
            else if (parsedText.CompareTo("*") == 0)
            {
                token.id = TKN_MULT;
            }
            else if (parsedText.CompareTo("/") == 0)
            {
                token.id = TKN_DIV;
            }
            else if (parsedText.CompareTo("%") == 0)
            {
                token.id = TKN_MOD;
            }
            else if (parsedText.CompareTo("SIN") == 0)
            {
                token.id = TKN_SIN;
            }
            else if (parsedText.CompareTo("COS") == 0)
            {
                token.id = TKN_COS;
            }
            else if (parsedText.CompareTo("LOG") == 0)
            {
                token.id = TKN_LOG;
            }
            else if (parsedText.CompareTo("EXP") == 0)
            {
                token.id = TKN_EXP;
            }
            else if (parsedText.CompareTo("FLOOR") == 0)
            {
                token.id = TKN_FLOOR;
            }
            else if (parsedText.CompareTo("CEIL") == 0)
            {
                token.id = TKN_CEIL;
            }
            else if (parsedText.CompareTo("ROUND") == 0)
            {
                token.id = TKN_ROUND;
            }
            else if (parsedText.CompareTo("POW") == 0)
            {
                token.id = TKN_POW;
            }
            else if (parsedText.CompareTo("ABS") == 0)
            {
                token.id = TKN_ABS;
            }
            else if (parsedText.CompareTo("SQRT") == 0)
            {
                token.id = TKN_SQRT;
            }
            else if (parsedText.CompareTo("RANDOM") == 0)
            {
                token.id = TKN_RANDOM;
            }
            else if (parsedText.CompareTo("LT") == 0)
            {
                token.id = TKN_LT;
            }
            else if (parsedText.CompareTo("LE") == 0)
            {
                token.id = TKN_LE;
            }
            else if (parsedText.CompareTo("GT") == 0)
            {
                token.id = TKN_GT;
            }
            else if (parsedText.CompareTo("GE") == 0)
            {
                token.id = TKN_GE;
            }
            else if (parsedText.CompareTo("EQ") == 0)
            {
                token.id = TKN_EQ;
            }
            else if (parsedText.CompareTo("IF") == 0)
            {
                token.id = TKN_IF;
            }
            else if (parsedText.CompareTo("MIN") == 0)
            {
                token.id = TKN_MIN;
            }
            else if (parsedText.CompareTo("MAX") == 0)
            {
                token.id = TKN_MAX;
            }
            else if (parsedText.CompareTo("LIMIT") == 0)
            {
                token.id = TKN_LIMIT;
            }
            else if (parsedText.CompareTo("DUP") == 0)
            {
                token.id = TKN_DUP;
            }
            else if (parsedText.CompareTo("EXC") == 0)
            {
                token.id = TKN_EXC;
            }
            else if (parsedText.CompareTo("POP") == 0)
            {
                token.id = TKN_POP;
            }
            else if (parsedText.CompareTo("UN") == 0)
            {
                token.id = TKN_UN;
            }
            else if (parsedText.CompareTo("UNKN") == 0)
            {
                token.id = TKN_UNKN;
            }
            else if (parsedText.CompareTo("NOW") == 0)
            {
                token.id = TKN_NOW;
            }
            else if (parsedText.CompareTo("TIME") == 0)
            {
                token.id = TKN_TIME;
            }
            else if (parsedText.CompareTo("PI") == 0)
            {
                token.id = TKN_PI;
            }
            else if (parsedText.CompareTo("E") == 0)
            {
                token.id = TKN_E;
            }
            else if (parsedText.CompareTo("AND") == 0)
            {
                token.id = TKN_AND;
            }
            else if (parsedText.CompareTo("OR") == 0)
            {
                token.id = TKN_OR;
            }
            else if (parsedText.CompareTo("XOR") == 0)
            {
                token.id = TKN_XOR;
            }
            else if (parsedText.CompareTo("PREV") == 0)
            {
                token.id = TKN_PREV;
                token.variable = sourceName;
                token.values = calculatedValues;
            }
            else if (parsedText.StartsWith("PREV(") && parsedText.EndsWith(")"))
            {
                token.id = TKN_PREV;
                token.variable = parsedText.Substring(5, parsedText.Length - 1);
                token.values = dataProcessor.getValues(token.variable);
            }
            else if (parsedText.CompareTo("INF") == 0)
            {
                token.id = TKN_INF;
            }
            else if (parsedText.CompareTo("NEGINF") == 0)
            {
                token.id = TKN_NEGINF;
            }
            else if (parsedText.CompareTo("STEP") == 0)
            {
                token.id = TKN_STEP;
            }
            else if (parsedText.CompareTo("YEAR") == 0)
            {
                token.id = TKN_YEAR;
            }
            else if (parsedText.CompareTo("MONTH") == 0)
            {
                token.id = TKN_MONTH;
            }
            else if (parsedText.CompareTo("DATE") == 0)
            {
                token.id = TKN_DATE;
            }
            else if (parsedText.CompareTo("HOUR") == 0)
            {
                token.id = TKN_HOUR;
            }
            else if (parsedText.CompareTo("MINUTE") == 0)
            {
                token.id = TKN_MINUTE;
            }
            else if (parsedText.CompareTo("SECOND") == 0)
            {
                token.id = TKN_SECOND;
            }
            else if (parsedText.CompareTo("WEEK") == 0)
            {
                token.id = TKN_WEEK;
            }
            else if (parsedText.CompareTo("SIGN") == 0)
            {
                token.id = TKN_SIGN;
            }
            else if (parsedText.CompareTo("RND") == 0)
            {
                token.id = TKN_RND;
            }
            else if (parsedText.CompareTo("TREND") == 0)
            {
               token.id = TKN_TREND;
               token.variable = sourceName;
               //token.values = dataProcessor.getValues(token.variable);
            }
            else
            {
                token.id = TKN_VAR;
                token.variable = parsedText;
                token.values = dataProcessor.getValues(token.variable);
            }
            return token;
        }

        public double[] calculateValues()
        {
           double[] trendBuffer = new double[timestamps.Length];
            for (int slot = 0; slot < timestamps.Length; slot++)
            {
                resetStack();
                foreach (Token token in tokens)
                {
                    double x1, x2, x3;
                    switch (token.id)
                    {
                        case TKN_NUM:
                            push(token.number);
                            break;
                        case TKN_VAR:
                            push(token.values[slot]);
                            break;
                        case TKN_PLUS:
                            push(pop() + pop());
                            break;
                        case TKN_MINUS:
                            x2 = pop();
                            x1 = pop();
                            push(x1 - x2);
                            break;
                        case TKN_MULT:
                            push(pop() * pop());
                            break;
                        case TKN_DIV:
                            x2 = pop();
                            x1 = pop();
                            push(x1 / x2);
                            break;
                        case TKN_MOD:
                            x2 = pop();
                            x1 = pop();
                            push(x1 % x2);
                            break;
                        case TKN_SIN:
                            push(Math.Sin(pop()));
                            break;
                        case TKN_COS:
                            push(Math.Cos(pop()));
                            break;
                        case TKN_LOG:
                            push(Math.Log(pop()));
                            break;
                        case TKN_EXP:
                            push(Math.Exp(pop()));
                            break;
                        case TKN_FLOOR:
                            push(Math.Floor(pop()));
                            break;
                        case TKN_CEIL:
                            push(Math.Ceiling(pop()));
                            break;
                        case TKN_ROUND:
                            push(Math.Round(pop()));
                            break;
                        case TKN_POW:
                            x2 = pop();
                            x1 = pop();
                            push(Math.Pow(x1, x2));
                            break;
                        case TKN_ABS:
                            push(Math.Abs(pop()));
                            break;
                        case TKN_SQRT:
                            push(Math.Sqrt(pop()));
                            break;
                        //case TKN_RANDOM:
                        //    push(Math.Random());
                        //    break;
                        case TKN_LT:
                            x2 = pop();
                            x1 = pop();
                            push(x1 < x2 ? 1 : 0);
                            break;
                        case TKN_LE:
                            x2 = pop();
                            x1 = pop();
                            push(x1 <= x2 ? 1 : 0);
                            break;
                        case TKN_GT:
                            x2 = pop();
                            x1 = pop();
                            push(x1 > x2 ? 1 : 0);
                            break;
                        case TKN_GE:
                            x2 = pop();
                            x1 = pop();
                            push(x1 >= x2 ? 1 : 0);
                            break;
                        case TKN_EQ:
                            x2 = pop();
                            x1 = pop();
                            push(x1 == x2 ? 1 : 0);
                            break;
                        case TKN_IF:
                            x3 = pop();
                            x2 = pop();
                            x1 = pop();
                            push(x1 != 0 ? x2 : x3);
                            break;
                        case TKN_MIN:
                            push(Math.Min(pop(), pop()));
                            break;
                        case TKN_MAX:
                            push(Math.Max(pop(), pop()));
                            break;
                        case TKN_LIMIT:
                            x3 = pop();
                            x2 = pop();
                            x1 = pop();
                            push(x1 < x2 || x1 > x3 ? Double.NaN : x1);
                            break;
                        case TKN_DUP:
                            push(peek());
                            break;
                        case TKN_EXC:
                            x2 = pop();
                            x1 = pop();
                            push(x2);
                            push(x1);
                            break;
                        case TKN_POP:
                            pop();
                            break;
                        case TKN_UN:
                            push(Double.IsNaN(pop()) ? 1 : 0);
                            break;
                        case TKN_UNKN:
                            push(Double.NaN);
                            break;
                        case TKN_NOW:
                            push(Util.getTimestamp());
                            break;
                        case TKN_TIME:
                            push((long)Math.Round((double)timestamps[slot]));
                            break;
                            // ToDo: Handle LTIME:
                            //Takes the time as defined by TIME, applies the time zone offset valid at that time
                            // including daylight saving time if your OS supports it, and pushes the result on the stack.
                        //case TKN_LTIME:
                        //    push((long)Math.Round((double)timestamps[slot])); Convert to local time
                        //    break;
                        case TKN_PI:
                            push(Math.PI);
                            break;
                        case TKN_E:
                            push(Math.E);
                            break;
                        case TKN_AND:
                            x2 = pop();
                            x1 = pop();
                            push((x1 != 0 && x2 != 0) ? 1 : 0);
                            break;
                        case TKN_OR:
                            x2 = pop();
                            x1 = pop();
                            push((x1 != 0 || x2 != 0) ? 1 : 0);
                            break;
                        case TKN_XOR:
                            x2 = pop();
                            x1 = pop();
                            push(((x1 != 0 && x2 == 0) || (x1 == 0 && x2 != 0)) ? 1 : 0);
                            break;
                        case TKN_PREV:
                            push((slot == 0) ? Double.NaN : token.values[slot - 1]);
                            break;
                        case TKN_INF:
                            push(Double.PositiveInfinity);
                            break;
                        case TKN_NEGINF:
                            push(Double.NegativeInfinity);
                            break;
                        case TKN_STEP:
                            push(timeStep);
                            break;
                            // ToDo: Test Time functions
                        case TKN_YEAR:
                            push(GetDateField(pop(),token.id));
                            break;
                        case TKN_MONTH:
                            push(GetDateField(pop(), token.id));
                            break;
                        case TKN_DATE:
                            push(GetDateField(pop(), token.id));
                            break;
                        case TKN_HOUR:
                            push(GetDateField(pop(), token.id));
                            break;
                        case TKN_MINUTE:
                            push(GetDateField(pop(), token.id));
                            break;
                        case TKN_SECOND:
                            push(GetDateField(pop(), token.id));
                            break;
                        //case TKN_WEEK:
                        //    push(getCalendarField(pop(), Calendar.WEEK_OF_YEAR));
                        //    break;
                        case TKN_SIGN:
                            x1 = pop();
                            push(Double.IsNaN(x1) ? Double.NaN : x1 > 0 ? +1 : x1 < 0 ? -1 : 0);
                            break;
                        //case TKN_RND:
                        //    push(Math.Floor(pop() * Math.Random()));
                        //    break;
                       case TKN_TREND:
                          int len = (int)pop();
                          double val = pop();
                          trendBuffer[slot%len] = val;
                          if (slot < len)
                          {
                             push(double.NaN);
                          }
                          else
                          {
                             double trendValue = 0;
                             for (var i = 0; i < len; i++)
                             {
                                trendValue += trendBuffer[i];
                             }
                             trendValue /= len;
                             push(trendValue);
                          }
                          break;

                        default:
                            throw new ArgumentException("Unexpected RPN token encountered, token.id=" + token.id);
                    }
                }
                calculatedValues[slot] = pop();
                // check if stack is empty only on the first try
                if (slot == 0 && !isStackEmpty())
                {
                    throw new ArgumentException("Stack not empty at the end of calculation. " +
                            "Probably bad RPN expression [" + rpnExpression + "]");
                }
            }
            return calculatedValues;
        }

        //private double getCalendarField(double timestamp, int field) {
        //    Calendar calendar = Util.getCalendar((long)(timestamp * 1000));
        //    return calendar.get(field);
        //}

        private double GetDateField(double timeStamp, int token)
        {
            DateTime dt = Util.ConvertToDateTime((long)timeStamp);
            switch (token)
            {
                case TKN_YEAR:
                    return dt.Year;
                case TKN_MONTH:
                    return dt.Month;
                case TKN_DATE:
                    return dt.Day;
                case TKN_HOUR:
                    return dt.Hour;
                case TKN_MINUTE:
                    return dt.Minute;
                case TKN_SECOND:
                    return dt.Second;
            }
            throw new ArgumentException("Invalid token in time conversion");
        }

        private void push(double x)
        {
            stack.push(x);
        }

        private double pop()
        {
            return stack.pop();
        }

        private double peek()
        {
            return stack.peek();
        }

        private void resetStack()
        {
            stack.reset();
        }

        private bool isStackEmpty()
        {
            return stack.isEmpty();
        }

        private class RpnStack
        {
            private const int MAX_STACK_SIZE = 1000;
            private double[] stack = new double[MAX_STACK_SIZE];
            private int pos = 0;

            public void push(double x)
            {
                if (pos >= MAX_STACK_SIZE)
                {
                    throw new ArgumentException("PUSH failed, RPN stack full [" + MAX_STACK_SIZE + "]");
                }
                stack[pos++] = x;
            }

            public double pop()
            {
                if (pos <= 0)
                {
                    throw new ArgumentException("POP failed, RPN stack is empty ");
                }
                return stack[--pos];
            }

            public double peek()
            {
                if (pos <= 0)
                {
                    throw new ArgumentException("PEEK failed, RPN stack is empty ");
                }
                return stack[pos - 1];
            }

            public void reset()
            {
                pos = 0;
            }

            public bool isEmpty()
            {
                return pos <= 0;
            }
        }

        private class Token
        {
            public int id = -1;
            public double number = Double.NaN;
            public String variable = null;
            public double[] values = null;
        }
    }
}
