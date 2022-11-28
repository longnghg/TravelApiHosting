using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel.Shared.ViewModels.Travel.ReviewVM
{
    public class ReviewViewModel
    {
        private Guid id { get; set; }
        private double rating { get; set; }
        private long dateTime { get; set; }
        private string comment { get; set; }

        public Guid Id { get => id; set => id = value; }
        public double Rating { get => rating; set => rating = value; }
        public long DateTime { get => dateTime; set => dateTime = value; }
        public string Comment { get => comment; set => comment = value; }
    }
}
