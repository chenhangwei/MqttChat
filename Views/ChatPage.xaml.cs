using Chat.Services;
using Chat.ViewModels;

namespace Chat.Views;

public partial class ChatPage : ContentPage
{


	public ChatPage(ChatPageViewModel vm)
	{
		
		InitializeComponent();

		BindingContext=vm;
	}
}