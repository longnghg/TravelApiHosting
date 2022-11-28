 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel.Shared.ViewModels.Travel.CustomerVM
{
    public class UpdateCustomerViewModel : CreateCustomerViewModel
    {

    }
    public class CreateCustomerViewModel
    {
        private Guid idCustomer;
        private string nameCustomer;
        private string phone;
        private string email;
        private string address;
        private bool gender;
        private string password;     
        private long birthday;


        public Guid IdCustomer { get => idCustomer; set => idCustomer = value; }
        public string NameCustomer { get => nameCustomer; set => nameCustomer = value; }
        public string Phone { get => phone; set => phone = value; }
        public string Email { get => email; set => email = value; }
        public string Address { get => address; set => address = value; }
        public string Password { get => password; set => password = value; }
        public long Birthday { get => birthday; set => birthday = value; }
        public bool Gender { get => gender; set => gender = value; }
    }
}
