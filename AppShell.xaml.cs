using Chat.Services;
using Chat;
using Chat.Views;

namespace Chat
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
            Routing.RegisterRoute(nameof(ChatPage), typeof(ChatPage));
            Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
            Routing.RegisterRoute(nameof(AboutPage), typeof(AboutPage));
            Routing.RegisterRoute(nameof(SetPage), typeof(SetPage));
            Routing.RegisterRoute(nameof(UserPage), typeof(UserPage));

        }
    }
}
