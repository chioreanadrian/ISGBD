using Mini_DBMS.Helpers;
using Mini_DBMS.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Mini_DBMS.Controllers
{
    public class HomeController : Controller
    {
        private static Database currentDatabase;
        private static Table currentTable;
        private static List<Database> databases;
        private static XMLOperation XMLOperationHelper;

        public ActionResult Index()
        {
            XMLOperationHelper = new XMLOperation();
            databases = XMLOperationHelper.ReadFromFile();

            return View(databases);
        }

        [HttpGet]
        public ActionResult CreateDatabase() => PartialView("_CreateDatabase", new Database());

        public ActionResult CreateDatabase(Database database)
        {
            currentDatabase = database;
            currentDatabase.Tables = new List<Table>();

            if (databases.Select(d => d.Name).ToList().Contains(database.Name)) return View("Index", databases);

            databases.Add(database);
            XMLOperationHelper.WriteToFile(databases);

            return View("Tables", currentDatabase);
        }

        [HttpGet]
        public ActionResult CreateTable() => PartialView("_CreateTable", new Table());

        public ActionResult CreateTable(Table table)
        {
            foreach (var t in currentDatabase.Tables)
                if (t.Name == table.Name)
                    return View("Tables", currentDatabase);

            currentTable = table;
            currentTable.Fields = new List<Field>();

            return View("Fields", currentTable);
        }

        [HttpGet]
        public ActionResult CreateField() => PartialView("_AddField", new Field());

        public ActionResult CreateField(Field field)
        {
            foreach (var f in currentTable.Fields)
                if (f.Name == field.Name)
                    return View("Fields", currentTable);

            currentTable.Fields.Add(field);
            if (field.IsPrimaryKey && !field.IsForeignKey)
                currentTable.PrimaryKey = field;
            if (field.IsForeignKey && !field.IsPrimaryKey)
                currentTable.ForeignKey = new FK
                {
                    Field = field,
                    OriginTable = new Table()
                };

            return View("Fields", currentTable);
        }

        [HttpGet]
        public ActionResult GoToTables()
        {
            currentDatabase.Tables.Add(currentTable);
            return View("Tables", currentDatabase);
        }

        [HttpGet]
        public ActionResult GoToDatabases()
        {
            XMLOperationHelper.WriteToFile(databases);

            return View("Index", databases);
        }

        public ActionResult DeleteDatabase(string databaseName)
        {
            Database databaseToDelete = databases.FirstOrDefault(x => x.Name == databaseName);

            if (databaseToDelete != null)
            {
                databases.Remove(databaseToDelete);
            }

            XMLOperationHelper.WriteToFile(databases);

            return View("Index", databases);
        }

        public ActionResult DeleteTable(string tableName)
        {
            Table tableToDelete = currentDatabase.Tables.FirstOrDefault(x => x.Name == tableName);

            if (tableToDelete != null)
            {
                currentDatabase.Tables.Remove(tableToDelete);
            }

            return View("Tables", currentDatabase);
        }

        [HttpGet]
        public ActionResult AddIndex()
        {
            currentTable.Index = new Field();
            return PartialView("_AddIndex", currentTable);
        }

        public ActionResult AddIndex(Table table)
        {
            currentTable.Index = table.Index;
            return View("Fields", currentTable);
        }

        public ActionResult ViewDatabase(string databaseName)
        {
            var database = databases.FirstOrDefault(x => x.Name == databaseName);

            if (database != null)
            {
                currentDatabase = database;
                return View("Tables", currentDatabase);
            }

            return null;
        }
        public ActionResult ViewFields(string tableName)
        {
            var table = currentDatabase.Tables.FirstOrDefault(t => t.Name == tableName);
            
            if(table != null)
            {
                currentTable = table;
                return View("Fields", currentTable);
            }
            return null;
        }
    }
}