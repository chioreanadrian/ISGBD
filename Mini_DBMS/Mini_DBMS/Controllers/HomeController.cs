using Mini_DBMS.Helpers;
using Mini_DBMS.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DBreeze;
using System.Web.Hosting;
using Mini_DBMS.Helpers.DBreeze;

namespace Mini_DBMS.Controllers
{
    public class HomeController : Controller
    {
        private static Database _currentDatabase;
        private static Table _currentTable;
        private static List<Database> _databases;
        private static DBreezeEngine _dBreeze;

        public ActionResult Index()
        {
            _databases = XMLOperation.ReadFromFile();
            _dBreeze?.Dispose();
            var folderPath = HostingEnvironment.MapPath("~/Helpers/DBreeze");
            _dBreeze = new DBreezeEngine(folderPath);
            return View(_databases);
        }

        [HttpGet]
        public ActionResult CreateDatabase() => PartialView("_CreateDatabase", new Database());

        public ActionResult CreateDatabase(Database database)
        {
            _currentDatabase = database;
            _currentDatabase.Tables = new List<Table>();

            if (_databases.Select(d => d.Name).ToList().Contains(database.Name)) return View("Index", _databases);

            _databases.Add(database);
            XMLOperation.WriteToFile(_databases);

            return View("Tables", _currentDatabase);
        }

        [HttpGet]
        public ActionResult CreateTable() => PartialView("_CreateTable", new Table());

        public ActionResult CreateTable(Table table)
        {
            foreach (var t in _currentDatabase.Tables)
                if (t.Name == table.Name)
                    return View("Tables", _currentDatabase);

            table.FileName = $"{table.Name.ToLower()}.kv";
            _currentTable = table;

            _currentTable.Fields = new List<Field>();

            return View("Fields", _currentTable);
        }

        [HttpGet]
        public ActionResult CreateField() => PartialView("_AddField", new Field());

        public ActionResult CreateField(Field field)
        {
            foreach (var f in _currentTable.Fields)
                if (f.Name == field.Name)
                    return View("Fields", _currentTable);

            _currentTable.Fields.Add(field);

            return View("Fields", _currentTable);
        }

        [HttpGet]
        public ActionResult GoToTables()
        {
            if (!_currentDatabase.Tables.Select(x => x.Name).Contains(_currentTable.Name))
            {
                _currentDatabase.Tables.Add(_currentTable);
            }

            return View("Tables", _currentDatabase);
        }

        [HttpGet]
        public ActionResult GoToDatabases()
        {
            XMLOperation.WriteToFile(_databases);

            return View("Index", _databases);
        }

        public ActionResult DeleteDatabase(string databaseName)
        {
            Database databaseToDelete = _databases.FirstOrDefault(x => x.Name == databaseName);

            if (databaseToDelete != null)
            {
                _databases.Remove(databaseToDelete);
            }

            XMLOperation.WriteToFile(_databases);

            return View("Index", _databases);
        }

        public ActionResult DeleteTable(string tableName)
        {
            Table tableToDelete = _currentDatabase.Tables.FirstOrDefault(x => x.Name == tableName);

            if (tableToDelete != null)
            {
                _currentDatabase.Tables.Remove(tableToDelete);
            }

            return View("Tables", _currentDatabase);
        }

        [HttpGet]
        public ActionResult AddIndex()
        {
            return PartialView("_AddIndex", _currentTable);
        }

        public ActionResult AddIndex(Table table)
        {
            var realField = new Field();
            var realTable = new Table();
            
            foreach (var db in _databases)
                foreach (var t in db.Tables)
                    if(t.Name == table.Name)
                        foreach (var f in t.Fields)
                            if (f.Name == table.Index)
                            {
                                realField = f;
                                realTable = t;
                            }

            var indexFile = new IndexFile
            {
                Indexs = new List<Index>() {new Index{ IndexName = table.FileName}}, // index name is set on FileName attribute in _AddIndex.cshtml
                KeyLength = realField.Length,
                FileName = $"{table.FileName}.ind",
                IndexType = table.IndexType,
                IsUnique = table.IndexUnique
            };

            if (!_currentTable.IndexFiles.Any(c => c.Indexs.Any(z => z.IndexName == table.Index)))
            {
                if (table.IndexUnique)
                {
                    _currentTable.IndexFiles.Add(indexFile);

                    var value = string.Empty;

                    var fieldsNotPk = realTable.Fields.Where(c => realTable.PrimaryKey != c.Name);

                    foreach (var s in fieldsNotPk)
                        if (s.Name != indexFile.Indexs.FirstOrDefault()?.IndexName)
                            value += s.Name + "#";
                    value = value.Remove(value.Length - 1);

                    using (var tranz = _dBreeze.GetTransaction())
                    {
                        if (!_dBreeze.Scheme.IfUserTableExists(table.FileName))
                        {
                            tranz.InsertTable(table.FileName, table.Index, 0);

                            var pairs = DBreezeOperations.GetAllData(_dBreeze, table.Name).OrderBy(c => c.Key);

                            foreach (var v in pairs)
                                tranz.Insert(table.Index, v.Key, v.Value);
                        }

                        tranz.Commit();
                    }
                }
            }
            else
            {
                   // non-unique index file
            }


            return View("Fields", _currentTable);
        }

        public ActionResult ViewDatabase(string databaseName)
        {
            var database = _databases.FirstOrDefault(x => x.Name == databaseName);

            if (database != null)
            {
                _currentDatabase = database;
                return View("Tables", _currentDatabase);
            }

            return null;
        }
        public ActionResult ViewFields(string tableName)
        {
            var table = _currentDatabase.Tables.FirstOrDefault(t => t.Name == tableName);

            if (table != null)
            {
                _currentTable = table;
                return View("Fields", _currentTable);
            }
            return null;
        }

        public ActionResult ViewData(string tableName)
        {
            var table = _currentDatabase.Tables.FirstOrDefault(x => x.Name == tableName);

            if (table != null)
            {
                var dataList = DBreezeOperations.GetAllData(_dBreeze, table.Name);

                if (dataList == null)
                {
                    table.DataList = new Dictionary<string, string>();
                }
                else
                {
                    table.DataList = dataList;
                }

                _currentTable = table;
                
            }

            return View(_currentTable);
        }

        [HttpPost]
        public ActionResult _AddData(string[] values)
        {
            var primaryKey = values[0];

            string valueToAdd = string.Empty;
            for (int index = 1; index < values.Length - 1; index++)
            {
                valueToAdd += string.Format("{0}#", values[index]);
            }

            valueToAdd += values[values.Length - 1];

            SimpleQuery query = new SimpleQuery
            {
                Type = QueryType.Insert,
                From = _currentTable.Name,
                PrimaryKey = primaryKey,
                Values = valueToAdd
            };
            CreateQuery(query);
            return View("ViewData", _currentTable);
        }

        public ActionResult _DeleteData(SimpleQuery query)
        {
            query.From = _currentTable.Name;
            query.Type = QueryType.Delete;
            return View(query);
        }

        public ActionResult CreateQuery(SimpleQuery query)
        {
            if (query.Type == QueryType.Insert)
            {
                DBreezeOperations.AddData(_dBreeze, query);
                _currentTable.DataList = DBreezeOperations.GetAllData(_dBreeze, _currentTable.Name);
                //DBreezeOperations.GetDataPK(_dBreeze, _currentTable, _currentTable.PrimaryKey, FieldType.number, ConditionType.Less, "10");
                //DBreezeOperations.GetConditionedData(_dBreeze, _currentTable, "firsstname", FieldType.varchar, ConditionType.Equal, "ell");
            }
            else
            {
                DBreezeOperations.DeleteData(_dBreeze, query, _currentTable,_currentDatabase);
            }

            return View("ViewData", _currentTable);
        }

        [HttpGet]
        public ActionResult AddPrimaryKey()
        {
            return PartialView("_AddPrimaryKey", _currentTable);
        }

        public ActionResult AddPrimaryKey(Table table)
        {
            _currentTable.PrimaryKey = table.PrimaryKey;
            return View("Fields", _currentTable);
        }

        [HttpGet]
        public ActionResult AddForeignKey(string field)
        {
            var databaseForModel = new Database();
            databaseForModel.Tables = new List<Table>();
            databaseForModel.Tables.AddRange(_currentDatabase.Tables);
            databaseForModel.Tables.Remove(_currentTable);

            AddForeignKeyModel model = new AddForeignKeyModel
            {
                Field = field,
                Database = databaseForModel
            };

            return PartialView("_AddForeignKey", model);
        }

        public ActionResult AddForeignKey(AddForeignKeyModel model)
        {
            var table = _currentDatabase.Tables.FirstOrDefault(x => x.Name == model.ReferencedTable);
            if (table != null)
            {
                if (table.Fields.Select(x => x.Name).Contains(model.ReferencedProperty))
                {
                    if (_currentTable.ForeignKeys == null)
                    {
                        _currentTable.ForeignKeys = new List<ForeignKey>();
                    }

                    _currentTable.ForeignKeys.Add(new ForeignKey
                    {
                        ParentField = model.Field,
                        Field = model.ReferencedProperty,
                        OriginTable = model.ReferencedTable
                    });
                }
            }

            return View("Fields", _currentTable);

        }

    }
}