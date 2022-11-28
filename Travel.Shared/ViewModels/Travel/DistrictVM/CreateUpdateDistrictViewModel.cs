using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel.Shared.ViewModels.Travel.DistrictVM
{
    public class UpdateDistrictViewModel:CreateDistrictViewModel
    {
        private Guid idDistrict;

        public Guid IdDistrict { get => idDistrict; set => idDistrict = value; }
    }

    public class CreateDistrictViewModel
    {
        private string nameDistrict;
        private Guid idProvince;

        public string NameDistrict { get => nameDistrict; set => nameDistrict = value; }
        public Guid IdProvince { get => idProvince; set => idProvince = value; }
    }
}
