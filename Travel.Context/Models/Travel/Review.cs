using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel.Context.Models
{
    public class Review
    {

        public Guid Id { get; set; }
        public double Rating { get; set; }
        public string IdTour { get; set; }

    }
}
