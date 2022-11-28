using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel.Shared.ViewModels.Travel
{
    public class RoleViewModel
    {
        private int idRole;
        private string nameRole;
        private string description;
        private bool isDelete;

        public string Description { get => description; set => description = value; }
        public bool IsDelete { get => isDelete; set => isDelete = value; }
        public int IdRole { get => idRole; set => idRole = value; }
        public string NameRole { get => nameRole; set => nameRole = value; }
    }
}
