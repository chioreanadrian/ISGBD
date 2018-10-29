using System.Xml.Serialization;

namespace Mini_DBMS.Models
{
    [XmlType("ForeignKey")]
    public class ForeignKey
    {
        [XmlAttribute("refAttribute")]
        public string Field { get; set; }

        [XmlAttribute("parentTable")]
        public string OriginTable { get; set; }
    }
}