using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel.Shared.ViewModels.Travel
{
    public class DistrictViewModel
    {
        private Guid idDistrict;
        private string nameDistrict;

        private Guid provinceId;
        private string provinceName;

        public Guid IdDistrict { get => idDistrict; set => idDistrict = value; }
        public string NameDistrict { get => nameDistrict; set => nameDistrict = value; }
        public Guid ProvinceId { get => provinceId; set => provinceId = value; }
        public string ProvinceName { get => provinceName; set => provinceName = value; }
    }
}
