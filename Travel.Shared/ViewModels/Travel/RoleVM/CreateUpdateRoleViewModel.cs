using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel.Shared.ViewModels.Travel
{
    public class UpdateRoleViewModel: CreateRoleViewModel
    {
        private int idRole;
        public int IdRole { get => idRole; set => idRole = value; }

    }
    public class CreateRoleViewModel
    {
        private string nameRole;
        private string description;

        public string NameRole { get => nameRole; set => nameRole = value; }
        public string Description { get => description; set => description = value; }
    }
}
