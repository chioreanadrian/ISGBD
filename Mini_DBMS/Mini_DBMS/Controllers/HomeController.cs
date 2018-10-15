using System;
using Mini_DBMS.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;

namespace Mini_DBMS.Controllers
{
    public class HomeController : Controller
    {
        private static Database currentDatabase;
        private static Table currentTable;
        private static string folderPath;
        private static List<XElement> databasesXElements;

        public HomeController()
        {
            folderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "//DBMS.xml";
        }

        public ActionResult Index()
        {
            var xmlStr = System.IO.File.ReadAllText(folderPath);
            var str = XElement.Parse(xmlStr);

            var result = str.Elements("Database").ToList();

            return View(result);
        }

        [HttpGet]
        public ActionResult CreateDatabase() => PartialView("_CreateDatabase", new Database());

        public ActionResult CreateDatabase(Database database)
        { 
            SaveDBToXML(database);
            currentDatabase = database;
            return View("Tables", currentDatabase);
        }

        [HttpGet]
        public ActionResult CreateTable() => PartialView("_CreateTable", new Table());

        public ActionResult CreateTable(Table table)
        {
            currentTable = table;
            SaveTableToXML(table, currentDatabase);

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

            return View("Tables");
        }

        [HttpGet]
        public ActionResult GoToDatabases()
        {
            return View("Index");
        }

        public void SaveDBToXML(Database db)
        {
            XDocument doc = XDocument.Load(folderPath);
            XElement root = new XElement("Database");
            root.Add(new XAttribute("name", db.Name));
            root.Add(new XElement("tables", db.Tables));
            doc.Element("Databases").Add(root);
            doc.Save(folderPath);
        }

        public void SaveTableToXML(Table table, Database db)
        {
            var xmlStr = System.IO.File.ReadAllText(folderPath);
            var str = XElement.Parse(xmlStr);

            var database = str.Elements("Database").FirstOrDefault(x => x.FirstAttribute.Value == db.Name);

            database?.Element("tables").Add();
        }
    }
}