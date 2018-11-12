using System.Xml.Serialization;

namespace Mini_DBMS.Models
{
    [XmlType("IndexFile")]
    public class IndexFile
    {
        [XmlElement("IAttribute")]
        public string IndexName { get; set; }

        [XmlAttribute("fileName")]
        public string FileName { get; set; }

        [XmlAttribute("keyLength")]
        public int KeyLength { get; set; }

        [XmlAttribute("indexType")]
        public IndexType IndexType { get; set; }
    }
}