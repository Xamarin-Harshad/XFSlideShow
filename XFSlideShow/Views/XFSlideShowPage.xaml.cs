using System;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using XFSlideShow.ViewModels;

namespace XFSlideShow
{
    public partial class XFSlideShowPage : PopupPage
    {
        XamGridViewViewModel _viewModel = new XamGridViewViewModel();
        int _selectedPos;
        public XFSlideShowPage(int selectedPos)
        {
            InitializeComponent();
            BindingContext = _viewModel;
            _selectedPos = selectedPos;
            carouselGallery.ItemsSource = _viewModel.XamMockDataList;
        }

        void Handle_Clicked_prev(object sender, System.EventArgs e)
        {
            try
            {
                var currentPosition = carouselGallery.Position;
                if (currentPosition - 1 >= 0)
                {
                    carouselGallery.Position = currentPosition - 1;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        void Handle_Clicked_next(object sender, System.EventArgs e)
        {
            try
            {
                var currentPosition = carouselGallery.Position;
                if (currentPosition + 1 <= _viewModel.XamMockDataList.Count)
                {
                    carouselGallery.Position = currentPosition + 1;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        async void Handle_Clicked_close(object sender, System.EventArgs e)
        {
            await PopupNavigation.PopAsync();
        }

        protected async override void OnAppearing()
        {
            carouselGallery.Position = _selectedPos;
            await Task.Delay(300);
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }
    }
}
