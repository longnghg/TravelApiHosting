using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel.Context.Models
{
    public class Contract
    {
        public Guid IdContract { get; set; }
        public Guid ServiceId { get; set; }
        public string NameContract { get; set; }
        public string TypeService { get; set; }
        public Guid FileId { get; set; }
        public long SignDate { get; set; }
        public long ExpDate { get; set; }
        public long ModifyDate { get; set; }
        public long CreateDate { get; set; }
        public string ModifyBy { get; set; }
        public string CreateBy { get; set; }
    }
}
