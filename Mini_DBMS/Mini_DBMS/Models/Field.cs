using System.Xml.Serialization;

namespace Mini_DBMS.Models
{
    [XmlType("Attribute")]
    public class Field
    {
        [XmlAttribute("attributeName")]
        public string Name { get; set; }

        [XmlAttribute("type")]
        public FieldType Type { get; set; }

        [XmlAttribute("length")]
        public int Length { get; set; }

        [XmlAttribute("isnull")]
        public bool AllowNull { get; set; }
    }
}