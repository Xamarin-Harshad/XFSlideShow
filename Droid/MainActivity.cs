
using Android.App;
using Android.Content.PM;
using Android.OS;
using FFImageLoading;

namespace XFSlideShow.Droid
{
    [Activity(Label = "XFSlideShow", Icon = "@drawable/icon", Theme = "@style/MyTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            //TabLayoutResource = Resource.Layout.Tabbar;
            //ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

			FFImageLoading.Forms.Droid.CachedImageRenderer.Init();
			var config = new FFImageLoading.Config.Configuration()
			{
				VerboseLogging = false,
				VerbosePerformanceLogging = false,
				VerboseMemoryCacheLogging = false,
				VerboseLoadingCancelledLogging = false
			};
			ImageService.Instance.Initialize(config);


			LoadApplication(new App());
        }
    }
}
