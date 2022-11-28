using System.Collections.Generic;
using Travel.Context.Models;

namespace Travel.Shared.ViewModels.Travel
{
    public class CreateUpdateTourViewModel
    {
        private string id;
        private string tourName;
        private double rating;
        private string thumbsnail;
        private string fromPlace;
        private string toPlace;
        private int status;    

        public string Id { get => id; set => id = value; }
        public string TourName { get => tourName; set => tourName = value; }
        public double Rating { get => rating; set => rating = value; }
        public string Thumbsnail { get => thumbsnail; set => thumbsnail = value; }
        public string FromPlace { get => fromPlace; set => fromPlace = value; }
        public string ToPlace { get => toPlace; set => toPlace = value; }
        public int Status { get => status; set => status = value; }    
    }
}
