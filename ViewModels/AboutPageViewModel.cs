using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Input;

namespace Chat.ViewModels
{
    public partial class AboutPageViewModel : ObservableValidator
    {
        [ObservableProperty]
        string version;


     public   AboutPageViewModel()
        {
            Version ver = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            version = ver.ToString();
           
        }
        [RelayCommand]
      public async void LinkWeb()
        {
           await Browser.Default.OpenAsync("https://github.com/chenhangwei");

        }

    }
}
