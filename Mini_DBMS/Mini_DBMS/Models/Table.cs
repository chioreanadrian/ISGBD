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

        [XmlElement("IndexAttribute")]
        public Field Index { get; set; }

        [XmlElement("PrimaryKey")]
        public Field PrimaryKey { get; set; }

        [XmlElement("ForeignKey")]
        public FK ForeignKey { get; set; }
    }
}