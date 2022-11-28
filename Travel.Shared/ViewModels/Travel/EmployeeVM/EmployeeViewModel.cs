using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Travel.Shared.Ultilities.Enums;

namespace Travel.Shared.ViewModels.Travel
{
    public class EmployeeViewModel
    {
        private Guid idEmployee;
        private string nameEmployee;
        private string email;
        private long birthday;
        private string image;
        private string phone;
        private string address;
        private bool gender;

        private TitleRole roleId;
        private string roleName;
        private string roleDescription;

        private long createDate;

        private string modifyBy;
        private long modifyDate;

        private bool isDelete;
        private bool isActive;



        public string Email { get => email; set => email = value; }
        public long Birthday { get => birthday; set => birthday = value; }
        public string Image { get => image; set => image = value; }
        public string Phone { get => phone; set => phone = value; }
        public string ModifyBy { get => modifyBy; set => modifyBy = value; }
        public long ModifyDate { get => modifyDate; set => modifyDate = value; }
        public bool IsActive { get => isActive; set => isActive = value; }
        public bool IsDelete { get => isDelete; set => isDelete = value; }
        public string RoleName { get => roleName; set => roleName = value; }
        public long CreateDate { get => createDate; set => createDate = value; }
        public string RoleDescription { get => roleDescription; set => roleDescription = value; }
        public TitleRole RoleId { get => roleId; set => roleId = value; }
        public Guid IdEmployee { get => idEmployee; set => idEmployee = value; }
        public string NameEmployee { get => nameEmployee; set => nameEmployee = value; }
        public string Address { get => address; set => address = value; }
        public bool Gender { get => gender; set => gender = value; }
    }
}
