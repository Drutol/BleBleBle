using System;
using System.Net;
using Android.App;
using Android.Runtime;
using AoLibs.Adapters.Android;
using AoLibs.Adapters.Android.Interfaces;
using AoLibs.Adapters.Core.Interfaces;
using AoLibs.Navigation.Core.Interfaces;
using Autofac;
using BleBleBle.Android.Activities;
using BleBleBle.Android.Adapters;
using BleBleBle.Android.Utils.ActivityLifecycle;
using BleBleBle.Domain.Enums;
using BleBleBle.Interfaces;
using BleBleBle.Shared.Statics;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;

namespace BleBleBle.Android
{
    [Application]
    public class App : Application
    {
        public App(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {
            Current = this;
        }

        public static INavigationManager<PageIndex> NavigationManager { get; set; }
        public static App Current { get; private set; }

        public override void OnCreate()
        {
            AppInitializationRoutines.Initialize(AdaptersRegistration);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            base.OnCreate();
        }

        private void AdaptersRegistration(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<ClipboardProvider>().As<IClipboardProvider>().SingleInstance();
            containerBuilder.RegisterType<DispatcherAdapter>().As<IDispatcherAdapter>().SingleInstance();
            containerBuilder.RegisterType<FileStorageProvider>().As<IFileStorageProvider>().SingleInstance();
            containerBuilder.RegisterType<MessageBoxProvider>().As<IMessageBoxProvider>().SingleInstance();
            containerBuilder.RegisterType<SettingsProvider>().As<ISettingsProvider>().SingleInstance();
            containerBuilder.RegisterType<UriLauncherAdapter>().As<IUriLauncherAdapter>().SingleInstance();
            containerBuilder.RegisterType<VersionProvider>().As<IVersionProvider>().SingleInstance();
            containerBuilder.RegisterType<PickerAdapter>().As<IPickerAdapter>().SingleInstance();
            containerBuilder.RegisterType<ContextProvider>().As<IContextProvider>().SingleInstance();
            containerBuilder.RegisterType<PhotoPickerAdapter>().As<IPhotoPickerAdapter>().SingleInstance();
            containerBuilder.RegisterType<PhoneCallAdapter>().As<IPhoneCallAdapter>().SingleInstance();

            containerBuilder.RegisterType<PermissionsManager>().As<IPermissionsManager>().SingleInstance();
            containerBuilder.RegisterType<BluetoothDeviceDataExtractor>().As<IBluetoothDeviceDataExtractor>().SingleInstance();

            containerBuilder.Register(context => CrossBluetoothLE.Current.Adapter).As<IAdapter>();

            containerBuilder
                .Register(context => MainActivity.Instance)
                .As<IRequestPermissionsResultProvider>();

            containerBuilder.Register(ctx => NavigationManager).As<INavigationManager<PageIndex>>();
        }

        private class ContextProvider : IContextProvider
        {
            public Activity CurrentContext => MainActivity.Instance;
        }
    }

}