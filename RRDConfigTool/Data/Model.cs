using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Castle.Core.Logging;
using rrd4n;
using rrd4n.Common;
using rrd4n.Core;
using rrd4n.DataAccess.Data;

namespace RRDConfigTool.Data
{
   public class Model
   {
      private ILogger logger = NullLogger.Instance;

      public ILogger Logger
      {
         get { return logger; }
         set { logger = value; }
      }

      private Dictionary<string, DatabaseData> databases = new Dictionary<string, DatabaseData>();
      private RrdDb database; 
      private long databaseStep;
      private RrdDef databaseDefinition = null;

      public bool DatabaseDirty { get; set; }
      public bool ReadOnly { get; set; }
      public DatabaseData EditingDatabaseData { get; set; }

      //public RrdDb Database
      //{
      //   get
      //   {
      //      return database;
      //   }
      //   set
      //   {
      //      database = value;
      //      if (Database != null)
      //         databaseDefinition = Database.getRrdDef();
      //      DatabaseDirty = true;
      //   }
      //}

      public string DatabaseName
      {
         get { return Path.GetFileNameWithoutExtension(EditingDatabaseData.Definition.getPath()); }
      }

      public string DatabasePath
      {
         get { return EditingDatabaseData.Definition.Path; }
      }


      public DsDef[] DataSourceDefinitions
      {
         get
         {
            return EditingDatabaseData.Definition.getDsDefs();
         }
      }

      //public ArchiveDisplayData[] Archives
      //{
      //   get
      //   {
      //      List<ArchiveDisplayData> archives = new List<ArchiveDisplayData>();
      //      foreach (ArcDef arcDef in DatabaseDefinition.getArcDefs())
      //      {
      //         archives.Add(new ArchiveDisplayData()
      //         {
      //            ConsolFunctionName = arcDef.getConsolFun().Name,
      //            RowCount = arcDef.getRows(),
      //            Steps = arcDef.getSteps(),
      //            Xff = arcDef.getXff(),
      //            StartTime = DateTime.MinValue,
      //            EndTime = DateTime.MaxValue
      //         });
      //      }
      //      return archives.ToArray();
      //   }
      //}
      //public RrdDef DatabaseDefinition
      //{
      //   get 
      //   {
      //      return databaseDefinition;
      //   }
      //   set
      //   {
      //      databaseDefinition = value;
      //   }
      //}
      public DateTime DatabaseLastUpdateTime
      {
         get
         {
            return EditingDatabaseData.Definition.getStartDateTime();
         }
      }
      public double LastValue
      {
         get
         {
            // ToDo:
            return double.NaN;
         }
      }
      public void AddDataSource(DsDef dsDef)
      {
         EditingDatabaseData.Definition.addDatasource(dsDef);
         DatabaseDirty = true;
      }

      public void UpdateDataSource(DatabaseData srcDatabase, DsDef updatedDsDef, DsDef originalDsDef)
      {
         RrdDb database = null;

         try
         {
            database = new RrdDb(srcDatabase.Definition.Path, false);
            Datasource datasource = database.getDatasource(originalDsDef.getDsName());
            if (datasource == null)
               throw new ArgumentException(updatedDsDef.getDsName() + " datasource don't exist");
            if (datasource.DsName != updatedDsDef.DsName)
               datasource.setDsName(updatedDsDef.getDsName());
            datasource.setDsType(updatedDsDef.getDsType());
            datasource.setHeartbeat(updatedDsDef.getHeartbeat());
            datasource.setMaxValue(updatedDsDef.getMaxValue(), true);
            datasource.setMinValue(updatedDsDef.getMinValue(), true);

         }
         catch (FileNotFoundException ex)
         {
            Logger.Error("Update datasource failed", ex);
            throw new ApplicationException("Can't update datasource until database saved!", ex);
         }
         finally
         {
            if (database != null)
               database.close();
         }
      }

      public void AddArchive(ArcDef arcDef)
      {
         EditingDatabaseData.Definition.addArchive(arcDef);
         DatabaseDirty = true;
      }

      public void UpdateArchive(ArcDef updatedArcDef)
      {
         var archDef = EditingDatabaseData.Definition.findArchive(updatedArcDef.getConsolFun(), updatedArcDef.getSteps());
         archDef.setRows(updatedArcDef.getRows());
         DatabaseDirty = true;
      }

      private ArcDef FindArchive(object archive)
      {
         foreach (var arcDef in EditingDatabaseData.Definition.getArcDefs())
         {
            if (arcDef.equals(archive))
            {
               return arcDef;
            }
         }
         return null;
      }

      public int ArchiveRows(object archive)
      {
         var archDef = FindArchive(archive);
         if (archDef == null)
            return 0;
         return archDef.getRows();
      }

      public int ArchiveStep(object archive)
      {
         var archDef = FindArchive(archive);
         if (archDef == null)
            return 0;
         return archDef.getSteps();
      }

      public string ArchiveType(object archive)
      {
         var archDef = FindArchive(archive);
         if (archDef == null)
            return string.Empty;
         return archDef.getConsolFun().Name;
      }

      public double ArchiveXff(object archive)
      {
         var archDef = FindArchive(archive);
         if (archDef == null)
            return 0.5d;
         return archDef.getXff();
      }


      [Obsolete]
      public string DumpArchive(object archiveDefinition)
      {
         throw new NotImplementedException("DumpArchive");
         //ArchiveDisplayData selectedArchive = archiveDefinition as ArchiveDisplayData;
         //Archive archive = Database.getArchive(new ConsolFun(selectedArchive.ConsolFunctionName), selectedArchive.Steps);
         //return archive.dump();
      }

      public DatabaseData GetDatabasedata()
      {
         return EditingDatabaseData;
      }
      public bool IsDatabase(string path)
      {
         return databases.ContainsKey(path);
      }

      public bool DatabaseExist(string databaseName)
      {
         foreach (var databasePath in databases.Keys)
         {
            if (Path.GetFileNameWithoutExtension(databasePath) == databaseName)
               return true;
         }
         return false;
      }

      public DatabaseData CreateDatabase(string databaseName, DateTime startTime, long step)
      {
         return CreateDatabase(new RrdDef(databaseName, startTime, step));
      }

      public DatabaseData CreateDatabase(RrdDef rrdDef)
      {

         EditingDatabaseData = new DatabaseData();
         EditingDatabaseData.Definition = rrdDef;
         EditingDatabaseData.LastUpdated = rrdDef.getStartDateTime();
         EditingDatabaseData.LastValue = double.NaN;
         EditingDatabaseData.SourceDatabasePath = null;
         DatabaseDirty = true;
         return EditingDatabaseData;
      }

      public DatabaseData AddDatabase(string databasePath)
      {
         if (databases.ContainsKey(databasePath))
            return databases[databasePath];
         try
         {
            RrdDb database = new RrdDb(databasePath);
            DatabaseData data = new DatabaseData();
            data.Saved = true;
            data.Definition = database.getRrdDef();
            data.LastUpdated = database.getLastUpdateDateTime();
            data.LastValue = database.getLastDatasourceValue(database.getDsNames()[0]);
            databases.Add(databasePath, data);
            database.close();
            return databases[databasePath];
         }
         catch (Exception ex)
         {
            Logger.Error("Fail to add database", ex);
            throw;
         }
      }

      public DatabaseData ReloadDatabase(DatabaseData srcDatabase)
      {
         if (!databases.ContainsKey(srcDatabase.Definition.Path))
            throw new ApplicationException("Database to reload don't exist");

         try
         {
            RrdDb database = new RrdDb(srcDatabase.Definition.Path);
            DatabaseData data = new DatabaseData();
            data.Saved = true;
            data.Definition = database.getRrdDef();
            data.LastUpdated = database.getLastUpdateDateTime();
            data.LastValue = database.getLastDatasourceValue(database.getDsNames()[0]);
            databases[srcDatabase.Definition.Path] = data;
            database.close();
            return data;
         }
         catch (Exception ex)
         {
            Logger.Error("Fail to add database", ex);
            throw;
         }
      }

      public bool RemoveDatabase(DatabaseData srcDatabase)
      {
         if (databases.ContainsKey(srcDatabase.Definition.Path))
         {
            databases.Remove(srcDatabase.Definition.Path);
            return true;
         }
         EditingDatabaseData = null;
         DatabaseDirty = false;
         return true;
      }
       
      public DsDef[] GetDataSourceDefinitions(DatabaseData databaseData)
      {
         return databaseData.Definition.getDsDefs();
      }

      public DatabaseData SetDatabaseAsEdit(DatabaseData srcDatabaseData)
      {
         if (!databases.ContainsKey(srcDatabaseData.Definition.Path))
            throw new ApplicationException("Database not open in model");

         // Make a clone of the source database definition and give it a new name
         RrdDb rrdDb = new RrdDb(srcDatabaseData.Definition.Path, true);
         databaseDefinition = rrdDb.getRrdDef();
         rrdDb.close();

         DatabaseData dstDatabaseData = new DatabaseData();
         dstDatabaseData.SourceDatabasePath = srcDatabaseData.Definition.Path;
         dstDatabaseData.Saved = false;
         dstDatabaseData.Definition = databaseDefinition;
         dstDatabaseData.Definition.Path = Path.GetFileNameWithoutExtension(databaseDefinition.Path) + "_";
         dstDatabaseData.LastUpdated = dstDatabaseData.Definition.getStartDateTime();
         dstDatabaseData.LastValue = double.NaN;
         DatabaseDirty = true;
         EditingDatabaseData = dstDatabaseData;
         databases.Add(dstDatabaseData.Definition.Path,dstDatabaseData);
         return dstDatabaseData;
      }

      public FetchData GetArchiveData(DatabaseData databaseData, string dataSourceName, ArcDef archiveDefinition)
      {
         RrdDb database = null;
         try
         {
            database = new RrdDb(databaseData.Definition.getPath(), true);
            int datasourceIndex = database.getDsIndex(dataSourceName);
            Archive archive = database.getArchive(new ConsolFun(archiveDefinition.getConsolFun().Name), archiveDefinition.Steps);
            Robin robin = archive.getRobin(datasourceIndex);
            double[] values = robin.getValues();
            DateTime archiveStartTime = archive.getStartDateTime();
            TimeSpan tick = new TimeSpan(archive.getArcStep() * TimeSpan.TicksPerSecond);

            FetchData fetchedData = new FetchData(archive.getArcStep(), archive.getEndTime(), new string[] { dataSourceName });
            long[] timestamps = new long[archive.getRows()];
            long offset = archive.getStartTime();
            for (var i = 0; i < archive.getRows(); i++)
            {
               timestamps[i] = offset;
               offset += archive.getArcStep();
            }
            fetchedData.setTimestamps(timestamps);
            double[][] value = new double[1][];
            value[0] = values;
            fetchedData.setValues(value);
            return fetchedData;

         }
         finally
         {
            if (database != null)
               database.close();
         }
      }
      public void ExportDatabase(string databasePath, string xmlFilePath)
      {
         RrdDb exportDatabase = new RrdDb(databasePath, true);
         exportDatabase.dumpXml(xmlFilePath);
      }
   }
}
