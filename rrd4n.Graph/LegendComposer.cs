using System;
using System.Collections.Generic;
using System.Text;

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

   class LegendComposer : RrdGraphConstants
   {
      private static RrdGraphDef gdef;
      private static ImageWorker worker;
      private static int legX;
      private static int legY;
      private static int legWidth;
      private static double interlegendSpace;
      private static double leading;
      private static double smallLeading;
      private static double boxSpace;

      internal LegendComposer(RrdGraph rrdGraph, int legX, int legY, int legWidth)
      {
         LegendComposer.gdef = rrdGraph.gdef;
         LegendComposer.worker = rrdGraph.worker;
         LegendComposer.legX = legX;
         LegendComposer.legY = legY;
         LegendComposer.legWidth = legWidth;
         interlegendSpace = rrdGraph.getInterlegendSpace();
         leading = rrdGraph.getLeading();
         smallLeading = rrdGraph.getSmallLeading();
         boxSpace = rrdGraph.getBoxSpace();
      }

      internal int placeComments() {
		Line line = new Line();
        foreach (CommentText comment in gdef.comments) {
            if (comment.isValidGraphElement()) {
                if (!line.canAccomodate(comment)) {
                    line.layoutAndAdvance(false);
                    line.clear();
                }
                line.add(comment);
            }
        }
		line.layoutAndAdvance(true);
		worker.dispose();
		return legY;
	}

      class Line
      {
         private String lastMarker;
         private double width;
         private int spaceCount;
         private bool noJustification;
         private List<CommentText> comments = new List<CommentText>();

         internal Line()
         {
            clear();
         }

         internal void clear()
         {
            lastMarker = "";
            width = 0;
            spaceCount = 0;
            noJustification = false;
            comments.Clear();
         }

         internal bool canAccomodate(CommentText comment)
         {
            // always accomodate if empty
            if (comments.Count == 0)
            {
               return true;
            }
            // cannot accomodate if the last marker was \j, \l, \r, \c, \s
            if (lastMarker.CompareTo(ALIGN_LEFT_MARKER) == 0 || lastMarker.CompareTo(ALIGN_CENTER_MARKER) == 0 ||
                  lastMarker.CompareTo(ALIGN_RIGHT_MARKER) == 0 || lastMarker.CompareTo(ALIGN_JUSTIFIED_MARKER) == 0 ||
                  lastMarker.CompareTo(VERTICAL_SPACING_MARKER) == 0)
            {
               return false;
            }
            // cannot accomodate if line would be too long
            double commentWidth = getCommentWidth(comment);
            if (lastMarker.CompareTo(GLUE_MARKER) != 0)
            {
               commentWidth += interlegendSpace;
            }
            return width + commentWidth <= legWidth;
         }

         internal void add(CommentText comment)
         {
            double commentWidth = getCommentWidth(comment);
            if (comments.Count > 0 && lastMarker.CompareTo(GLUE_MARKER) != 0)
            {
               commentWidth += interlegendSpace;
               spaceCount++;
            }
            width += commentWidth;
            lastMarker = comment.marker;
            noJustification |= lastMarker.CompareTo(NO_JUSTIFICATION_MARKER) == 0;
            comments.Add(comment);
         }

         internal void layoutAndAdvance(bool isLastLine)
         {
            if (comments.Count > 0)
            {
               if (lastMarker.CompareTo(ALIGN_LEFT_MARKER) == 0)
               {
                  placeComments(legX, interlegendSpace);
               }
               else if (lastMarker.CompareTo(ALIGN_RIGHT_MARKER) == 0)
               {
                  placeComments(legX + legWidth - width, interlegendSpace);
               }
               else if (lastMarker.CompareTo(ALIGN_CENTER_MARKER) == 0)
               {
                  placeComments(legX + (legWidth - width) / 2.0, interlegendSpace);
               }
               else if (lastMarker.CompareTo(ALIGN_JUSTIFIED_MARKER) == 0)
               {
                  // anything to justify?
                  if (spaceCount > 0)
                  {
                     placeComments(legX, (legWidth - width) / spaceCount + interlegendSpace);
                  }
                  else
                  {
                     placeComments(legX, interlegendSpace);
                  }
               }
               else if (lastMarker.CompareTo(VERTICAL_SPACING_MARKER) == 0)
               {
                  placeComments(legX, interlegendSpace);
               }
               else
               {
                  // nothing specified, align with respect to '\J'
                  if (noJustification || isLastLine)
                  {
                     placeComments(legX, interlegendSpace);
                  }
                  else
                  {
                     placeComments(legX, (legWidth - width) / spaceCount + interlegendSpace);
                  }
               }
               if (lastMarker.CompareTo(VERTICAL_SPACING_MARKER) == 0)
               {
                  legY += (int)smallLeading;
               }
               else
               {
                  legY += (int)leading;
               }
            }
         }

         private double getCommentWidth(CommentText comment)
         {
            double commentWidth = worker.getStringWidth(comment.resolvedText, gdef.smallFont);
            if (comment.GetType() == typeof(LegendText))
            {
               commentWidth += boxSpace;
            }
            return commentWidth;
         }

         private void placeComments(double xStart, double space)
         {
            double x = xStart;
            foreach (CommentText comment in comments)
            {
               comment.x = (int)x;
               comment.y = legY;
               x += getCommentWidth(comment);
               if (comment.marker.CompareTo(GLUE_MARKER) != 0)
               {
                  x += space;
               }
            }
         }
      }
   }
}
