﻿/* ============================================================
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
using System.Text;

namespace rrd4n.Core
{

/**
 * Base (abstract public) backend factory class which holds references to all concrete
 * backend factories and defines abstract public methods which must be implemented in
 * all concrete factory implementations.<p>
 *
 * Factory classes are used to create concrete {@link RrdBackend} implementations.
 * Each factory creates unlimited number of specific backend objects.
 *
 * Rrd4n supports four different backend types (backend factories) out of the box:<p>
 * <ul>
 * <li>{@link RrdFileBackend}: objects of this class are created from the
 * {@link RrdFileBackendFactory} class. This was the default backend used in all
 * Rrd4n releases before 1.4.0 release. It uses java.io.* package and RandomAccessFile class to store
 * RRD data in files on the disk.
 *
 * <li>{@link RrdSafeFileBackend}: objects of this class are created from the
 * {@link RrdSafeFileBackendFactory} class. It uses java.io.* package and RandomAccessFile class to store
 * RRD data in files on the disk. This backend is SAFE:
 * it locks the underlying RRD file during update/fetch operations, and caches only static
 * parts of a RRD file in memory. Therefore, this backend is safe to be used when RRD files should
 * be shared <b>between several JVMs</b> at the same time. However, this backend is *slow* since it does
 * not use fast java.nio.* package (it's still based on the RandomAccessFile class).
 *
 * <li>{@link RrdNioBackend}: objects of this class are created from the
 * {@link RrdNioBackendFactory} class. The backend uses java.io.* and java.nio.*
 * classes (mapped ByteBuffer) to store RRD data in files on the disk. This is the default backend
 * since 1.4.0 release.
 *
 * <li>{@link RrdMemoryBackend}: objects of this class are created from the
 * {@link RrdMemoryBackendFactory} class. This backend stores all data in memory. Once
 * JVM exits, all data gets lost. The backend is extremely fast and memory hungry.
 * </ul>
 *
 * Each backend factory is identifed by its {@link #getFactoryName() name}. Constructors
 * are provided in the {@link RrdDb} class to create RrdDb objects (RRD databases)
 * backed with a specific backend.<p>
 *
 * See javadoc for {@link RrdBackend} to find out how to create your custom backends.
 */
public abstract class RrdBackendFactory 
{
	
    private static readonly Dictionary<String, RrdBackendFactory> factories = new Dictionary<String, RrdBackendFactory>();
	private static RrdBackendFactory defaultFactory;

    static RrdBackendFactory()
    {

        RrdFileBackendFactory fileFactory = new RrdFileBackendFactory();
        registerFactory(fileFactory);
        //RrdMemoryBackendFactory memoryFactory = new RrdMemoryBackendFactory();
        //registerFactory(memoryFactory);
        //RrdNioBackendFactory nioFactory = new RrdNioBackendFactory();
        //registerFactory(nioFactory);
        //RrdSafeFileBackendFactory safeFactory = new RrdSafeFileBackendFactory();
        //registerFactory(safeFactory);
        selectDefaultFactory();
	}

	private static void selectDefaultFactory() {
        // ToDo: Get version from assembly
		String version = "1.3.";//System.getProperty("java.version");
		if(version == null 
            || version.StartsWith("1.3.") 
            || version.StartsWith("1.4.0") 
            || version.StartsWith("1.4.1")) 
        {
			setDefaultFactory("FILE");
		}
		else 
			setDefaultFactory("NIO");
	}

	/**
	 * Returns backend factory for the given backend factory name.
	 * @param name Backend factory name. Initially supported names are:<p>
	 * <ul>
	 * <li><b>FILE</b>: Default factory which creates backends based on the
	 * java.io.* package. RRD data is stored in files on the disk
	 * <li><b>SAFE</b>: Default factory which creates backends based on the
	 * java.io.* package. RRD data is stored in files on the disk. This backend
	 * is "safe". Being safe means that RRD files can be safely shared between
	 * several JVM's.
	 * <li><b>NIO</b>: Factory which creates backends based on the
	 * java.nio.* package. RRD data is stored in files on the disk
	 * <li><b>MEMORY</b>: Factory which creates memory-oriented backends.
	 * RRD data is stored in memory, it gets lost as soon as JVM exits.
	 * </ul>
	 * @return Backend factory for the given factory name
	 */
	public static RrdBackendFactory getFactory(String name) 
    {
      if (factories.ContainsKey(name))
         return factories[name];

        throw new ApplicationException("No backend factory found with the name specified [" + name + "]");
	}

	/**
	 * Registers new (custom) backend factory within the Rrd4n framework.
	 * @param factory Factory to be registered
	 */
	public static void registerFactory(RrdBackendFactory factory) {
		String name = factory.getFactoryName();
		if (!factories.ContainsKey(name)) {
			factories.Add(name, factory);
		}
		else {
			throw new ApplicationException("Backend factory '" + name + "' cannot be registered twice");
		}
	}

	/**
	 * Registers new (custom) backend factory within the Rrd4n framework and sets this
	 * factory as the default.
	 * @param factory Factory to be registered and set as default
	 */
	public static void registerAndSetAsDefaultFactory(RrdBackendFactory factory) {
		registerFactory(factory);
		setDefaultFactory(factory.getFactoryName());
	}

	/**
	 * Returns the defaul backend factory. This factory is used to construct
	 * {@link RrdDb} objects if no factory is specified in the RrdDb constructor.
	 * @return Default backend factory.
	 */
	public static RrdBackendFactory getDefaultFactory() {
		return defaultFactory;
	}

	/**
	 * Replaces the default backend factory with a new one. This method must be called before
	 * the first RRD gets created. <p>
	 * @param factoryName Name of the default factory. Out of the box, Rrd4n supports four
	 * different RRD backends: "FILE" (java.io.* based), "SAFE" (java.io.* based - use this
	 * backend if RRD files may be accessed from several JVMs at the same time),
	 * "NIO" (java.nio.* based) and "MEMORY" (byte[] based).
	 */
	public static void setDefaultFactory(String factoryName) {
		// We will allow this only if no RRDs are created
		if (!RrdBackend.isInstanceCreated()) {
			defaultFactory = getFactory(factoryName);
		}
		else {
			throw new ApplicationException("Could not change the default backend factory. " +
					"This method must be called before the first RRD gets created");
		}
	}

	/**
	 * Creates RrdBackend object for the given storage path.
	 * @param path Storage path
	 * @param readOnly True, if the storage should be accessed in read/only mode.
	 * False otherwise.
	 * @return Backend object which handles all I/O operations for the given storage path
	 * @ Thrown in case of I/O error.
	 */
	public abstract RrdBackend open(String path, bool readOnly) ;

	/**
	 * Determines if a storage with the given path already exists.
     *
	 * @param path Storage path
	 * @return True, if such storage exists, false otherwise.
	 */
	public abstract bool exists(String path) ;

    /**
     * Determines if the header should be validated.
     *
     * @param path
     * @return
     * @
     */
    public abstract bool shouldValidateHeader(String path) ;

    /**
	 * Returns the name (primary ID) for the factory.
	 * @return Name of the factory.
	 */
	public abstract String getFactoryName();
}
}
