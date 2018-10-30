using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mini_DBMS.Models
{
    public class AddForeignKeyModel
    {
        public string Field { get; set; }
        public Database Database { get; set; }
        public string ReferencedTable { get; set; }
        public string ReferencedProperty { get; set; }
    }
}