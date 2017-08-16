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
using System.Diagnostics;

namespace rrd4n.Core
{


abstract public class RrdPrimitive {
    
    public enum PrimitiveType
    {
    	RRD_INT = 0,
        RRD_LONG = 1,
        RRD_DOUBLE = 2,
        RRD_STRING = 3
    };

   public const int STRING_LENGTH = 20;

	static readonly int[] RRD_PRIM_SIZES = { 4, 8, 8, 2 * STRING_LENGTH };

	private readonly RrdBackend backend;
	private readonly int byteCount;
	private readonly long pointer;
	private readonly bool cachingAllowed;

    public RrdPrimitive(RrdUpdater updater, PrimitiveType type, bool isConstant) 
		:this(updater, type, 1, isConstant)
   {}


    public RrdPrimitive(RrdUpdater updater, PrimitiveType type, int count, bool isConstant)
   {
		backend = updater.getRrdBackend();
		byteCount = RRD_PRIM_SIZES[(int)type] * count;
		pointer = updater.getRrdAllocator().allocate(byteCount);
		cachingAllowed = isConstant || backend.isCachingAllowed();
	}

   public byte[] readBytes() {
		byte[] b = new byte[byteCount];
		backend.read(pointer, b);
		return b;
	}

   public void writeBytes(byte[] b) {
		Debug.Assert( b.Length == byteCount, "Invalid number of bytes supplied to RrdPrimitive.write method");
		backend.write(pointer, b);
	}

   public int readInt() {
		return backend.readInt(pointer);
	}

   public void writeInt(int value) {
		backend.writeInt(pointer, value);
	}

   public long readLong() {
		return backend.readLong(pointer);
	}

   public void writeLong(long value) {
		backend.writeLong(pointer, value);
	}

   public double readDouble() {
		return backend.readDouble(pointer);
	}

   public double readDouble(int index) {
       long offset = pointer + index * RRD_PRIM_SIZES[(int)PrimitiveType.RRD_DOUBLE];
		return backend.readDouble(offset);
	}

   public double[] readDouble(int index, int count) {
       long offset = pointer + index * RRD_PRIM_SIZES[(int)PrimitiveType.RRD_DOUBLE];
		return backend.readDouble(offset, count);
	}

	public void writeDouble(double value) {
		backend.writeDouble(pointer, value);
	}

	public void writeDouble(int index, double value, int count) {
        long offset = pointer + index * RRD_PRIM_SIZES[(int)PrimitiveType.RRD_DOUBLE];
		backend.writeDouble(offset, value, count);
	}

	public void writeDouble(int index, double[] values) {
        long offset = pointer + index * RRD_PRIM_SIZES[(int)PrimitiveType.RRD_DOUBLE];
		backend.writeDouble(offset, values);
	}

	public String readString() {
		return backend.readString(pointer);
	}

	public void writeString(String value) {
		backend.writeString(pointer, value);
	}

	public bool isCachingAllowed() {
		return cachingAllowed;
	}
}
}
