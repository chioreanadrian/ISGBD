using System.Collections.Generic;

namespace Mini_DBMS.Models
{
    public class Table
    {
        public string Name { get; set; }
        public List<Field> Fields { get; set; }
    }
}