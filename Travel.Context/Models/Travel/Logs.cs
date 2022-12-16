using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel.Context.Models.Travel
{
    public class Logs
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public string EmailCreator { get; set; }
        public string Type { get; set; }
        public long CreationDate { get; set; }
        public string ClassContent { get; set; }
    }
}
