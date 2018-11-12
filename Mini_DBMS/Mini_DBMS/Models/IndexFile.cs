using System.Collections.Generic;
using System.Xml.Serialization;

namespace Mini_DBMS.Models
{
    [XmlType("IndexFile")]
    public class IndexFile
    {
        [XmlElement("IndexAttributes")]
        public List<Index> Indexs { get; set; }

        [XmlAttribute("fileName")]
        public string FileName { get; set; }

        [XmlAttribute("keyLength")]
        public int KeyLength { get; set; }

        [XmlAttribute("indexType")]
        public IndexType IndexType { get; set; }

        [XmlAttribute("isUnique")]
        public bool IsUnique { get; set; }
    }
}