using System;
using System.Collections.Generic;
using System.Linq;
using FFImageLoading;
using Foundation;
using UIKit;

namespace XFSlideShow.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();

            LoadApplication(new App());

			FFImageLoading.Forms.Touch.CachedImageRenderer.Init();

			var config = new FFImageLoading.Config.Configuration()
			{
				VerboseLogging = false,
				VerbosePerformanceLogging = false,
				VerboseMemoryCacheLogging = false,
				VerboseLoadingCancelledLogging = false
			};
			ImageService.Instance.Initialize(config);

			Rg.Plugins.Popup.IOS.Popup.Init();


			return base.FinishedLaunching(app, options);
        }
    }
}
