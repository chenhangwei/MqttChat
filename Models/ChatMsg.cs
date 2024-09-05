using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Models
{
  public partial class ChatMsg
    {      
        public User? Users { get; set; }
        public DateTime ReceiveDateTime { get; set; }
        public string? ReceiveMsg { get; set; }
    }
}
