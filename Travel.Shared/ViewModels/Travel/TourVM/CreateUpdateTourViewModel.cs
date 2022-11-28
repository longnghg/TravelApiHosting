using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travel.Shared.ViewModels.Travel.ContractVM;

namespace Travel.Shared.ViewModels.Travel.TourVM
{
    public class UpdateTourViewModel : CreateTourViewModel
    {
        
    }
    public class CreateTourViewModel : UpdateApproveData
    {
        private string idTour;
        private string nameTour;
        private string thumbnail;
        private string toPlace;
        private string description;
        private double rating;
        public string IdTour { get => idTour; set => idTour = value; }
        public string NameTour { get => nameTour; set => nameTour = value; }
        public string Thumbnail { get => thumbnail; set => thumbnail = value; }
        public string ToPlace { get => toPlace; set => toPlace = value; }
        public string Description { get => description; set => description = value; }
        public double Rating { get => rating; set => rating = value; }
    }
}
