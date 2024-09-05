using Chat.Models;
using Chat.Services;
using Chat.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MQTTnet.Client;
using MQTTnet.Protocol;
using MQTTnet;
using MQTTnet.Server;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static Microsoft.Maui.ApplicationModel.Permissions;
namespace Chat.ViewModels
{
    public partial class ChatPageViewModel : ObservableValidator
    {
        CancellationToken shutdownToken;
        CancellationTokenSource shutdownTokenSource;
        TodoItemDatabase _database;
        MqttService _mqttService;
        [ObservableProperty]
        ChatMsg chatMsg;
        [ObservableProperty]
        string sendMsg;
        [ObservableProperty]
        public ObservableCollection<ChatMsg> chatMsgs;
        [ObservableProperty]
        private string linkState;
        [ObservableProperty]
        User? userP;
        string Phone{ get; set; }
     
        private string topicFirst;
        public string TopicFirst
        {
            get => topicFirst;
            set
            {
                if (value is not null)
                {
                    SecureStorage.SetAsync("TopicFirst", value);
                }
                else
                {
                }
                SetProperty(ref topicFirst, value, true);
            }
        }
        private string topicSecond;
        public string TopicSecond
        {
            get => topicSecond;
            set
            {
                if (value is not null)
                {
                    SecureStorage.SetAsync("TopicSecond", value);
                }
                else
                {
                }
                SetProperty(ref topicSecond, value, true);
            }
        }
        private string topic;
        public ChatPageViewModel(TodoItemDatabase todoItemDatabase, MqttService mqttService)
        {
            try
            {
                shutdownTokenSource = new CancellationTokenSource();
                _database = todoItemDatabase;
                _mqttService = mqttService;
                this.ChatMsg = new ChatMsg();
                ChatMsgs = _mqttService.ChatMsgs;
                LinkState = "disconnect.png";
                // 调用异步方法 LoadUserCredentialsAsync() 来加载用户的凭证。
                LoadUserCredentialsAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
        private async void LoadUserCredentialsAsync()
        {
            try
            {
                UserP = new User();
                // 从安全存储中获取与键 "userPhone" 关联的电话号码。
                TopicFirst = await SecureStorage.GetAsync("TopicFirst");
                // 从安全存储中获取与键 "userPassWord" 关联的密码。
                TopicSecond = await SecureStorage.GetAsync("TopicSecond");
                // 从安全存储中获取与键 "userPhone" 关联的电话号码。
                UserP.Phone = await SecureStorage.GetAsync("userPhone");
                // 从安全存储中获取与键 "userPassWord" 关联的密码。
                UserP.Password = await SecureStorage.GetAsync("userPassWord");
                UserP = await _database.GetItemAsync(UserP.Phone);
            }
            catch (Exception e)
            {
                // 如果出现异常，则在调试输出中写入异常消息。
                Debug.WriteLine(e.Message);
            }
        }
        [RelayCommand]
        public async Task GoToUserPage()
        {

            Phone = UserP.Phone;
            await AppShell.Current.GoToAsync($"{nameof(UserPage)}?userPhone={Phone}");
        }
        [RelayCommand]
        public async void ConnetMqttAsync()
        {
            try
            {
                topic = $"{TopicFirst}/{topicSecond}";
                if (LinkState == "disconnect.png")
                {
                    await _mqttService.ConnetMqttAsync(UserP, topic, shutdownTokenSource.Token);
                    LinkState = "connected.png";
                }
                else
                {
                    await _mqttService.DisconnectAsync();
                    LinkState = "disconnect.png";
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        [RelayCommand]
        public async void PublishAsync()
        {
            if (LinkState == "connected.png")
            {
                await _mqttService.PublishAsync(topic, ChatMsg, SendMsg);
                SendMsg = "";
            }
        }
    }
}
