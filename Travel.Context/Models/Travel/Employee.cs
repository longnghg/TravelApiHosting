using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Travel.Context.Models
{
    public class Employee
    {
        public Guid IdEmployee { get; set; }
        public string NameEmployee { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public long Birthday { get; set; }
        public bool Gender { get; set; }
        public string Address { get; set; }
        public string Image { get; set; }
        public string Phone { get; set; }
        public int RoleId { get; set; }
        public long CreateDate { get; set; }
        public string AccessToken { get; set; }
        public string ModifyBy { get; set; }
        public long ModifyDate { get; set; }
        public bool IsDelete { get; set; }
        public bool IsActive { get; set; }
        public bool IsOnline { get; set; }
        public long TimeBlock { get; set; }
        public bool IsBlock { get; set; }
        public virtual Role Role { get; set; }
        public virtual ICollection<Schedule> Schedules { get; set; }

    }

}
