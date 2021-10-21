using System;
using Android.App;
using Android.OS;
using AndroidX.AppCompat.App;
using Microsoft.Maui.LifecycleEvents;

namespace Microsoft.Maui
{
	public partial class MauiAppCompatActivity : AppCompatActivity
	{
		// Override this if you want to handle the default Android behavior of restoring fragments on an application restart
		protected virtual bool AllowFragmentRestore => false;

		protected override void OnCreate(Bundle? savedInstanceState)
		{
			if (!AllowFragmentRestore)
			{
				// Remove the automatically persisted fragment structure; we don't need them
				// because we're rebuilding everything from scratch. This saves a bit of memory
				// and prevents loading errors from child fragment managers
				savedInstanceState?.Remove("android:support:fragments");
				savedInstanceState?.Remove("androidx.lifecycle.BundlableSavedStateRegistry.key");
			}

			// If the theme has the maui_splash attribute, change the theme
			if (Theme.TryResolveAttribute(Resource.Attribute.maui_splash))
			{
				SetTheme(Resource.Style.Maui_MainTheme_NoActionBar);
			}

			base.OnCreate(savedInstanceState);

			CreateNativeWindow(savedInstanceState);
		}

		void CreateNativeWindow(Bundle? savedInstanceState = null)
		{
			var mauiApp = MauiApplication.Current.Application;
			if (mauiApp == null)
				throw new InvalidOperationException($"The {nameof(IApplication)} instance was not found.");

			if (mauiApp.Handler?.MauiContext is not IMauiContext applicationContext)
				throw new InvalidOperationException($"The {nameof(IServiceProvider)} instance was not found.");

			var mauiContext = applicationContext.MakeScoped(this);

			applicationContext.Services.InvokeLifecycleEvents<AndroidLifecycle.OnMauiContextCreated>(del => del(mauiContext));

			// TODO: Fix once we have multiple windows
			IWindow window;
			if (mauiApp.Windows.Count > 0)
			{
				// assume if there are windows, then this is a "resume" activity
				window = mauiApp.Windows[0];
			}
			else
			{
				// there are no windows, so this is a fresh launch
				var state = new ActivationState(mauiContext, savedInstanceState);
				window = mauiApp.CreateWindow(state);
			}

			this.SetWindowHandler(window, mauiContext);
		}
	}
}
