using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mini_DBMS.Models
{
    public class Field
    {
        public string Name { get; set; }
        public FieldType Type { get; set; }
        public int Length { get; set; }
        public bool IsPrimaryKey { get; set; }
        public bool IsForeignKey { get; set; }
        public bool AllowNull { get; set; }
    }
}