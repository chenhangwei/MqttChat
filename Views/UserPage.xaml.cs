using Chat.ViewModels;

namespace Chat.Views;

public partial class UserPage : ContentPage
{
	public UserPage(UserPageViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}