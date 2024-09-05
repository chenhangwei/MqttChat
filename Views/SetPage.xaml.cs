using Chat.ViewModels;

namespace Chat.Views;

public partial class SetPage : ContentPage
{
	public SetPage(SetPageViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}