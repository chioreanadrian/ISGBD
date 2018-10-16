using Mini_DBMS.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
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

            var currentDirectory = Environment.CurrentDirectory;
            //var stringulet = Environment.GetFolderPath(Environment.CurrentDirectory);
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

            string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "//DBMS_test.xml";
            TextWriter fileStream = new StreamWriter(folderPath);
            serializer.Serialize(fileStream, databases);
            fileStream.Close();
        }

    }
}