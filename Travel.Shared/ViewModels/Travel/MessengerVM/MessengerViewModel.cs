using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travel.Context.Models.Notification;

namespace Travel.Shared.ViewModels.Travel.MessengerVM
{
    public class MessengerViewModel
    {
        public Guid IdCustomer { get; set; }
        public string NameCustomer { get; set; }
        public bool IsSeen { get; set; }
        public int TotalNew { get; set; }
        public long Date { get; set; }
        public List<Messenger> Messengers { get; set; }
    }
}
