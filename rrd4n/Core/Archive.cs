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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Xml;
using rrd4n.Common;
using rrd4n.DataAccess.Data;

namespace rrd4n.Core
{

   /**
    * Class to represent single RRD archive in a RRD with its internal state.
    * Normally, you don't need methods to manipulate archive objects directly
    * because Rrd4n framework does it automatically for you.<p>
    * <p/>
    * Each archive object consists of three parts: archive definition, archive state objects
    * (one state object for each datasource) and round robin archives (one round robin for
    * each datasource). API (read-only) is provided to access each of theese parts.
    *
    * @author Mikael Nilsson
    */
   public class Archive : RrdUpdater
   {
      private readonly RrdDb parentDb;

      // definition
      private readonly RrdString consolFun;
      private readonly RrdDouble xff;
      private readonly RrdInt steps;
      private readonly RrdInt rows;

      // state
      private Robin[] robins;
      private ArcState[] states;

      public Archive(RrdDb parentDb, ArcDef arcDef)
      {
         bool shouldInitialize = arcDef != null;
         this.parentDb = parentDb;
         consolFun = new RrdString(this, true);  // constant, may be cached
         xff = new RrdDouble(this);
         steps = new RrdInt(this, true);            // constant, may be cached
         rows = new RrdInt(this, true);            // constant, may be cached
         if (shouldInitialize)
         {
            consolFun.set(arcDef.getConsolFun().Name);
            xff.set(arcDef.getXff());
            steps.set(arcDef.getSteps());
            rows.set(arcDef.getRows());
         }
         int n = parentDb.getHeader().getDsCount();
         states = new ArcState[n];
         robins = new Robin[n];
         for (int i = 0; i < n; i++)
         {
            states[i] = new ArcState(this, shouldInitialize);
            int numRows = rows.get();
            robins[i] = new Robin(this, numRows, shouldInitialize);
         }
      }

      //// read from XML
      //public Archive(RrdDb parentDb, DataImporter reader, int arcIndex) {
      //    this(parentDb, new ArcDef(
      //            reader.getConsolFun(arcIndex), reader.getXff(arcIndex),
      //            reader.getSteps(arcIndex), reader.getRows(arcIndex)));
      //    int n = parentDb.getHeader().getDsCount();
      //    for (int i = 0; i < n; i++) {
      //        // restore state
      //        states[i].setAccumValue(reader.getStateAccumValue(arcIndex, i));
      //        states[i].setNanSteps(reader.getStateNanSteps(arcIndex, i));
      //        // restore robins
      //        double[] values = reader.getValues(arcIndex, i);
      //        robins[i].update(values);
      //    }
      //}

      /**
       * Returns archive time step in seconds. Archive step is equal to RRD step
       * multiplied with the number of archive steps.
       *
       * @return Archive time step in seconds
       * @Thrown in case of I/O error.
       */
      public long getArcStep()
      {
         long step = parentDb.getHeader().getStep();
         return step * steps.get();
      }

      public String dump()
      {
         StringBuilder buffer = new StringBuilder("== ARCHIVE ==\n");
         buffer.Append("RRA:").Append(consolFun.get()).Append(":").Append(xff.get()).Append(":").Append(steps.get()).Append(":").Append(rows.get()).Append("\n");
         buffer.Append("interval [").Append(getStartTime()).AppendFormat(" ({0}) ", getStartDateTime().ToString())
            .Append(", ").Append(getEndTime()).AppendFormat(" ({0}) ", getEndDateTime().ToString()).Append("]" + "\n");
         for (int i = 0; i < robins.Length; i++)
         {
            buffer.Append(states[i].dump());
            buffer.Append(robins[i].dump());
         }
         return buffer.ToString();
      }

      public RrdDb getParentDb()
      {
         return parentDb;
      }

      public void archive(int dsIndex, double value, long numUpdates)
      {
         Robin robin = robins[dsIndex];
         ArcState state = states[dsIndex];
         long step = parentDb.getHeader().getStep();
         long lastUpdateTime = parentDb.getHeader().getLastUpdateTime();
         long updateTime = Util.normalize(lastUpdateTime, step) + step;
         long arcStep = getArcStep();
         // finish current step
         while (numUpdates > 0)
         {
            accumulate(state, value);
            numUpdates--;
            if (updateTime % arcStep == 0)
            {
               finalizeStep(state, robin);
               break;
            }
            else
            {
               updateTime += step;
            }
         }
         // update robin in bulk
         int bulkUpdateCount = (int)Math.Min(numUpdates / steps.get(), (long)rows.get());
         robin.bulkStore(value, bulkUpdateCount);
         // update remaining steps
         long remainingUpdates = numUpdates % steps.get();
         for (long i = 0; i < remainingUpdates; i++)
         {
            accumulate(state, value);
         }
      }

      private void accumulate(ArcState state, double value)
      {
         if (Double.IsNaN(value))
         {
            state.setNanSteps(state.getNanSteps() + 1);
         }
         else
         {
            ConsolFun cf = new ConsolFun(ConsolFun.ValueOf(consolFun.get()));
            switch (cf.CSType)
            {
               case ConsolFun.ConsolFunTypes.MIN:
                  state.setAccumValue(Util.min(state.getAccumValue(), value));
                  break;
               case ConsolFun.ConsolFunTypes.MAX:
                  state.setAccumValue(Util.max(state.getAccumValue(), value));
                  break;
               case ConsolFun.ConsolFunTypes.LAST:
                  state.setAccumValue(value);
                  break;
               case ConsolFun.ConsolFunTypes.AVERAGE:
                  state.setAccumValue(Util.sum(state.getAccumValue(), value));
                  break;
               case ConsolFun.ConsolFunTypes.TOTAL:
                  state.setAccumValue(Util.sum(state.getAccumValue(), value));
                  break;
            }
         }
      }

      private void finalizeStep(ArcState state, Robin robin)
      {
         // should store
         long arcSteps = steps.get();
         double arcXff = xff.get();
         long nanSteps = state.getNanSteps();
         //double nanPct = (double) nanSteps / (double) arcSteps;
         double accumValue = state.getAccumValue();
         if (nanSteps <= arcXff * arcSteps && !Double.IsNaN(accumValue))
         {
            if (getConsolFun().CSType == ConsolFun.ConsolFunTypes.AVERAGE)
            {
               accumValue /= (arcSteps - nanSteps);
            }
            robin.store(accumValue);
         }
         else
         {
            robin.store(Double.NaN);
         }
         state.setAccumValue(Double.NaN);
         state.setNanSteps(0);
      }

      /**
       * Returns archive consolidation function ("AVERAGE", "MIN", "MAX" or "LAST").
       *
       * @return Archive consolidation function.
       * @Thrown in case of I/O error.
       */
      public ConsolFun getConsolFun()
      {
         return new ConsolFun(ConsolFun.ValueOf(consolFun.get()));
      }

      /**
       * Returns archive X-files factor.
       *
       * @return Archive X-files factor (between 0 and 1).
       * @Thrown in case of I/O error.
       */
      public double getXff()
      {
         return xff.get();
      }

      /**
       * Returns the number of archive steps.
       *
       * @return Number of archive steps.
       * @Thrown in case of I/O error.
       */
      public int getSteps()
      {
         return steps.get();
      }

      /**
       * Returns the number of archive rows.
       *
       * @return Number of archive rows.
       * @Thrown in case of I/O error.
       */
      public int getRows()
      {
         return rows.get();
      }

      /**
       * Returns current starting timestamp. This value is not constant.
       *
       * @return Timestamp corresponding to the first archive row
       * @Thrown in case of I/O error.
       */
      public long getStartTime()
      {
         long endTime = getEndTime();
         long arcStep = getArcStep();
         long numRows = rows.get();
         return endTime - (numRows - 1) * arcStep;
      }

      public DateTime getStartDateTime()
      {
         return Util.ConvertToDateTime(getStartTime());
      }

      /**
       * Returns current ending timestamp. This value is not constant.
       *
       * @return Timestamp corresponding to the last archive row
       * @Thrown in case of I/O error.
       */
      public long getEndTime()
      {
         long arcStep = getArcStep();
         long lastUpdateTime = parentDb.getHeader().getLastUpdateTime();
         return Util.normalize(lastUpdateTime, arcStep);
      }

      public DateTime getEndDateTime()
      {
         return Util.ConvertToDateTime(getEndTime());
      }
      /**
       * Returns the underlying archive state object. Each datasource has its
       * corresponding ArcState object (archive states are managed independently
       * for each RRD datasource).
       *
       * @param dsIndex Datasource index
       * @return Underlying archive state object
       */
      public ArcState getArcState(int dsIndex)
      {
         return states[dsIndex];
      }

      /**
       * Returns the underlying round robin archive. Robins are used to store actual
       * archive values on a per-datasource basis.
       *
       * @param dsIndex Index of the datasource in the RRD.
       * @return Underlying round robin archive for the given datasource.
       */
      public Robin getRobin(int dsIndex)
      {
         return robins[dsIndex];
      }

      public FetchData fetchData(FetchRequest request)
      {
         long arcStep = getArcStep();
         long fetchStart = Util.normalize(request.FetchStart, arcStep);
         long fetchEnd = Util.normalize(request.FetchEnd, arcStep);
         if (fetchEnd < request.FetchEnd)
         {
            fetchEnd += arcStep;
         }
         long startTime = getStartTime();
         long endTime = getEndTime();
         String[] dsToFetch = request.getFilter();
         if (dsToFetch == null)
         {
            dsToFetch = parentDb.getDsNames();
         }
         int dsCount = dsToFetch.Length;
         int ptsCount = (int)((fetchEnd - fetchStart) / arcStep + 1);
         long[] timestamps = new long[ptsCount];
         double[][] values = new double[dsCount][];
         for (int i = 0; i < dsCount; i++)
            values[i] = new double[ptsCount];


         long matchStartTime = Math.Max(fetchStart, startTime);
         long matchEndTime = Math.Min(fetchEnd, endTime);
         double[][] robinValues = null;
         if (matchStartTime <= matchEndTime)
         {
            // preload robin values
            int matchCount = (int)((matchEndTime - matchStartTime) / arcStep + 1);
            int matchStartIndex = (int)((matchStartTime - startTime) / arcStep);
            robinValues = new double[dsCount][];
            for (int i = 0; i < dsCount; i++)
            {
               int dsIndex = parentDb.getDsIndex(dsToFetch[i]);
               robinValues[i] = robins[dsIndex].getValues(matchStartIndex, matchCount);
            }
         }
         for (int ptIndex = 0; ptIndex < ptsCount; ptIndex++)
         {
            long time = fetchStart + ptIndex * arcStep;
            timestamps[ptIndex] = time;
            for (int i = 0; i < dsCount; i++)
            {
               double value = Double.NaN;
               if (time >= matchStartTime && time <= matchEndTime)
               {
                  // inbound time
                  int robinValueIndex = (int)((time - matchStartTime) / arcStep);
                  Debug.Assert(robinValues != null);
                  value = robinValues[i][robinValueIndex];
               }
               values[i][ptIndex] = value;
            }
         }
         FetchData fetchData = new FetchData(steps.get(),endTime, parentDb.getDsNames());
         fetchData.setTimestamps(timestamps);
         fetchData.setValues(values);
         return fetchData;
      }

      public void appendXml(XmlWriter writer)
      {
         writer.WriteStartElement("rra");
         writer.WriteElementString("cf", consolFun.get());
         writer.WriteComment(getArcStep() + " seconds");
         writer.WriteElementString("pdp_per_row", steps.get().ToString());
         writer.WriteElementString("xff", xff.get().ToString());
         writer.WriteStartElement("cdp_prep");
         foreach (ArcState state in states)
         {
            state.appendXml(writer);
         }
         writer.WriteEndElement(); // cdp_prep
         writer.WriteStartElement("database");
         long startTime = getStartTime();
         for (int i = 0; i < rows.get(); i++)
         {
            long time = startTime + i * getArcStep();
            writer.WriteComment(Util.getDate(time) + " / " + time);
            writer.WriteStartElement("row");
            foreach (Robin robin in robins)
            {
               writer.WriteElementString("v", robin.getValue(i).ToString());
            }
            writer.WriteEndElement(); // row
         }
         writer.WriteEndElement(); // database
         writer.WriteEndElement(); // rra
      }

      /**
       * Copies object's internal state to another Archive object.
       *
       * @param other New Archive object to copy state to
       * @Thrown in case of I/O error
       */
      public void copyStateTo(RrdUpdater other)
      {
         if (other.GetType() != typeof(Archive))
         {
            throw new ArgumentException(
                    "Cannot copy Archive object to " + other.GetType().ToString());
         }
         Archive arc = (Archive)other;
         if (arc.consolFun.get().CompareTo(consolFun.get()) != 0)
         {
            throw new ArgumentException("Incompatible consolidation functions");
         }
         if (arc.steps.get() != steps.get())
         {
            throw new ArgumentException("Incompatible number of steps");
         }
         int count = parentDb.getHeader().getDsCount();
         for (int i = 0; i < count; i++)
         {
            int j = getMatchingDatasourceIndex(parentDb, i, arc.parentDb);
            if (j >= 0)
            {
               states[i].copyStateTo(arc.states[j]);
               robins[i].copyStateTo(arc.robins[j]);
            }
         }
      }

      /**
       * Sets X-files factor to a new value.
       *
       * @param xff New X-files factor value. Must be >= 0 and < 1.
       * @Thrown in case of I/O error
       */
      public void setXff(double xff)
      {
         if (xff < 0D || xff >= 1D)
         {
            throw new ArgumentException("Invalid xff supplied (" + xff + "), must be >= 0 and < 1");
         }
         this.xff.set(xff);
      }

      /**
       * Returns the underlying storage (backend) object which actually performs all
       * I/O operations.
       *
       * @return I/O backend object
       */
      public RrdBackend getRrdBackend()
      {
         return parentDb.getRrdBackend();
      }

      /**
       * Required to implement RrdUpdater interface. You should never call this method directly.
       *
       * @return Allocator object
       */
      public RrdAllocator getRrdAllocator()
      {
         return parentDb.getRrdAllocator();
      }
      public override string ToString()
      {
         return string.Format("{0} stepps:{1} rows:{2}", consolFun.get(), steps.get(), rows.get());
      }
      private static int getMatchingDatasourceIndex(RrdDb rrd1, int dsIndex, RrdDb rrd2)
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
   }
}
