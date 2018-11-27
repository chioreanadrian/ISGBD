﻿using System;
using Mini_DBMS.Helpers;
using Mini_DBMS.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using DBreeze;
using System.Web.Hosting;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json.Linq;

namespace Mini_DBMS.Controllers
{
    public class HomeController : Controller
    {
        private static Database currentDatabase;
        private static Table currentTable;
        private static List<Database> databases;
        private static XMLOperation XMLOperationHelper;
        private static DBreezeEngine dBreeze;

        public ActionResult Index()
        {
            XMLOperationHelper = new XMLOperation();
            databases = XMLOperationHelper.ReadFromFile();
            dBreeze?.Dispose();
            var folderPath = HostingEnvironment.MapPath("~/Helpers/DBreeze");
            dBreeze = new DBreezeEngine(folderPath);
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

            table.FileName = $"{table.Name.ToLower()}.kv";
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

            return View("Fields", currentTable);
        }

        [HttpGet]
        public ActionResult GoToTables()
        {
            if (!currentDatabase.Tables.Select(x => x.Name).Contains(currentTable.Name))
            {
                currentDatabase.Tables.Add(currentTable);
            }

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
            return PartialView("_AddIndex", currentTable);
        }

        public ActionResult AddIndex(Table table)
        {
            var realField = new Field();
            var realTable = new Table();
            
            foreach (var db in databases)
                foreach (var t in db.Tables)
                    foreach (var f in t.Fields)
                        if (f.Name == table.Index)
                        {
                            realField = f;
                            realTable = t;
                        }

            var indexFile = new IndexFile
            {
                Indexs = new List<Index>() {new Index{ IndexName = table.Index}},
                KeyLength = realField.Length,
                FileName = $"{table.Index}.ind",
                IndexType = table.IndexType,
                IsUnique = table.IndexUnique
            };

            if (!currentTable.IndexFiles.Any(c => c.Indexs.Any(z => z.IndexName == table.Index)))
            {
                if (table.IndexUnique)
                {
                    currentTable.IndexFiles.Add(indexFile);

                    var value = string.Empty;

                    var fieldsNotPk = realTable.Fields.Where(c => realTable.PrimaryKey != c.Name);

                    foreach (var s in fieldsNotPk)
                        if (s.Name != indexFile.Indexs.FirstOrDefault()?.IndexName)
                            value += s.Name + "#";
                    value = value.Remove(value.Length - 1);

                    using (var tranz = dBreeze.GetTransaction())
                    {
                        tranz.Insert(table.Index, indexFile.Indexs.FirstOrDefault()?.IndexName, value );
                        tranz.Commit();
                    }
                }
            }
            else
            {
                   // non-unique index file
            }


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

            if (table != null)
            {
                currentTable = table;
                return View("Fields", currentTable);
            }
            return null;
        }

        public ActionResult ViewData(string tableName)
        {
            var table = currentDatabase.Tables.FirstOrDefault(x => x.Name == tableName);

            if (table != null)
            {
                currentTable = table;
                
            }

            return View(currentTable);
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
                From = currentTable.Name,
                PrimaryKey = primaryKey,
                Values = valueToAdd
            };
            CreateQuery(query);
            return View("ViewData", currentTable);
        }

        public ActionResult _DeleteData(SimpleQuery query)
        {
            query.From = currentTable.Name;
            query.Type = QueryType.Delete;
            return View(query);
        }

        public ActionResult CreateQuery(SimpleQuery query)
        {
            if (query.Type == QueryType.Insert)
            {
                var key = query.PrimaryKey;
                var value = query.Values;                

                using (var tranz = dBreeze.GetTransaction())
                {
                    //check table exists
                    if (dBreeze.Scheme.IfUserTableExists(query.From))
                    {
                        tranz.Insert(query.From, key, value);
                    }
                    else
                    {
                        tranz.InsertTable(query.From, key, 0);
                        tranz.Insert(query.From, key, value);
                    }
                    
                    tranz.Commit();

                    var row = tranz.Select<string, string>(query.From, key);
                    if (row.Exists)
                    {
                        var r = row.Key;
                        var y = row.Value;
                    }
                }
            }
            else
            {
                using (var tranz = dBreeze.GetTransaction())
                {
                    tranz.RemoveKey(query.From, query.PrimaryKey);
                    tranz.Commit();
                    var row = tranz.Select<string, string>(query.From, query.PrimaryKey);
                    if (!row.Exists)
                        Debug.WriteLine("deleted item with success");
                   
                    
                }
            }

            return View("ViewData", currentTable);
        }

        [HttpGet]
        public ActionResult AddPrimaryKey()
        {
            return PartialView("_AddPrimaryKey", currentTable);
        }

        public ActionResult AddPrimaryKey(Table table)
        {
            currentTable.PrimaryKey = table.PrimaryKey;
            return View("Fields", currentTable);
        }

        [HttpGet]
        public ActionResult AddForeignKey(string field)
        {
            var databaseForModel = new Database();
            databaseForModel.Tables = new List<Table>();
            databaseForModel.Tables.AddRange(currentDatabase.Tables);
            databaseForModel.Tables.Remove(currentTable);

            AddForeignKeyModel model = new AddForeignKeyModel
            {
                Field = field,
                Database = databaseForModel
            };

            return PartialView("_AddForeignKey", model);
        }

        public ActionResult AddForeignKey(AddForeignKeyModel model)
        {
            var table = currentDatabase.Tables.FirstOrDefault(x => x.Name == model.ReferencedTable);
            if (table != null)
            {
                if (table.Fields.Select(x => x.Name).Contains(model.ReferencedProperty))
                {
                    if (currentTable.ForeignKeys == null)
                    {
                        currentTable.ForeignKeys = new List<ForeignKey>();
                    }

                    currentTable.ForeignKeys.Add(new ForeignKey
                    {
                        ParentField = model.Field,
                        Field = model.ReferencedProperty,
                        OriginTable = model.ReferencedTable
                    });
                }
            }

            return View("Fields", currentTable);

        }

    }
}