using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using XFSlideShow.Models;
using XFSlideShow.ViewModels;

namespace XFSlideShow
{
    public partial class XamGridViewFormsPage : ContentPage
    {
        public XamGridViewFormsPage()
        {
            InitializeComponent();
            BindingContext = new XamGridViewViewModel();

            navAdd.Clicked += (sender, e) => {
                Device.OpenUri(new System.Uri("https://github.com/Xamarin-Harshad/XFSlideShow"));
            };


			GrdView.ItemSelected += async (object sender, object e) =>
			{
				var currentModel = e as XamGridModel;
				//await App.Current.MainPage.DisplayAlert("Clicked", "Current image position is " + currentModel.Position, "OK");
				var page = new XFSlideShowPage(currentModel.Position);
                //PopupNavigation.pop(); 
                await PopupNavigation.PushAsync(page);
			};
        }
    }
}
