using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel.Shared.ViewModels.Notify.MessengerVM
{
    public class CreateMessengerViewModel
    {
        private Guid idMessenger;
        private string senderName;
        private long sendDate;
        private string content;
        private Guid senderId;
        private Guid receiverId;

        public Guid IdMessenger { get => idMessenger; set => idMessenger = value; }
        public string SenderName { get => senderName; set => senderName = value; }
        public long SendDate { get => sendDate; set => sendDate = value; }
        public string Content { get => content; set => content = value; }
        public Guid SenderId { get => senderId; set => senderId = value; }
        public Guid ReceiverId { get => receiverId; set => receiverId = value; }
    }
}
