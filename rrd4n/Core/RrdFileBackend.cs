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
using System.IO;
using System.Text;
using rrd4n.Common;

namespace rrd4n.Core
{


/**
 * Backend which is used to store RRD data to ordinary files on the disk. This was the
 * default factory before 1.4.0 version. This backend is based on the
 * RandomAccessFile class (java.io.* package).
 */
public class RrdFileBackend : RrdBackend {
	/** read/write file status */
	protected bool readOnly;
	/** radnom access file handle */
	protected FileStream file;

	/**
	 * Creates RrdFileBackend object for the given file path, backed by RandomAccessFile object.
	 * @param path Path to a file
	 * @param readOnly True, if file should be open in a read-only mode. False otherwise
	 * @ Thrown in case of I/O error
	 */
	public RrdFileBackend(String path, bool readOnly)  
      :base(path)
   {
		this.readOnly = readOnly;
      file = new FileStream(path, FileMode.OpenOrCreate, readOnly ? FileAccess.Read : FileAccess.ReadWrite, readOnly ? FileShare.ReadWrite : FileShare.Read);
	}

	/**
	 * Closes the underlying RRD file.
	 *
	 * @ Thrown in case of I/O error
	 */
	public override void close()  {
		file.Close();
	}

	/**
	 * Returns canonical path to the file on the disk.
	 *
	 * @param path File path
	 * @return Canonical file path
	 * @ Thrown in case of I/O error
	 */
	public static String getCanonicalPath(String path)  {
		return Util.getCanonicalPath(path);
	}

	/**
	 * Returns canonical path to the file on the disk.
	 *
	 * @return Canonical file path
	 * @ Thrown in case of I/O error
	 */
	public String getCanonicalPath()  {
		return RrdFileBackend.getCanonicalPath(getPath());
	}

	/**
	 * Writes bytes to the underlying RRD file on the disk
	 *
	 * @param offset Starting file offset
	 * @param b      Bytes to be written.
	 * @ Thrown in case of I/O error
	 */
	internal override void write(long offset, byte[] b)  {
		file.Seek(offset, SeekOrigin.Begin);
		file.Write(b,0,b.Length);
	}

	/**
	 * Reads a number of bytes from the RRD file on the disk
	 *
	 * @param offset Starting file offset
	 * @param b      Buffer which receives bytes read from the file.
	 * @ Thrown in case of I/O error.
	 */
    internal override void read(long offset, byte[] b)
    {
		file.Seek(offset,SeekOrigin.Begin);
		if (file.Read(b,0,b.Length) != b.Length) {
			throw new IOException("Not enough bytes available in file " + getPath());
		}
	}

	/**
	 * Returns RRD file length.
	 *
	 * @return File length.
	 * @ Thrown in case of I/O error.
	 */
	public override long getLength()  {
		return file.Length;
	}

	/**
	 * Sets length of the underlying RRD file. This method is called only once, immediately
	 * after a new RRD file gets created.
	 *
	 * @param length Length of the RRD file
	 * @ Thrown in case of I/O error.
	 */
	public override void setLength(long length)  {
		file.SetLength(length);
	}
}
}
