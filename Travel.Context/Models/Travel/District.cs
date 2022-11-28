using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel.Context.Models
{
    public class District
    {
        public Guid IdDistrict { get; set; }
        public string NameDistrict { get; set; }
        public Guid ProvinceId { get; set; }
        public virtual Province Province { get; set; }
        public virtual ICollection<Ward> Ward { get; set; }

    }
}
