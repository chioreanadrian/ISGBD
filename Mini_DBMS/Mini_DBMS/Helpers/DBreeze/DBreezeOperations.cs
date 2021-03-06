﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using DBreeze;
using Mini_DBMS.Models;

namespace Mini_DBMS.Helpers.DBreeze
{
    public static class DBreezeOperations
    {
        public static void AddData(DBreezeEngine dBreeze, SimpleQuery query)
        {
            var key = query.PrimaryKey;
            var value = query.Values;

            using (var transaction = dBreeze.GetTransaction())
            {
                //check table exists
                if (dBreeze.Scheme.IfUserTableExists(query.From))
                {
                    transaction.Insert(query.From, key, value);

                }
                else
                {
                    transaction.InsertTable(query.From, key, 0);
                    transaction.Insert(query.From, key, value);
                }

                transaction.Commit();

                var row = transaction.Select<string, string>(query.From, key);
                if (row.Exists)
                {
                    // do something here
                    var r = row.Key;
                    var y = row.Value;
                }
            }
        }

        public static bool DeleteData(DBreezeEngine dBreeze, SimpleQuery query, Table currentTable, Database currentDatabase)
        {
            using (var transaction = dBreeze.GetTransaction())
            {
                var dataToDelete = GetAllData(dBreeze, currentTable.Name).FirstOrDefault(x => x.Key == query.PrimaryKey);

                if (dataToDelete.Key == null)
                {
                    return true;
                }
                foreach (var foreignKey in currentTable.ForeignKeys)
                {
                    var data = GetAllData(dBreeze, foreignKey.OriginTable);
                    foreach(var pair in data)
                        if (dataToDelete.Value.Split('#').Contains(pair.Value))
                        {
                            Debug.WriteLine("Can't delete! It's a foreign key!");
                            return false;
                        }
                }
            

                transaction.RemoveKey(query.From, query.PrimaryKey);
                transaction.Commit();
                var row = transaction.Select<string, string>(query.From, query.PrimaryKey);
                if (!row.Exists)
                {
                    Debug.WriteLine("item deleted with success");
                    return true;
                }
            }

            return true;
        }

        public static Dictionary<string, string> GetAllData(DBreezeEngine dBreeze, string tableName)
        {
            var dic = new Dictionary<string, string>();

            using (var transaction = dBreeze.GetTransaction())
            {
                foreach (var row in transaction.SelectForward<string, string>(tableName))
                    dic.Add(row.Key, row.Value);
            }

            return dic;
        }

        public static Dictionary<string, string> GetConditionedData(DBreezeEngine dBreeze, Table table, string field, FieldType fieldType, ConditionType condition, string value)
        {
            var dic = new Dictionary<string, string>();

            using (var transaction = dBreeze.GetTransaction())
            {
                if (dBreeze.Scheme.IfUserTableExists(field)) // search after index
                {
                    foreach (var row in transaction.SelectForward<string, string>(field))
                        dic.Add(row.Key, row.Value);
                }
                else if (dBreeze.Scheme.IfUserTableExists(table.Name)) // search after table
                {
                    foreach (var row in transaction.SelectForward<string, string>(table.Name))
                        dic.Add(row.Key, row.Value);
                }
            }

            var returnDic = new Dictionary<string, string>();

            var fieldCount = 1;
            var ftype = FieldType.varchar; // nu conteaza initializarea

            var fields = table.Fields.Where(c => c.Name != table.PrimaryKey);

            foreach (var f in fields)
                if (f.Name != field)
                {
                    fieldCount += 1;
                }
                else
                {
                    ftype = f.Type;
                    break;
                }


            foreach (var pair in dic)
            {
                var values = pair.Value.Split('#');
                var fieldCautat = values[values.Length - 1];
                switch (condition)
                {
                    //string
                    case ConditionType.Equal
                        when fieldCautat == value && fieldType == FieldType.varchar:
                        returnDic.Add(pair.Key, pair.Value);
                        break;

                    //number
                    case ConditionType.Equal
                        when fieldType == FieldType.number && Int32.Parse(fieldCautat) == Int32.Parse(value):
                        returnDic.Add(pair.Key, pair.Value);
                        break;
                    case ConditionType.Less
                        when fieldType == FieldType.number && Int32.Parse(fieldCautat) < Int32.Parse(value):
                        returnDic.Add(pair.Key, pair.Value);
                        break;
                    case ConditionType.Greater
                        when fieldType == FieldType.number && Int32.Parse(fieldCautat) > Int32.Parse(value):
                        returnDic.Add(pair.Key, pair.Value);
                        break;

                    //datetime
                    case ConditionType.Equal
                        when fieldType == FieldType.datetime && DateTime.Parse(fieldCautat) == DateTime.Parse(value):
                        returnDic.Add(pair.Key, pair.Value);
                        break;
                    case ConditionType.Less
                        when fieldType == FieldType.datetime && DateTime.Parse(fieldCautat) < DateTime.Parse(value):
                        returnDic.Add(pair.Key, pair.Value);
                        break;
                    case ConditionType.Greater
                        when fieldType == FieldType.datetime && DateTime.Parse(fieldCautat) > DateTime.Parse(value):
                        returnDic.Add(pair.Key, pair.Value);
                        break;
                }
            }



            return dic;
        }

        public static Dictionary<string, string> GetDataPK(DBreezeEngine dBreeze, Table table, string pk, FieldType pkFieldType, ConditionType condition, string value)
        {
            var dic = new Dictionary<string, string>();

            using (var transaction = dBreeze.GetTransaction())
            {
                if (dBreeze.Scheme.IfUserTableExists(pk))
                {
                    foreach (var row in transaction.SelectForward<string, string>(pk))
                        dic.Add(row.Key, row.Value);
                }
                else
                    foreach (var row in transaction.SelectForward<string, string>(table.Name))
                        dic.Add(row.Key, row.Value);
            }

            var returnDic = new Dictionary<string, string>();

            if (table.PrimaryKey == pk)
                foreach (var pair in dic)
                    switch (condition)
                    {
                        //number
                        case ConditionType.Equal
                            when Int32.Parse(pair.Key) == Int32.Parse(value) && pkFieldType == FieldType.number:
                            returnDic.Add(pair.Key, pair.Value);
                            break;
                        case ConditionType.Less
                            when Int32.Parse(pair.Key) < Int32.Parse(value) && pkFieldType == FieldType.number:
                            returnDic.Add(pair.Key, pair.Value);
                            break;
                        case ConditionType.Greater
                            when Int32.Parse(pair.Key) > Int32.Parse(value) && pkFieldType == FieldType.number:
                            returnDic.Add(pair.Key, pair.Value);
                            break;

                        //string
                        case ConditionType.Equal
                            when pair.Key == value && pkFieldType == FieldType.varchar:
                            returnDic.Add(pair.Key, pair.Value);
                            break;

                        //datetime
                        case ConditionType.Equal
                            when DateTime.Parse(pair.Key) == DateTime.Parse(value) && pkFieldType == FieldType.datetime:
                            returnDic.Add(pair.Key, pair.Value);
                            break;
                        case ConditionType.Less
                            when DateTime.Parse(pair.Key) < DateTime.Parse(value) && pkFieldType == FieldType.datetime:
                            returnDic.Add(pair.Key, pair.Value);
                            break;
                        case ConditionType.Greater
                            when DateTime.Parse(pair.Key) > DateTime.Parse(value) && pkFieldType == FieldType.datetime:
                            returnDic.Add(pair.Key, pair.Value);
                            break;
                    }
            return dic;
        }
    }
}