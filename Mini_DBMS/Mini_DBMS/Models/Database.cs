using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace Mini_DBMS.Models
{
    [XmlType("Databases")]
    public class Database
    {
        [XmlElement("DatabaseName")]
        public string Name { get; set; }

        [XmlElement("Tables")]
        public List<Table> Tables { get; set; }
    }
}