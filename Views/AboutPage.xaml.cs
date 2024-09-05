using Chat.ViewModels;

namespace Chat.Views;

public partial class AboutPage
	: ContentPage
{

	public AboutPage(AboutPageViewModel vm)
	{
		InitializeComponent();
		BindingContext=vm;
	}
}