using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using Chat.Models;
using Chat.Views;
using Chat.Services;
using System.Reflection.Metadata;
using System.Diagnostics;
using Microsoft.Maui.Storage;
using CommunityToolkit.Mvvm.Messaging;
using Chat.Constants;
using Chat.Resources;
namespace Chat.ViewModels
{
    public partial class MainPageViewModel : ObservableValidator
    {
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
        private string phoneErrorMsg;
        [ObservableProperty]
        private string passWordErrorMsg;
        public List<User>? UserList { get; set; }
        public User UserP { set; get; }
        private TodoItemDatabase database;
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
        private bool? isRemember;
        public bool? IsRemember
        {
            get => isRemember;
            set => SetProperty(ref isRemember, value, true);
        }
        /// <summary>
        /// 初始化 MainPageViewModel 类的新实例。
        /// </summary>
        /// <param name="todoItemDatabase">待办事项数据库服务。</param>
        /// <param name="mqttService">MQTT 服务。</param>
        public MainPageViewModel(TodoItemDatabase todoItemDatabase)
        {
            try
            {
                // 将待办事项数据库服务实例赋值给成员变量 database。
                database = todoItemDatabase;
                // 创建一个新的 User 实例并将其赋值给成员变量 UserP。
                UserP = new User();
                // 调用异步方法 LoadUserCredentialsAsync() 来加载用户的凭证。
                LoadUserCredentialsAsync();
            }
            catch (Exception ex)
            {
                // 如果构造过程中发生异常，则在调试输出中记录异常信息。
                Debug.WriteLine(ex.Message);
            }
        }
        /// <summary>
        /// 异步从安全存储中加载用户凭证。
        /// </summary>
        /// <returns>一个代表异步操作的任务。</returns>
        private async Task LoadUserCredentialsAsync()
        {
            try
            {
                // 从安全存储中获取与键 "userPhone" 关联的电话号码。
                Phone = await SecureStorage.GetAsync("userPhone");
                // 从安全存储中获取与键 "userPassWord" 关联的密码。
                PassWord = await SecureStorage.GetAsync("userPassWord");
                // 尝试将存储中的 "userIsRemember" 值解析为布尔值。
                if (bool.TryParse(await SecureStorage.GetAsync("userIsRemember"), out bool isRemember))
                {
                    // 如果解析成功，则设置 IsRemember 为相应的布尔值。
                    IsRemember = isRemember;
                }
                else
                {
                    // 如果解析失败，则默认设置 IsRemember 为 false。
                    IsRemember = false;
                }
            }
            catch (Exception e)
            {
                // 如果出现异常，则在调试输出中写入异常消息。
                Debug.WriteLine(e.Message);
            }
        }
        [RelayCommand(CanExecute = nameof(CanExecuteLogin))]
        /// <summary>
        /// 异步执行登录操作。
        /// </summary>
        /// <returns>一个代表异步操作的任务。</returns>
        private async Task Login()
        {
            try
            {
                //管理用户凭证的存储
                await ManageUserCredentials();
                // 从数据库中获取用户列表。
                UserList = await database.GetItemsAsync();
                // 设置当前用户的电话号码和密码。
                UserP.Phone = Phone;
                UserP.Password = PassWord;
                // 检查用户列表是否存在。
                if (UserList != null)
                {
                    // 遍历用户列表以查找匹配的用户。
                    bool found = false;
                    foreach (var user in UserList)
                    {
                        if (user.Phone == UserP.Phone && user.Password == UserP.Password)
                        {
                            // 如果找到了匹配的用户，设置标志并退出循环。
                            found = true;
                            break;
                        }
                    }
                    // 根据是否找到匹配的用户进行不同的处理。
                    if (found)
                    {
                        // 如果找到了匹配的用户，导航到聊天页面。
                        await AppShell.Current.GoToAsync(nameof(ChatPage));
                    }
                    else
                    {
                        // 如果没有找到匹配的用户，显示警告提示。
                        await AppShell.Current.DisplayAlert("警告", "用户没有记录或密码不正确", "确认");
                    }
                }
                else
                {
                    // 如果用户列表为空，显示警告提示。
                    await AppShell.Current.DisplayAlert("警告", "用户没有记录", "确认");
                }
            }
            catch (Exception ex)
            {
                // 如果发生异常，记录异常信息。
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                // 最终块，通常用于释放资源。在这个例子中没有需要释放的资源。
            }
        }
        /// <summary>
        /// 管理用户凭证的保存和清除。
        /// </summary>
        private async Task ManageUserCredentials()
        {
            // 如果用户选择了记住凭证选项...
            if (IsRemember == true)
            {
                // ...保存电话号码到安全存储中（如果电话号码不为空）。
                if (Phone is not null)
                {
                    await SecureStorage.SetAsync("userPhone", Phone);
                }
                // ...保存密码到安全存储中（如果密码不为空）。
                if (PassWord is not null)
                {
                    await SecureStorage.SetAsync("userPassWord", PassWord);
                }
                // 保存是否记住凭证的状态为真。
                await SecureStorage.SetAsync("userIsRemember", "True");
            }
            else
            {
                // 如果用户没有选择记住凭证选项...
                // ...从安全存储中移除密码。
                SecureStorage.Remove("userPassWord");
                // 保存是否记住凭证的状态为假。
                await SecureStorage.SetAsync("userIsRemember", "False");
            }
        }    
        /// <summary>
        /// 登入按钮是否有效
        /// </summary>
        /// <returns></returns>
        private bool CanExecuteLogin()
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
        /// <summary>
        /// 注册用户
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task GoToRegisterPage()
        {
            try
            {
                await AppShell.Current.GoToAsync($"{nameof(RegisterPage)}?userPhone={Phone}");
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// 注销用户
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task LogOff()
        {
            try
            {
                UserList = await database.GetItemsAsync();
                UserP.Phone = Phone;
                UserP.Password = PassWord;
                if (UserList != null)
                {
                    bool found = false;
                    foreach (var user in UserList)
                    {
                        if (user.Phone == UserP.Phone && user.Password == UserP.Password)
                        {
                            found = true;
                            break;
                        }
                    }
                    if (found)
                    {
                        if (await AppShell.Current.DisplayAlert("警告", "是否要删除用户账户记录", "确认", "取消"))
                        {
                            await database.DeleteItemAsync(UserP);
                            Phone = "";
                            PassWord = "";
                        }
                    }
                    else
                    {
                        await AppShell.Current.DisplayAlert("警告", "用户没有记录或密码不正确", "确认");
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// 导航到关于页面
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
  private async Task GoToAboutPage()
        {
            await AppShell.Current.GoToAsync(nameof(AboutPage));
        } 
        [RelayCommand]
        private async Task GoToSetPage()
        {
            await AppShell.Current.GoToAsync(nameof(SetPage));
        }
    }
}
