using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travel.Context.Models;
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
        private string nameTour_EN;
        private string thumbnail;
        private string toPlace;
        private string description;
        private double rating;
        private string   alias;
        private List<Image> image;

        
        public string IdTour { get => idTour; set => idTour = value; }
        public string NameTour { get => nameTour; set => nameTour = value; }
        public string Thumbnail { get => thumbnail; set => thumbnail = value; }
        public string ToPlace { get => toPlace; set => toPlace = value; }
        public string Description { get => description; set => description = value; }
        public double Rating { get => rating; set => rating = value; }
        public string NameTour_EN { get => nameTour_EN; set => nameTour_EN = value; }
        public List<Image> Image { get => image; set => image = value; }
        public string Alias { get => alias; set => alias = value; }
    }
}
