using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mini_DBMS.Models
{
    public class Table
    {
        public string Name { get; set; }
        public List<Field> Fields { get; set; }
    }
}