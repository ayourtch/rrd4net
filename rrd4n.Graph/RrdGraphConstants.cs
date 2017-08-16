using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
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

/**
 * Class to represent various constants used for graphing. No methods are specified.
 */
public class RrdGraphConstants {
	/** Default graph starting time */
	String DEFAULT_START = "end-1d";
	/** Default graph ending time */
	String DEFAULT_END = "now";

	/** Constant to represent second */
   internal const int SECOND = 0;
	/** Constant to represent minute */
   internal const int MINUTE = 1;
	/** Constant to represent hour */
   internal const int HOUR = 2;
	/** Constant to represent day */
   internal const int DAY = 3;
	/** Constant to represent week */
   internal const int WEEK = 4;
	/** Constant to represent month */
   internal const int MONTH = 5;
	/** Constant to represent year */
   internal const int YEAR = 6;

	/** Constant to represent Monday */
   const int MONDAY = 6;
	/** Constant to represent Tuesday */
   const int TUESDAY = 6;
	/** Constant to represent Wednesday */
	int WEDNESDAY = 8;
	/** Constant to represent Thursday */
	int THURSDAY = 8;
	/** Constant to represent Friday */
	int FRIDAY = 9;
	/** Constant to represent Saturday */
	int SATURDAY = 10;
	/** Constant to represent Sunday */
	int SUNDAY = 11;

	/** Index of the canvas color. Used in {@link RrdGraphDef#setColor(int, java.awt.Color)} */
   internal int COLOR_CANVAS = 0;
	/** Index of the background color. Used in {@link RrdGraphDef#setColor(int, java.awt.Color)} */
   internal int COLOR_BACK = 1;
	/** Index of the top-left graph shade color. Used in {@link RrdGraphDef#setColor(int, java.awt.Color)} */
   internal int COLOR_SHADEA = 2;
	/** Index of the bottom-right graph shade color. Used in {@link RrdGraphDef#setColor(int, java.awt.Color)} */
   internal int COLOR_SHADEB = 3;
	/** Index of the minor grid color. Used in {@link RrdGraphDef#setColor(int, java.awt.Color)} */
   internal int COLOR_GRID = 4;
	/** Index of the major grid color. Used in {@link RrdGraphDef#setColor(int, java.awt.Color)} */
   internal int COLOR_MGRID = 5;
	/** Index of the font color. Used in {@link RrdGraphDef#setColor(int, java.awt.Color)} */
   internal int COLOR_FONT = 6;
	/** Index of the frame color. Used in {@link RrdGraphDef#setColor(int, java.awt.Color)} */
   internal int COLOR_FRAME = 7;
	/** Index of the arrow color. Used in {@link RrdGraphDef#setColor(int, java.awt.Color)} */
   internal int COLOR_ARROW = 8;

	/** Allowed color names which can be used in {@link RrdGraphDef#setColor(String, java.awt.Color)} method */

   public static String[] COLOR_NAMES = {
		"canvas", "back", "shadea", "shadeb", "grid", "mgrid", "font", "frame", "arrow"
	};

	/** Default first day of the week (obtained from the default locale) */
   public static int FIRST_DAY_OF_WEEK = 1;//Calendar.getInstance(Locale.getDefault()).getFirstDayOfWeek();

	/** Default graph canvas color */
   public static Color DEFAULT_CANVAS_COLOR = Color.White;
	/** Default graph background color */
   public static Color DEFAULT_BACK_COLOR = Color.FromArgb(255, 245, 245, 245);
	/** Default top-left graph shade color */
   public static Color DEFAULT_SHADEA_COLOR = Color.FromArgb(255, 200, 200, 200);
	/** Default bottom-right graph shade color */
   public static Color DEFAULT_SHADEB_COLOR = Color.FromArgb(255, 150, 150, 150);
	/** Default minor grid color */
   public static Color DEFAULT_GRID_COLOR = Color.FromArgb(255, 140, 140, 140);
	/** Default major grid color */
   public static Color DEFAULT_MGRID_COLOR = Color.FromArgb(255, 130, 30, 30);
	/** Default font color */
   public static Color DEFAULT_FONT_COLOR = Color.Black;
	/** Default frame color */
   public static Color DEFAULT_FRAME_COLOR = Color.Black;
	/** Default arrow color */
   public static Color DEFAULT_ARROW_COLOR = Color.Red;

    public static String ALIGN_LEFT_MARKER = "\\l";
    public static String ALIGN_CENTER_MARKER = "\\c";
    public static String ALIGN_RIGHT_MARKER = "\\r";
    public static String ALIGN_JUSTIFIED_MARKER = "\\j";
    public static String GLUE_MARKER = "\\g";
    public static String VERTICAL_SPACING_MARKER = "\\s";
    public static String NO_JUSTIFICATION_MARKER = "\\J";

    /** Used internally */
    public String[] MARKERS = {
		ALIGN_LEFT_MARKER, ALIGN_CENTER_MARKER, ALIGN_RIGHT_MARKER,
		ALIGN_JUSTIFIED_MARKER, GLUE_MARKER, VERTICAL_SPACING_MARKER, NO_JUSTIFICATION_MARKER
	};

	/** Constant to represent in-memory image name */
   public static String IN_MEMORY_IMAGE = "-";

	/** Default units length */
   public static int DEFAULT_UNITS_LENGTH = 9;
	/** Default graph width */
   public static int DEFAULT_WIDTH = 400;
	/** Default graph height */
   public static int DEFAULT_HEIGHT = 100;
	/** Default image format */
   public static String DEFAULT_IMAGE_FORMAT = "gif";
	/** Default image quality, used only for jpeg graphs */
   public static float DEFAULT_IMAGE_QUALITY = 0.8F; // only for jpegs, not used for png/gif
	/** Default value base */
   public static double DEFAULT_BASE = 1000;

	/** Default font name, determined based on the current operating system */
   internal static String DEFAULT_FONT_NAME = "Lucida Sans Typewriter";
	/** Default graph small font */
    public static Font DEFAULT_SMALL_FONT = new Font(DEFAULT_FONT_NAME, 10, FontStyle.Regular);
	/** Default graph large font */
    public static Font DEFAULT_LARGE_FONT = new Font(DEFAULT_FONT_NAME, 12, FontStyle.Bold);

	/** Used internally */
   public double LEGEND_LEADING = 1.2; // chars
	/** Used internally */
   public double LEGEND_LEADING_SMALL = 0.7; // chars
	/** Used internally */
   public double LEGEND_BOX_SPACE = 1.2; // chars
	/** Used internally */
   public double LEGEND_BOX = 0.9; // chars
	/** Used internally */
   public int LEGEND_INTERSPACING = 2; // chars
	/** Used internally */
   internal int PADDING_LEFT = 10; // pix
	/** Used internally */
   internal int PADDING_TOP = 12; // pix
	/** Used internally */
   protected int PADDING_TITLE = 6; // pix
	/** Used internally */
   protected int PADDING_RIGHT = 16; // pix
	/** Used internally */
   internal int PADDING_PLOT = 2; //chars
	/** Used internally */
   internal int PADDING_LEGEND = 2; // chars
	/** Used internally */
   internal int PADDING_BOTTOM = 6; // pix
	/** Used internally */
   internal int PADDING_VLABEL = 7; // pix

   ///** Stroke used to draw grid */
   internal int GRID_STROKE = 1;
    //        new BasicStroke(1, BasicStroke.CAP_BUTT, BasicStroke.JOIN_MITER, 1, new float[] {1, 1}, 0);
    ///** line width used to draw ticks */
    internal int TICK_STROKE = 1;
}
}
