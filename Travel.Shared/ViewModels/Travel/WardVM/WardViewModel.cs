using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel.Shared.ViewModels.Travel
{
    public class WardViewModel
    {
        private Guid idWard;
        private string nameWard;

        private Guid districtId;
        private string districtName;

        public Guid IdWard { get => idWard; set => idWard = value; }
        public string NameWard { get => nameWard; set => nameWard = value; }
        public Guid DistrictId { get => districtId; set => districtId = value; }
        public string DistrictName { get => districtName; set => districtName = value; }
    }
}
