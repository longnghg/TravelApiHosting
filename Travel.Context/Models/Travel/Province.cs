using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel.Context.Models
{
    public class Province
    {
        public Guid IdProvince { get; set; }
        public string NameProvince { get; set; }
        public virtual ICollection<District> Districts { get; set; }
    }
}
