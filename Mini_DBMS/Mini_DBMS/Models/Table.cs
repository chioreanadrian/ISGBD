using System.Collections.Generic;
using System.Xml.Serialization;

namespace Mini_DBMS.Models
{
    [XmlType("Table")]
    public class Table
    {        
        [XmlAttribute("tableName")]
        public string Name { get; set; }

        [XmlAttribute("fileName")]
        public string FileName { get; set; }

        [XmlArray("Attributes")]
        public List<Field> Fields { get; set; }

        [XmlElement("PrimaryKey")]
        public string PrimaryKey { get; set; }

        [XmlArray("ForeignKeys")]
        public List<ForeignKey> ForeignKeys { get; set; }

        [XmlElement("IndexAttribute")]
        public string Index { get; set; }
    }
}