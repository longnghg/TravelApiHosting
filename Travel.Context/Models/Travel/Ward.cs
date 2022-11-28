using System;

namespace Travel.Context.Models
{
    public class Ward
    {

        public Guid IdWard { get; set; }
        public string NameWard { get; set; }
        public Guid DistrictId { get; set; }
        public virtual District District { get; set; }
    }
}
