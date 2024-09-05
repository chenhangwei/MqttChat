using Chat.Constants;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Models
{
   public  class User
    {
        [PrimaryKey]
        public string? Phone{ get; set; }
        public string? Name { get; set; }
        public Gender Gender { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Picture { get; set; }


    }
}
