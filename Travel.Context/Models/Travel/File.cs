using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Travel.Context.Models
{
    public class File
    {

        public Guid IdFile { get; set; }
        public string NameFile { get; set; }
        public int FileSize { get; set; }
        public string FileExtension { get; set; }
        public string FilePath { get; set; }

    }
}
