using System.Collections.Generic;
using System.Xml.Serialization;

namespace Mini_DBMS.Models
{
    [XmlType("Tables")]
    public class Table
    {
        [XmlElement("TableName")]
        public string Name { get; set; }

        [XmlElement("Attributes")]
        public List<Field> Fields { get; set; }
    }
}