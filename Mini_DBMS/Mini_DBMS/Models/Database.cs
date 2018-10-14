using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mini_DBMS.Models
{
    public class Database
    {
        public string Name { get; set; }
        public List<Table> Tables { get; set; }
    }
}