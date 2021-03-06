﻿using System;
using System.Collections.Generic;
using System.Text;
using rrd4n.Common;
using rrd4n.DataAccess.Data;

namespace rrd4n.Core
{
/* ============================================================
 * Rrd4j : Pure java implementation of RRDTool's functionality
 * ============================================================
 *
 * Project Info:  http://www.rrd4j.org
 * Project Lead:  Mathias Bogaert (m.bogaert@memenco.com)
 *
 * (C) Copyright 2003-2007, by Sasa Markovic.
 *
 * Developers:    Sasa Markovic
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

/**
 * Class to represent fetch request. For the complete explanation of all
 * fetch parameters consult RRDTool's
 * <a href="../../../../man/rrdfetch.html" target="man">rrdfetch man page</a>.
 *
 * You cannot create <code>FetchRequest</code> directly (no public constructor
 * is provided). Use {@link org.rrd4j.core.RrdDb#createFetchRequest(ConsolFun, long, long, long)
 * createFetchRequest()} method of your {@link org.rrd4j.core.RrdDb RrdDb} object.
 *
 * @author Sasa Markovic
 */
public class FetchRequest {
	private RrdDb parentDb;
	private ConsolFun consolFun;
	private long fetchStart;
	private long fetchEnd;
	private long resolution;
	private String[] filter;


   public FetchRequest(RrdDb parentDb, ConsolFun consolFun, DateTime fetchStart, DateTime fetchEnd, long resolution)
      : this(parentDb,consolFun,Util.getTimestamp(fetchStart),Util.getTimestamp(fetchEnd),resolution)
   {
   }

   public FetchRequest(RrdDb parentDb, ConsolFun consolFun, long fetchStart, long fetchEnd, long resolution) {
        if (consolFun == null) {
            throw new ArgumentException("Null consolidation function in fetch request");
        }
        if (fetchStart < 0) {
            throw new ArgumentException("Invalid start time in fetch request: " + fetchStart);
        }
        if (fetchEnd < 0) {
            throw new ArgumentException("Invalid end time in fetch request: " + fetchEnd);
        }
        if (fetchStart > fetchEnd) {
            throw new ArgumentException("Invalid start/end time in fetch request: " + fetchStart +
                    " > " + fetchEnd);
        }
        if (resolution <= 0) {
            throw new ArgumentException("Invalid resolution in fetch request: " + resolution);
        }

		this.parentDb = parentDb;
		this.consolFun = consolFun;
		this.fetchStart = fetchStart;
		this.fetchEnd = fetchEnd;
		this.resolution = resolution;
	}

	/**
	 * Sets request filter in order to fetch data only for
	 * the specified array of datasources (datasource names).
	 * If not set (or set to null), fetched data will
	 * containt values of all datasources defined in the corresponding RRD.
	 * To fetch data only from selected
	 * datasources, specify an array of datasource names as method argument.
	 * @param filter Array of datsources (datsource names) to fetch data from.
	 */
	public void setFilter(String[] filter) {
		this.filter = filter;
	}

	/**
	 * Sets request filter in order to fetch data only for
	 * the specified set of datasources (datasource names).
	 * If the filter is not set (or set to null), fetched data will
	 * containt values of all datasources defined in the corresponding RRD.
	 * To fetch data only from selected
	 * datasources, specify a set of datasource names as method argument.
	 * @param filter Set of datsource names to fetch data for.
	 */
	public void setFilter(List<String> filter) {
		this.filter = filter.ToArray();
	}

	/**
	 * Sets request filter in order to fetch data only for
	 * a single datasource (datasource name).
	 * If not set (or set to null), fetched data will
	 * containt values of all datasources defined in the corresponding RRD.
	 * To fetch data for a single datasource only,
	 * specify an array of datasource names as method argument.
	 * @param filter Array of datsources (datsource names) to fetch data from.
	 */
	public void setFilter(String filter) {
		this.filter = (filter == null)? null: (new String[] { filter });
	}

	/**
	 * Returns request filter. See {@link #setFilter(String[]) setFilter()} for
	 * complete explanation.
	 * @return Request filter (array of datasource names), null if not set.
	 */
	public String[] getFilter() {
		return filter;
	}

	/**
	 * Returns consolitation function to be used during the fetch process.
	 * @return Consolidation function.
	 */
	public ConsolFun getConsolFun() {
		return consolFun;
	}

	/**
	 * Returns starting timestamp to be used for the fetch request.
	 * @return Starting timstamp in seconds.
	 */
	public long getFetchStart() {
		return fetchStart;
	}

	/**
	 * Returns ending timestamp to be used for the fetch request.
	 * @return Ending timestamp in seconds.
	 */
	public long getFetchEnd() {
		return fetchEnd;
	}

	/**
	 * Returns fetch resolution to be used for the fetch request.
	 * @return Fetch resolution in seconds.
	 */
	public long getResolution() {
		return resolution;
	}

	/**
	 * Dumps the content of fetch request using the syntax of RRDTool's fetch command.
	 * @return Fetch request dump.
	 */
	public String dump() {
		return "fetch \"" + parentDb.getRrdBackend().getPath() +
			"\" " + consolFun + "\n --start " + fetchStart + "\n --end " + fetchEnd +
			(resolution > 1? "\n --resolution " + resolution: "");
	}

	String getRrdToolCommand() {
		return dump();
	}

	/**
	 * Returns data from the underlying RRD and puts it in a single
	 * {@link org.rrd4j.core.FetchData FetchData} object.
	 * @return FetchData object filled with timestamps and datasource values.
	 * @Thrown in case of I/O error.
	 */
	public FetchData fetchData() {
		return parentDb.fetchData(this);
	}

	/**
	 * Returns the underlying RrdDb object.
	 * @return RrdDb object used to create this FetchRequest object.
	 */
	public RrdDb getParentDb() {
		return parentDb;
	}

}
}
