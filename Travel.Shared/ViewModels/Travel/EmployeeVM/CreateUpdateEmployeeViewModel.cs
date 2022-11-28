using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Travel.Shared.Ultilities.Enums;

namespace Travel.Shared.ViewModels.Travel
{
    public class UpdateEmployeeViewModel: CreateEmployeeViewModel
    {
        
       
    }

    public class CreateEmployeeViewModel
    {
        private Guid idEmployee;
        private string nameEmployee;
        private string email;
        private long birthday;
        private string image;
        private string modifyBy;
        private string phone;
        private string address;
        private bool gender;
        private bool isActive;
        private TitleRole roleId;

        public Guid IdEmployee { get => idEmployee; set => idEmployee = value; }
        public string Email { get => email; set => email = value; }
        public long Birthday { get => birthday; set => birthday = value; }
        public string Image { get => image; set => image = value; }
        public string Phone { get => phone; set => phone = value; }
        public TitleRole RoleId { get => roleId; set => roleId = value; }
        public string NameEmployee { get => nameEmployee; set => nameEmployee = value; }
        public string Address { get => address; set => address = value; }
        public bool Gender { get => gender; set => gender = value; }
        public string ModifyBy { get => modifyBy; set => modifyBy = value; }
        public bool IsActive { get => isActive; set => isActive = value; }
    }
}
