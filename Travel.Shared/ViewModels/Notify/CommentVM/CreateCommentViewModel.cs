using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel.Shared.ViewModels.Notify.CommentVM
{
    public class CreateCommentViewModel
    {
        private string commentText;
        private Guid idCustomer;
        private string idSchedule;
        private string idTourBooking;
        private double rating;
        public Guid IdCustomer { get => idCustomer; set => idCustomer = value; }
        public string CommentText { get => commentText; set => commentText = value; }
        public string IdSchedule { get => idSchedule; set => idSchedule = value; }
        public string IdTourBooking { get => idTourBooking; set => idTourBooking = value; }
        public double Rating { get => rating; set => rating = value; }

    }
}
