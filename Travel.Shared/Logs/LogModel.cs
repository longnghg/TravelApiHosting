using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel.Shared.Logs
{
    public class LogModel
    {
        public string Name { get; set; }
        public string MethodName { get; set; }
        public string Status { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
