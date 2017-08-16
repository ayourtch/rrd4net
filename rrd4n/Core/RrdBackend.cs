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

/**
 * Base implementation class for all backend classes. Each Round Robin Database object
 * ({@link RrdDb} object) is backed with a single RrdBackend object which performs
 * actual I/O operations on the underlying storage. Rrd4n supports
 * three different bakcends out of the box:</p>
 * <ul>
 * <li>{@link RrdFileBackend}: objects of this class are created from the
 * {@link RrdFileBackendFactory} class. This was the default backend used in all
 * Rrd4n releases prior to 1.4.0. It uses java.io.* package and
 * RandomAccessFile class to store RRD data in files on the disk.
 *
 * <li>{@link RrdNioBackend}: objects of this class are created from the
 * {@link RrdNioBackendFactory} class. The backend uses java.io.* and java.nio.*
 * classes (mapped ByteBuffer) to store RRD data in files on the disk. This backend is fast, very fast,
 * but consumes a lot of memory (borrowed not from the JVM but from the underlying operating system
 * directly). <b>This is the default backend used in Rrd4n since 1.4.0 release.</b>
 *
 * <li>{@link RrdMemoryBackend}: objects of this class are created from the
 * {@link RrdMemoryBackendFactory} class. This backend stores all data in memory. Once
 * JVM exits, all data gets lost. The backend is extremely fast and memory hungry.
 * </ul>
 *
 * To create your own backend in order to provide some custom type of RRD storage,
 * you should do the following:</p>
 *
 * <ul>
 * <li>Create your custom RrdBackend class (RrdCustomBackend, for example)
 * by extending RrdBackend class. You have to implement all abstract public methods defined
 * in the base class.
 *
 * <li>Create your custom RrdBackendFactory class (RrdCustomBackendFactory,
 * for example) by extending RrdBackendFactory class. You have to implement all
 * abstract public methods defined in the base class. Your custom factory class will actually
 * create custom backend objects when necessary.
 *
 * <li>Create instance of your custom RrdBackendFactory and register it as a regular
 * factory available to Rrd4n framework. See javadoc for {@link RrdBackendFactory} to
 * find out how to do this
 * </ul>
 *
 */
public abstract class RrdBackend {
	private static bool instanceCreated = false;
	private readonly String path;

	/**
	 * Creates backend for a RRD storage with the given path.
	 * @param path String identifying RRD storage. For files on the disk, this
	 * argument should represent file path. Other storage types might interpret
	 * this argument differently.
	 */
    public RrdBackend(String path)
    {
		this.path = path;
		instanceCreated = true;
	}

	/**
	 * Returns path to the storage.
	 * @return Storage path
	 */
	public String getPath() {
		return path;
	}

	/**
	 * Writes an array of bytes to the underlying storage starting from the given
	 * storage offset.
	 * @param offset Storage offset.
	 * @param b Array of bytes that should be copied to the underlying storage
	 * @ Thrown in case of I/O error
	 */
   internal abstract void write(long offset, byte[] b) ;

	/**
	 * Reads an array of bytes from the underlying storage starting from the given
	 * storage offset.
	 * @param offset Storage offset.
	 * @param b Array which receives bytes from the underlying storage
	 * @ Thrown in case of I/O error
	 */
   internal abstract void read(long offset, byte[] b) ;

	/**
	 * Returns the number of RRD bytes in the underlying storage.
	 * @return Number of RRD bytes in the storage.
	 * @ Thrown in case of I/O error.
	 */
	public abstract long getLength() ;

	/**
	 * Sets the number of bytes in the underlying RRD storage.
	 * This method is called only once, immediately after a new RRD storage gets created.
	 * @param length Length of the underlying RRD storage in bytes.
	 * @ Thrown in case of I/O error.
	 */
	public abstract void setLength(long length) ;

	/**
	 * Closes the underlying backend.
	 * @ Thrown in case of I/O error
	 */
    public abstract void close();

	/**
	 * This method suggests the caching policy to the Rrd4n frontend (high-level) classes. If <code>true</code>
	 * is returned, frontent classes will cache frequently used parts of a RRD file in memory to improve
	 * performance. If </code>false</code> is returned, high level classes will never cache RRD file sections
	 * in memory.
	 * @return <code>true</code> if file caching is enabled, <code>false</code> otherwise. By default, the
	 * method returns <code>true</code> but it can be overriden in subclasses.
	 */
	internal bool isCachingAllowed() {
		return true;
	}

	/**
	 * Reads all RRD bytes from the underlying storage
	 * @return RRD bytes
	 * @ Thrown in case of I/O error
	 */
	public  byte[] readAll()  {
		byte[] b = new byte[(int) getLength()];
		read(0, b);
		return b;
	}

   public void writeInt(long offset, int value)  {
		write(offset, getIntBytes(value));
	}

   public void writeLong(long offset, long value)  {
		write(offset, getLongBytes(value));
	}

   public void writeDouble(long offset, double value)  {
		write(offset, getDoubleBytes(value));
	}

   public void writeDouble(long offset, double value, int count)  {
		byte[] b = getDoubleBytes(value);
		byte[] image = new byte[8 * count];
		for(int i = 0, k = 0; i < count; i++) {
			image[k++] = b[0];
			image[k++] = b[1];
			image[k++] = b[2];
			image[k++] = b[3];
			image[k++] = b[4];
			image[k++] = b[5];
			image[k++] = b[6];
			image[k++] = b[7];
		}
		write(offset, image);
	}

   public void writeDouble(long offset, double[] values)  {
		int count = values.Length;
		byte[] image = new byte[8 * count];
		for(int i = 0, k = 0; i < count; i++) {
			byte[] b = getDoubleBytes(values[i]);
			image[k++] = b[0];
			image[k++] = b[1];
			image[k++] = b[2];
			image[k++] = b[3];
			image[k++] = b[4];
			image[k++] = b[5];
			image[k++] = b[6];
			image[k++] = b[7];
		}
		write(offset, image);
	}

   public void writeString(long offset, String value)  {
		value = value.Trim();
		byte[] b = new byte[RrdPrimitive.STRING_LENGTH * 2];
		for(int i = 0, k = 0; i < RrdPrimitive.STRING_LENGTH; i++) {
			char c = (i < value.Length)? value[i]: ' ';
			byte[] cb = getCharBytes(c);
			b[k++] = cb[0];
			b[k++] = cb[1];
		}
		write(offset, b);
	}

   public int readInt(long offset)  {
		byte[] b = new byte[4];
		read(offset, b);
		return getInt(b);
	}

   public long readLong(long offset)  {
		byte[] b = new byte[8];
		read(offset, b);
		return getLong(b);
	}

   public double readDouble(long offset)  {
		byte[] b = new byte[8];
		read(offset, b);
		return getDouble(b);
	}

   public double[] readDouble(long offset, int count)  {
		int byteCount = 8 * count;
		byte[] image = new byte[byteCount];
		read(offset, image);
		double[] values = new double[count];
		for(int i = 0, k = -1; i < count; i++) {
			byte[] b = new byte[] {
				image[++k], image[++k], image[++k], image[++k],
				image[++k], image[++k], image[++k], image[++k]
			};
			values[i] = getDouble(b);
		}
      return values;
	}

   public String readString(long offset)  {
		byte[] b = new byte[RrdPrimitive.STRING_LENGTH * 2];
		char[] c = new char[RrdPrimitive.STRING_LENGTH];
		read(offset, b);
		for(int i = 0, k = -1; i < RrdPrimitive.STRING_LENGTH; i++) {
			byte[] cb = new byte[] { b[++k], b[++k] };
			c[i] = getChar(cb);
		}
        string st = new string(c);
        return st.Trim();
	}

	// static helper methods

	private static byte[] getIntBytes(int value) {
	   return BitConverter.GetBytes(value);
	}

	private static byte[] getLongBytes(long value) {
	   return BitConverter.GetBytes(value);
	}

	private static byte[] getCharBytes(char value) {
	   return BitConverter.GetBytes(value);
	}

	private static byte[] getDoubleBytes(double value) {
	   return BitConverter.GetBytes(value);
	}

	private static int getInt(byte[] b) {
		Debug.Assert (b.Length == 4, "Invalid number of bytes for integer conversion");
	   return BitConverter.ToInt32(b, 0);
	}

	private static long getLong(byte[] b) {
		Debug.Assert (b.Length == 8, "Invalid number of bytes for long conversion");
	   return BitConverter.ToInt64(b, 0);
	}

	private static char getChar(byte[] b) {
		Debug.Assert (b.Length == 2, "Invalid number of bytes for char conversion");
	   return BitConverter.ToChar(b, 0);
	}

	private static double getDouble(byte[] b) {
      Debug.Assert (b.Length == 8, "Invalid number of bytes for double conversion");
		return BitConverter.ToDouble(b,0);
	}

   public static bool isInstanceCreated() {
		return instanceCreated;
	}
}
}
