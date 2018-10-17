using Mini_DBMS.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Hosting;
using System.Xml;
using System.Xml.Serialization;

namespace Mini_DBMS.Helpers
{
    public class XMLOperation
    {
        public List<Database> ReadFromFile()
        {
            List<Database> databases = new List<Database>();

            var serializer = new XmlSerializer(typeof(List<Database>));

            string folderPath = HostingEnvironment.MapPath("~/Helpers/databases.xml");

            using (var stream = new FileStream(folderPath, FileMode.Open))
            {
                using(var reader = XmlReader.Create(stream))
                {
                    databases = (List<Database>)serializer.Deserialize(reader);
                }
            }

            return databases;
        }

        public void WriteToFile(List<Database> databases)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Database>));

            string folderPath = HostingEnvironment.MapPath("~/Helpers/databases.xml");
            TextWriter fileStream = new StreamWriter(folderPath);
            serializer.Serialize(fileStream, databases);
            fileStream.Close();
        }

    }
}