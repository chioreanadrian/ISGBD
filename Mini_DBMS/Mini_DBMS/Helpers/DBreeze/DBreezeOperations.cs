using System.Diagnostics;
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

        public static void DeleteData(DBreezeEngine dBreeze, SimpleQuery query)
        {
            using (var transaction = dBreeze.GetTransaction())
            {
                transaction.RemoveKey(query.From, query.PrimaryKey);
                transaction.Commit();
                var row = transaction.Select<string, string>(query.From, query.PrimaryKey);
                if (!row.Exists)
                    Debug.WriteLine("deleted item with success");
            }
        }
    }
}