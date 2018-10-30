using System.Collections.Generic;
using System.Xml.Serialization;

namespace Mini_DBMS.Models
{
    [XmlType("Database")]
    public class Database
    {
        [XmlAttribute("databaseName")]
        public string Name { get; set; }

        [XmlArray("Tables")]
        public List<Table> Tables { get; set; }
    }
}