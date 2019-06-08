using System;
using System.Diagnostics;
using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Util;
using Android.Widget;
using AoLibs.Navigation.Android.Navigation;
using AoLibs.Navigation.Core.Interfaces;
using Autofac;
using BleBleBle.Android.Utils.ActivityLifecycle;
using BleBleBle.Domain.Enums;
using BleBleBle.Shared.Statics;
using BleBleBle.Shared.ViewModels;

namespace BleBleBle.Android.Activities
{
    [Activity(Label = "@string/app_name",
        Theme = "@style/AppTheme.Dark",
        ScreenOrientation = ScreenOrientation.Portrait,
        MainLauncher = true,
        Icon = "@mipmap/ic_launcher",
        RoundIcon = "@mipmap/ic_launcher_round",
        LaunchMode = LaunchMode.SingleInstance,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public class MainActivity : AppCompatActivity, IRequestPermissionsResultProvider
    {
        public static MainActivity Instance { get; set; }

        public MainActivity()
        {
            Instance = this;
        }

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            var manager = new NavigationManager<PageIndex>(
                SupportFragmentManager,
                RootView,
                new ViewModelResolver());

            App.NavigationManager = manager;

            using (var scope = ViewModelLocator.ObtainScope())
            {
                scope.Resolve<MainViewModel>().Initialize();
            }
        }

        public override void OnBackPressed()
        {
            if (!App.NavigationManager.OnBackRequested())
            {
                MoveTaskToBack(true);
            }
        }

        #region Views

        private FrameLayout _rootView;

        public FrameLayout RootView => _rootView ?? (_rootView = FindViewById<FrameLayout>(Resource.Id.RootView));

        #endregion

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            Received?.Invoke(this, (requestCode, permissions, grantResults));
        }

        private class ViewModelResolver : IViewModelResolver
        {
            public TViewModel Resolve<TViewModel>()
            {
                Log.Debug(nameof(App), $"Resolving ViewModel: {typeof(TViewModel).Name}");
                try
                {
                    using (var scope = ResourceLocator.ObtainScope())
                    {
                        return scope.Resolve<TViewModel>();
                    }
                }
                catch (Exception e)
                {
                    Debugger.Break();
                    throw;
                }
            }
        }

        public event EventHandler<(int RequestCode, string[] Permissions, Permission[] GrantResults)> Received;
    }

}