using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace preview.Models
{
    public class MongodbModel
    {
        public string name { set; get; }
        public string type { set; get; }
        public string count { set; get; }

        public string content { set; get; }
    }
}
