using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travel.Context.Models;
using Travel.Shared.ViewModels.Travel.TourVM;

namespace Travel.Shared.ViewModels.Travel
{
    public class TourViewModel
    {
        private string idTour;
        private string nameTour;
        private double rating;
        private string thumbnail;
        private string fromPlace;
        private string toPlace;

        private string approveStatus;
        private int status;

        private long createDate;

        private string modifyBy;
        private long modifyDate;

        private bool isDelete;
        private bool isActive;

        private string idUserModify;
        private string typeAction;
        private int quantityBooked;


        private TourDetailViewModel tourDetail;

        public string NameTour { get => nameTour; set => nameTour = value; }
        public double Rating { get => rating; set => rating = value; }
        public string Thumbnail { get => thumbnail; set => thumbnail = value; }
        public string FromPlace { get => fromPlace; set => fromPlace = value; }
        public string ToPlace { get => toPlace; set => toPlace = value; }
        public string ApproveStatus { get => approveStatus; set => approveStatus = value; }
        public int Status { get => status; set => status = value; }
        public long CreateDate { get => createDate; set => createDate = value; }
        public string ModifyBy { get => modifyBy; set => modifyBy = value; }
        public long ModifyDate { get => modifyDate; set => modifyDate = value; }
        public bool IsDelete { get => isDelete; set => isDelete = value; }
        public bool IsActive { get => isActive; set => isActive = value; }
        public string IdTour { get => idTour; set => idTour = value; }
        public TourDetailViewModel TourDetail { get => tourDetail; set => tourDetail = value; }
        public int QuantityBooked { get => quantityBooked; set => quantityBooked = value; }
        public string TypeAction { get => typeAction; set => typeAction = value; }
        public string IdUserModify { get => idUserModify; set => idUserModify = value; }
    }
}
