using Mini_DBMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mini_DBMS.Controllers
{
    public class HomeController : Controller
    {
        private static List<Database> databases;
        private static Database currentDatabase;
        private static Table currentTable;

        public HomeController()
        {
            databases = new List<Database>();
        }

        public ActionResult Index()
        {
            // get databases from file
            return View(databases);
        }

        [HttpGet]
        public ActionResult CreateDatabase() => PartialView("_CreateDatabase", new Database());

        public ActionResult CreateDatabase(Database database)
        {

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
            databases.Add(currentDatabase);

            return View("Index", databases);
        }
    }
}