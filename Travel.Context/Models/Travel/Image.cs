using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Travel.Context.Models
{
    public class Image
    {
        public Guid IdImage { get; set; }
        public string NameImage { get; set; }
        public long Size { get; set; }

        public string FilePath { get; set; }
        public string IdService { get; set; }
        public string Extension { get; set; }

        public bool IsDelete { get; set; }
        public string TypeAction { get; set; }
    }
}
