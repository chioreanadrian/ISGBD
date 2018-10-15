using Mini_DBMS.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace Mini_DBMS.Helpers
{
    public class XMLOperation
    {
        public List<Database> ReadFromFile()
        {
            List<Database> databases = new List<Database>();
            databases.Add(new Database { Name = "database1", Tables = new List<Table> { new Table { Name = "table1", Fields = new List<Field> { new Field { Name = "field1" } } } } });
            databases.Add(new Database { Name = "database2", Tables = new List<Table>() { new Table { Name = "table2", Fields = new List<Field> { new Field { Name = "field2" } } } } });
            databases.Add(new Database { Name = "database3", Tables = new List<Table>() { new Table { Name = "table3", Fields = new List<Field> { new Field { Name = "field3" } } } } });

            return databases;
        }

        public void WriteToFile(List<Database> databases)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Database>));

            string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "//DBMS_test.xml";
            TextWriter fileStream = new StreamWriter(folderPath);
            serializer.Serialize(fileStream, databases);
            fileStream.Close();
        }

    }
}