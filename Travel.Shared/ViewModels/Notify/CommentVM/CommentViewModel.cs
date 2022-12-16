using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel.Shared.ViewModels.Notify.CommentVM
{
    public class CommentViewModel
    {
        private Guid idComment;
        private long commentTime;
        private string commentText;
        private Guid idCustomer;
        private string nameCustomer;
        private Guid reviewId;
        private double rating;
        public Guid IdComment { get => idComment; set => idComment = value; }
        public long CommentTime { get => commentTime; set => commentTime = value; }
        public string CommentText { get => commentText; set => commentText = value; }
        public Guid IdCustomer { get => idCustomer; set => idCustomer = value; }
        public string NameCustomer { get => nameCustomer; set => nameCustomer = value; }
        public double Rating { get => rating; set => rating = value; }
        public Guid ReviewId { get => reviewId; set => reviewId = value; }
    }
}
