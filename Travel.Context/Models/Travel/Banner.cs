using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel.Context.Models
{
    public class Banner
    {
        public Guid IdBanner { get; set; }
        public string NameBanner { get; set; }
        public int Total { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
    }
}
