using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel.Shared.ViewModels
{
    public class Notification
    {
        public string Messenge { get; set; }
        public string Type { get; set; }
        public DateTime DateTime { get; set; }
        public string Description { get; set; }

    }
}
