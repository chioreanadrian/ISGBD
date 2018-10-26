using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mini_DBMS.Models
{
    public class FK
    {
        public Field Field { get; set; }

        public Table OriginTable { get; set; }
    }
}