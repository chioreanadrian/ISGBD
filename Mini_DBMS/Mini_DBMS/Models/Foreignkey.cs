using System.Xml.Serialization;

namespace Mini_DBMS.Models
{
    [XmlType("ForeignKey")]
    public class ForeignKey
    {
        [XmlAttribute("parentField")]
        public string ParentField { get; set; }

        [XmlAttribute("refAttribute")]
        public string Field { get; set; }

        [XmlAttribute("parentTable")]
        public string OriginTable { get; set; }
    }
}