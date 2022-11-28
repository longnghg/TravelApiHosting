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
        private string idTour;
        public Guid IdCustomer { get => idCustomer; set => idCustomer = value; }
        public string CommentText { get => commentText; set => commentText = value; }
        public string IdTour { get => idTour; set => idTour = value; }
    }
}
