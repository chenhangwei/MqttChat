using Chat.Models;
using Chat.Services;
using Chat.ViewModels;
using Chat.Views;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;


namespace Chat
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
           
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });



            builder.Services.AddSingleton<TodoItemDatabase>();         
            builder.Services.AddSingleton<MainPageViewModel>();
            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddSingleton<ChatPageViewModel>();
            builder.Services.AddSingleton<ChatPage>();
            builder.Services.AddTransient<RegisterPageViewModel>();
            builder.Services.AddTransient<RegisterPage>();
            builder.Services.AddTransient<AboutPageViewModel>();
            builder.Services.AddTransient<AboutPage>();
            builder.Services.AddSingleton<SetPage>();
            builder.Services.AddSingleton<SetPageViewModel>();
            builder.Services.AddSingleton<UserPage>();
            builder.Services.AddSingleton<UserPageViewModel>();
            builder.Services.AddSingleton<ChatMsg>();
            builder.Services.AddSingleton<MqttService>();


        


           




#if DEBUG

#endif

            return builder.Build();
        }
    }
}
