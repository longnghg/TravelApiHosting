using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel.Context.Models.Notification
{
    public class Notifications
    {
        public Guid IdNotification { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int Type { get; set; }
        public long Time { get; set; }
        public bool IsSeen { get; set; }
        public Guid RequestId { get; set; }
        public Guid ReponseId { get; set; }
        public string RoleId { get; set; }
    }
}
