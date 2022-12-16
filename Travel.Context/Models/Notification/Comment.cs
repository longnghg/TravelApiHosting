using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel.Context.Models.Notification
{
    public class Comment
    {
        public Guid IdComment { get; set; }
        public long CommentTime { get; set; }
        public string CommentText { get; set; }
        public Guid IdCustomer { get; set; }
        public string NameCustomer { get; set; }
        public string IdTour { get; set; }

        public Guid ReviewId { get; set; }
    }
}
