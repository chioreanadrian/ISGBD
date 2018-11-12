using System.Xml.Serialization;

namespace Mini_DBMS.Models
{
    [XmlType("IndexAttributes")]
    public class Index
    {
        [XmlElement("IAttribute")]
        public string IndexName { get; set; }
    }
}