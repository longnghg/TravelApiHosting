using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Travel.Context.Models
{
    public class Role
    {

        public int IdRole { get; set; }
        public string NameRole { get; set; }
        public string Description { get; set; }
        public bool IsDelete { get; set; }
        public virtual ICollection<Employee> Employees { get; set; }

    } 
}
