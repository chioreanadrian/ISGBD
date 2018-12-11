
using System.Collections.Generic;

namespace Mini_DBMS.Models
{
    public class ConditionalDataModel
    {
        public Table Table { get; set; }
        public string FieldValue { get; set; }
        public FieldType FieldType { get; set; }
        public ConditionType ConditionType { get; set; }
        public string SearchedValue { get; set; }
        public Dictionary<string, string> ValuesFound { get; set; }
    }
}