using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mini_DBMS.Models
{
    public class SimpleQuery
    {
        public QueryType Type { get; set; }

        public string From { get; set; }

        public string PrimaryKey { get; set; }
        public string Values { get; set; }
    }
}