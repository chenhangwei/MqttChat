using Chat.Constants;
using Chat.Models;
using Chat.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.ViewModels
{
    [QueryProperty("Phone", "userPhone")]
    public partial class UserPageViewModel:ObservableRecipient
    {

        [ObservableProperty]
        string? phone;
        [ObservableProperty]
        string? name;
        [ObservableProperty]
        Gender gender;
        [ObservableProperty]
        string? pictrue;
        [ObservableProperty]
        string? email;

      public  UserPageViewModel() 
        {   
                    
        }

     
    }
}
