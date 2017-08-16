using System;
using System.Collections.Generic;
using System.Text;
using rrd4n.Data;
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

   public class CommentText : RrdGraphConstants
    {
        private String text;		// original text

       public String resolvedText;	// resolved text
        public String marker; // end-of-text marker
      internal bool enabled; // hrule and vrule comments can be disabled at runtime
        public int x, y; // coordinates, evaluated later

       public CommentText(String text)
        {
            this.text = text;
        }

       public virtual void resolveText(DataProcessor dproc, ValueScaler valueScaler)
        {
            resolvedText = text;
            marker = "";
            if (resolvedText != null)
            {
                foreach (String aMARKERS in MARKERS)
                {
                    if (resolvedText.EndsWith(aMARKERS))
                    {
                        marker = aMARKERS;
                        resolvedText = resolvedText.Substring(0, resolvedText.Length - marker.Length);
                        trimIfGlue();
                        break;
                    }
                }
            }
            enabled = resolvedText != null;
        }


       public void trimIfGlue()
        {
            if (marker.CompareTo(GLUE_MARKER) == 0)
            {
                resolvedText = resolvedText.Trim();
            }
        }

       public virtual bool isPrint()
        {
            return false;
        }

       public bool isValidGraphElement()
        {
            return !isPrint() && enabled;
        }
    }
}
