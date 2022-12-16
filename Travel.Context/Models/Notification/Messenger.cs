using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel.Context.Models.Notification
{
    public class Messenger
    {
        public Guid IdMessenger { get; set; }
        public string SenderName { get; set; }
        public long SendDate { get; set; }
        public string Content { get; set; }
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
        public bool IsSeen { get; set; }
    }
}
