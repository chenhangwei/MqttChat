using Chat.Constants;
using Chat.Models;
using Chat.Resources;
using Chat.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Chat.ViewModels
{
    [QueryProperty("Phone", "userPhone")]
    public partial class RegisterPageViewModel : ObservableValidator
    {
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RegisterCommand))]
        private string phoneErrorMsg;
        [ObservableProperty]
        private string nameErrorMsg;
        [ObservableProperty]
        private string passWordErrorMsg;
        [ObservableProperty]
        private string againPassWordErrorMsg;
        [ObservableProperty]
        private string emailErrorMsg;
        [ObservableProperty]
        private string allErrorMsg;
        public TodoItemDatabase database;
        // 用户对象，用于存储注册页面收集到的用户信息
        public User UserP { get; set; }
        // 电话号码
        private string phone;
        [Required(ErrorMessage = "手机号码必填")]//框架内置的验证特性
        [RegularExpression(@"^\d{11}$", ErrorMessage = "电话号码格式不正确")]
        [MaxLength(11, ErrorMessage = "手机号码最大11个字符")]
        [MinLength(11, ErrorMessage = "手机号码最小11个字符")]
        public string Phone
        {
            get => phone;
            set
            {
                ValidateProperty(value, "Phone");
                if (HasErrors)
                {
                    PhoneErrorMsg = string.Join(Environment.NewLine, GetErrors("Phone").Select(e => e.ErrorMessage));
                }
                else
                {
                    PhoneErrorMsg = string.Empty;
                }
                SetProperty(ref phone, value, true);
            }
        }
        private string name;
        [Required(ErrorMessage = "名字必须填写")]
        [MinLength(2, ErrorMessage = "名字不能小于2个字符")]
        [MaxLength(4, ErrorMessage = "名字不能大于4个字符")]
        public string Name
        {
            get => name;
            set
            {
                ValidateProperty(value, "Name");
                if (HasErrors)
                {
                    NameErrorMsg = string.Join(Environment.NewLine, GetErrors("Name").Select(e => e.ErrorMessage));
                }
                else
                {
                    NameErrorMsg = string.Empty;
                }
                SetProperty(ref name, value, true);
            }
        }
        // 性别
        private Gender gender;
        public Gender Gender
        {
            get => gender;
            set => SetProperty(ref gender, value, true);
        }
        // 邮箱地址
        private string? email;
        [EmailAddress(ErrorMessage = "邮箱格式错误")]
        public string? Email
        {
            get => email;
            set
            {
                ValidateProperty(value, "Email");
                if (HasErrors)
                {
                    EmailErrorMsg = string.Join(Environment.NewLine, GetErrors("Email").Select(e => e.ErrorMessage));
                }
                else
                {
                    EmailErrorMsg = string.Empty;
                }
                SetProperty(ref email, value, true);
            }
        }
        // 密码
        private string? password;
        [Required(ErrorMessage = "密码必须填写")]
        [MinLength(6, ErrorMessageResourceName = "PasswordMinLengthMessage", ErrorMessageResourceType = typeof(Resource1))]
        [MaxLength(20, ErrorMessageResourceName = "PasswordMaxLengthMessage", ErrorMessageResourceType = typeof(Resource1))]
        public string? PassWord
        {
            get => password;
            set
            {
                ValidateProperty(value, "PassWord");
                if (HasErrors)
                {
                    PassWordErrorMsg = string.Join(Environment.NewLine, GetErrors("PassWord").Select(e => e.ErrorMessage));
                }
                else
                {
                    PassWordErrorMsg = string.Empty;
                }
                SetProperty(ref password, value, true);
            }
        }
        private string? againPassWord;
        public string? AgainPassWord
        {
            get => againPassWord;
            set
            {
                if (PassWord != value)
                {
                    AgainPassWordErrorMsg = "重复密码不准确";
                }
                else
                {
                    AgainPassWordErrorMsg = string.Empty;
                }
                SetProperty(ref againPassWord, value, true);
            }
        }
        // 头像 URL
        private string picture;
        public string Picture
        {
            get => picture;
            set => SetProperty(ref picture, value, true);
        }
        [RelayCommand(CanExecute = nameof(CanUse))]
        async void Register()
        {
            try
            {
                UserP.Name = Name;
                UserP.Phone = Phone;
                UserP.Email = Email;
                UserP.Gender = Gender;
                UserP.Picture = Picture;
                UserP.Password = PassWord;
                var UserList = await database.GetItemsAsync();
                if (UserList != null)
                {
                    bool found = false;
                    foreach (var user in UserList)
                    {
                        if (user.Phone == UserP.Phone)
                        {
                            found = true;
                            break;
                        }
                    }
                    if (found)
                    {
                        await AppShell.Current.DisplayAlert("警告", "手机号码已经存在", "确认");
                    }
                    else
                    {
                        if (PassWordErrorMsg == ""&&AgainPassWordErrorMsg=="")
                        {
                            if (await AppShell.Current.DisplayAlert("警告","确认需要注册用户信息吗？","确认","取消"))
                            { 
                                var a = database.SaveItemAsync(UserP); 
                                await AppShell.Current.DisplayAlert("成功", "用户注册成功", "确认");
                                Clear();
                            }
                        }
                    }
                }
                else
                {
                    await AppShell.Current.DisplayAlert("警告", "服务未开启", "确认");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
        private bool CanUse()
        {
            if (PhoneErrorMsg == "")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        [RelayCommand]
        async void Clear()
        {
            Phone = "";
            Name = "";
            Email = "";
            PassWord = "";
            Picture = "";
            Gender = 0;
            UserP = new User();
        }
        public RegisterPageViewModel(TodoItemDatabase todoItemDatabase)
        {
            UserP = new User();
            database = todoItemDatabase;
        }
    }
}
   