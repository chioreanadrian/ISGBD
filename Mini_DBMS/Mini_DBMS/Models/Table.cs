using System.Collections.Generic;
using System.Web.Script.Serialization;
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

        [XmlArray("IndexFiles")]
        public List<IndexFile> IndexFiles { get; set; }

        [XmlIgnore]
        [ScriptIgnore]
        public string Index { get; set; }
        [XmlIgnore]
        [ScriptIgnore]
        public IndexType IndexType { get; set; }
    }
}