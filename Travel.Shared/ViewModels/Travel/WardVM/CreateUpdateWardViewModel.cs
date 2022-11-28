using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel.Shared.ViewModels.Travel.WardVM
{
    public class UpdateWardViewModel:CreateWardViewModel
    {
        private Guid idWard;

        public Guid IdWard { get => idWard; set => idWard = value; }
    }

    public class CreateWardViewModel
    {
        private string nameWard;
        private Guid idDistrict;

        public string NameWard { get => nameWard; set => nameWard = value; }
        public Guid IdDistrict { get => idDistrict; set => idDistrict = value; }
    }
}
