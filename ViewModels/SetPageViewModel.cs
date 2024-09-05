using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Chat.ViewModels
{
    public partial class SetPageViewModel : ObservableRecipient
    {
        [ObservableProperty]
        private string host;
        [ObservableProperty]
        private string port;
       public SetPageViewModel()
        {
            try
            {
                LoadSetAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
        private async void LoadSetAsync()
        {
            Host =await SecureStorage.GetAsync("Host");
            Port =await SecureStorage.GetAsync("Port");
        }
        [RelayCommand]
        private void Save() 
        {
            try
            {
                if (Host is not null)
                {
                    SecureStorage.SetAsync("Host", Host);
                }
                else
                {
                    Host = "不能为空";
                }
                if (Port is not null)
                {
                    SecureStorage.SetAsync("Port", Port);
                }
                else
                {
                    Port = "不能为空";
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
