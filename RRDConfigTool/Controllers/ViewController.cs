using System;
using System.Collections.Generic;
using System.IO;
using Castle.Core.Logging;
using rrd4n.Common;
using rrd4n.Core;
using rrd4n.DataAccess.Data;
using rrd4n.DataAccess.Interface;
using rrd4n.Parser;
using RRDConfigTool.Data;

namespace RRDConfigTool.Controllers
{
   public class ViewController
   {
      enum DataDateFormat
      {
         date,
         time,
         dateandtime,
         other
      };

      public RrdDbTreeForm TreeForm { get; set; }
      private RrdDbForm rrdDbForm = null;

      public RrdDbAccessInterface dbAccess { get; set; }

      public DockingMainForm view;
      private Model model;

      private ILogger logger = NullLogger.Instance;

      public ILogger Logger
      {
         get { return logger; }
         set { logger = value; }
      }

      public void SetDockingForm(DockingMainForm dockingForm)
      {
         view = dockingForm;
      }

      public ViewController(Model model)
      {
         this.model = model;
      }

      public void Run()
      {
         if (rrdDbForm == null)
         {
            rrdDbForm = new RrdDbForm();
            rrdDbForm.RegisterController(this);
         }

         TreeForm.RegisterController(this);
         TreeForm.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockLeft;
         view.AddDockingPanel(TreeForm);
         view.AddDockingPanel(rrdDbForm);
      }

      [Obsolete]
      public bool OpenDatabase(string databasePath, bool readOnly)
      {
         //model.Database = new RrdDb(databasePath, readOnly);
         //model.ReadOnly = readOnly;
         //if (readOnly)
         //   model.DatabaseDirty = false;
         ////rrdDbTreeForm.SetDatabaseDefinition(model.DatabaseDefinition, model.DatabaseDirty);
         ////rrdDbForm.SetDatabaseDefinition(model.DatabaseDefinition, model.DatabaseDirty);
         //rrdDbForm.SetDatabaseData(model.DatabaseDefinition, model.DatabaseLastUpdateTime, model.LastValue);
         //rrdDbForm.SetDatasourceData(model.DataSourceDefinitions);
         ////rrdDbForm.SetArchiveData(model.Archives);
         return true;
      }

      public bool OpenDatabase(string databasePath)
      {
         DatabaseData databaseData =  model.AddDatabase(databasePath);
         TreeForm.SetDatabaseDefinition(databaseData);
         rrdDbForm.SetDatabaseDefinition(databaseData);
         TreeForm.SetEditMode(false);
         return true;
      }

      public void NewDatabase(string databaseName, DateTime startTime, long step)
      {
         TreeForm.SetDatabaseDefinition(model.CreateDatabase(databaseName, startTime, step));
         rrdDbForm.SetDatabaseDefinition(model.CreateDatabase(databaseName, startTime, step));
      }

      public void CreateDatabase(string databasePath)
      {
         if (model.EditingDatabaseData == null)
            throw new ApplicationException("Not in edit mode");

         string oldpath = model.EditingDatabaseData.Definition.Path;
         RrdDb srcDatabase = null;
         if (!string.IsNullOrEmpty(model.EditingDatabaseData.SourceDatabasePath))
            srcDatabase = new RrdDb(model.EditingDatabaseData.SourceDatabasePath);
         RrdDef rrdDef = model.EditingDatabaseData.Definition;
         rrdDef.setPath(databasePath);
         RrdDb dstDatabase = new RrdDb(rrdDef);
         if (srcDatabase != null)
            srcDatabase.copyStateTo(dstDatabase);

         if (srcDatabase != null)
            srcDatabase.close();
         dstDatabase.close();
         model.DatabaseDirty = false;
         model.EditingDatabaseData = null;
         DatabaseData databaseData = model.AddDatabase(databasePath);
         rrdDbForm.SetDatabaseDefinition(databaseData);

         TreeForm.RemoveDatabaseDefinition(oldpath);
         TreeForm.SetDatabaseDefinition(databaseData);
         TreeForm.SetEditMode(false);
      }

      public void RemoveDatabase(DatabaseData databaseData)
      {
         if (model.RemoveDatabase(databaseData))
            TreeForm.RemoveDatabaseDefinition(databaseData.Definition.Path);
      }

      public void DatabaseCreated()
      {
         DatabaseData databaseData = model.GetDatabasedata();
         TreeForm.SetDatabaseDefinition(databaseData);
         TreeForm.SetEditMode(true);
         rrdDbForm.SetDatabaseDefinition(databaseData);
      }
      public void DeleteDatabaseDefinition()
      {
         TreeForm.RemoveDatabaseDefinition(model.EditingDatabaseData.Definition.Path);
         model.EditingDatabaseData = null;
         model.DatabaseDirty = false;
      }

      public string GetDatabaseDefinitionPath()
      {
         return model.DatabasePath;
      }

      public void StartEditDatabase(DatabaseData srcDatabaseData)
      {
         DatabaseData dstDatabaseData = model.SetDatabaseAsEdit(srcDatabaseData);
         TreeForm.SetDatabaseDefinition(dstDatabaseData);
         TreeForm.SetEditMode(true);
      }


      public void DatabaseUpdated()
      {
         DatabaseUpdated(null);
      }

      public void DatabaseUpdated(DatabaseData srcDatabaseData)
      {
         DatabaseData reloadedData;
         if (srcDatabaseData != null)
            reloadedData = model.ReloadDatabase(srcDatabaseData);
         else
            reloadedData = model.EditingDatabaseData;
         rrdDbForm.SetDatabaseDefinition(reloadedData);
         TreeForm.SetDatabaseDefinition(reloadedData);
      }

      public void DatabaseSelected(DatabaseData database)
      {
         rrdDbForm.SetDatabaseDefinition(database);
         rrdDbForm.SetDocumentName(Path.GetFileNameWithoutExtension(database.Definition.getPath()));
      }

      [Obsolete]
      private void DatabaseSelected(RrdDef databaseDefinition)
      {
         //rrdDbForm.SetDatabaseData(model.EditingDatabaseData.Definition, model.DatabaseLastUpdateTime, model.LastValue);
      }

      public void DatasourcesSelected(DatabaseData database)
      {
         rrdDbForm.SetDatasourceData(model.GetDataSourceDefinitions(database));
         rrdDbForm.SetDocumentName(Path.GetFileNameWithoutExtension(database.Definition.getPath()));
      }

      public void ArchivesSelected(DatabaseData databaseData)
      {
         List<ArchiveDisplayData> archives = new List<ArchiveDisplayData>();
         foreach (ArcDef arcDef in databaseData.Definition.getArcDefs())
         {
            archives.Add(new ArchiveDisplayData()
            {
               ConsolFunctionName = arcDef.getConsolFun().Name,
               RowCount = arcDef.getRows(),
               Steps = arcDef.getSteps(),
               Xff = arcDef.getXff(),
               StartTime = DateTime.MinValue,
               EndTime = DateTime.MaxValue
            });
         }
         rrdDbForm.SetArchiveData(archives.ToArray());
      }

      public string DumpArchive(object archiveDefinition)
      {
         throw new NotImplementedException("DumpArchive");
      }

      public void DatasourceArchiveSelected(DatabaseData database, object datasource, object archiveDefinition)
      {
         if (!model.IsDatabase(database.Definition.Path))
            return;
         DsDef dsDef = datasource as DsDef;

         ArcDef selectedArchive = archiveDefinition as ArcDef;
         FetchData fetchedData = model.GetArchiveData(database, dsDef.DsName, selectedArchive);
         rrdDbForm.SetArchiveDumpData(fetchedData);
         rrdDbForm.SetDocumentName(Path.GetFileNameWithoutExtension(database.Definition.getPath()));
      }


      public string DumpDatabaseDefinition(DatabaseData database)
      {
         return database.Definition.dump();
      }

      public bool DatabaseUnsaved(DatabaseData databaseData)
      {
         if (model.EditingDatabaseData == null)
            return false;
         return databaseData.Definition.Path == model.EditingDatabaseData.Definition.Path;
      }

      public bool DatabaseUnsaved()
      {
         return model.DatabaseDirty;
      }

      public void CloseApp()
      {
         if (model.EditingDatabaseData == null)
            return;
         if (model.DatabaseDirty)
         {
            string savePath = TreeForm.SaveDataBase(model.EditingDatabaseData.Definition.getPath());
            if (savePath == string.Empty)
               return;
            CreateDatabase(savePath);
         }
      }

      public void ExportDatabase(string databasePath, string xmlFilePath)
      {
         model.ExportDatabase(databasePath, xmlFilePath);
      }
      public void ImportDataIntoDatabase(string databasePath, string xmlFilePath)
      {
         RrdDb database = new RrdDb(databasePath,xmlFilePath);
         model.ExportDatabase(databasePath, xmlFilePath);
      }

      public void ImportData(string dataPath, DatabaseData databaseData, TimeSpan expectedTick )
      {
         if (model.ReadOnly)
            throw new ApplicationException("Can't import data. Database readonly");


         List<string> columns = new List<string>();
         List<FetchedData> unifiedData = ReadAndUnifyData(dataPath, out columns);

         string databasePath = databaseData.Definition.Path;
         RrdDb database = new RrdDb(databasePath, false);

         int[] dsIndexes = new int[columns.Count];
         for (int i = 0; i < columns.Count; i++)
         {
            dsIndexes[i] = database.getDsIndex(columns[i]);
         }


         string[] dsNames = database.getDsNames();
         rrd4n.DataAccess.Data.Sample sample = new rrd4n.DataAccess.Data.Sample(databasePath, dsNames, 0);
         
         foreach (var data in unifiedData)
         {
            sample.setDateTime(data.TimeStamp);
            for (int i = 0; i < data.Values.Count; i++ )
            {
               sample.setValue(dsIndexes[i], data.Values[i]); 
            }

            try
            {
               // When using file access abstraction
               //dbAccess.StoreData(sample);

               //Without abstraction
               database.store(sample);
               sample.clearValues();
            }
            catch (ArgumentException)
            {
            }
            model.DatabaseDirty = true;
         }
         database.close();
         OpenDatabase(databasePath);
      }

      private static List<FetchedData> ReadAndUnifyData(string dataPath, out List<string> columns)
      {
         columns = new List<string>();

         List<FetchedData> rawData = new List<FetchedData>();
         const int valueStartIndex = 1;
         const int dateIndex = 0;

         using (StreamReader reader = new StreamReader(dataPath))
         {
            // Read columns header
            string line = reader.ReadLine();
            string[] dataColumns = line.Split(';');
            for (int i = valueStartIndex; i < dataColumns.Length; i++)
            {
               columns.Add(dataColumns[i]);
            }

            line = reader.ReadLine();

            while (!string.IsNullOrEmpty(line))
            {
               try
               {
                  var values = line.Split(';');
                  DateTime timeStamp = DateTime.Parse(values[dateIndex]);
                  var fetchedData = new FetchedData {TimeStamp = timeStamp};
                  fetchedData.Values = new List<double>(values.Length - 1);
                  for (var i = valueStartIndex; i < values.Length; i++)
                  {
                     fetchedData.Values.Add(double.Parse(values[i]));
                  }
                  rawData.Add(fetchedData);
               }
               catch (ArgumentException)
               {
               }
               line = reader.ReadLine();
            }
            reader.Close();
         } 

         return new List<FetchedData>(rawData);
      }
   }
}
