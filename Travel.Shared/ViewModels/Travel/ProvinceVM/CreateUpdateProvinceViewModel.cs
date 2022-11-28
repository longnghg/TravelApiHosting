using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel.Shared.ViewModels.Travel
{
    public class UpdateProvinceViewModel : CreateProvinceViewModel
    {
        private Guid idProvince;
        public Guid IdProvince { get => idProvince; set => idProvince = value; }
    }

    public class CreateProvinceViewModel
    {
        private string nameProvince;
        public string NameProvince { get => nameProvince; set => nameProvince = value; }
    }
}
