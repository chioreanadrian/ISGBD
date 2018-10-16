using System;
using Mini_DBMS.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;
using Mini_DBMS.Helpers;

namespace Mini_DBMS.Controllers
{
    public class HomeController : Controller
    {
        private static Database currentDatabase;
        private static Table currentTable;
        //private static string folderPath;
        private static List<Database> databases;
        private static XMLOperation XMLOperationHelper;

        public ActionResult Index()
        {
            XMLOperationHelper = new XMLOperation();
            databases = XMLOperationHelper.ReadFromFile();
            //var xmlStr = System.IO.File.ReadAllText(folderPath);
            //var str = XElement.Parse(xmlStr);

            //var result = str.Elements("Database").ToList();

            return View(databases);
        }

        [HttpGet]
        public ActionResult CreateDatabase() => PartialView("_CreateDatabase", new Database());

        public ActionResult CreateDatabase(Database database)
        { 
            //SaveDBToXML(database);
            currentDatabase = database;
            currentDatabase.Tables = new List<Table>();

            return View("Tables", currentDatabase);
        }

        [HttpGet]
        public ActionResult CreateTable() => PartialView("_CreateTable", new Table());

        public ActionResult CreateTable(Table table)
        {
            currentTable = table;
            currentTable.Fields = new List<Field>();
            //SaveTableToXML(table, currentDatabase);

            return View("Fields", currentTable);
        }

        [HttpGet]
        public ActionResult CreateField() => PartialView("_AddField", new Field());

        public ActionResult CreateField(Field field)
        {
            currentTable.Fields.Add(field);

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
            return View("Index", databases);
        }

        //public void SaveDBToXML(Database db)
        //{
        //    XDocument doc = XDocument.Load(folderPath);
        //    XElement root = new XElement("Database");
        //    root.Add(new XAttribute("name", db.Name));
        //    root.Add(new XElement("tables", db.Tables));
        //    doc.Element("Databases").Add(root);
        //    doc.Save(folderPath);
        //}

        //public void SaveTableToXML(Table table, Database db)
        //{
        //    var xmlStr = System.IO.File.ReadAllText(folderPath);
        //    var str = XElement.Parse(xmlStr);

        //    var database = str.Elements("Database").FirstOrDefault(x => x.FirstAttribute.Value == db.Name);

        //    database?.Element("tables").Add();
        //}

        public ActionResult DeleteDatabase(string databaseName)
        {
            Database databaseToDelete = databases.FirstOrDefault(x => x.Name == databaseName);

            if (databaseToDelete != null)
            {
                databases.Remove(databaseToDelete);
            }

            return View("Index", databases);
        }

        public ActionResult DeleteTable(string tableName)
        {
            Table tableToDelete = currentDatabase.Tables.FirstOrDefault(x => x.Name == tableName);

            if(tableToDelete != null)
            {
                currentDatabase.Tables.Remove(tableToDelete);
            }

            return View("Tables", currentDatabase);
        }
    }
}