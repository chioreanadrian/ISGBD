using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace Mini_DBMS.Models
{
    [XmlType("Attributes")]
    public class Field
    {
        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("Type")]
        public FieldType Type { get; set; }

        [XmlElement("Length")]
        public int Length { get; set; }

        public bool IsPrimaryKey { get; set; }
        public bool IsForeignKey { get; set; }
        public bool AllowNull { get; set; }
    }
}