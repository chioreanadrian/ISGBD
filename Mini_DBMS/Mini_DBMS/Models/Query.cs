using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mini_DBMS.Models
{
    public class Query
    {
        public string Select { get; set; }
        public string From { get; set; }

        public string Where { get; set; }

    }
}