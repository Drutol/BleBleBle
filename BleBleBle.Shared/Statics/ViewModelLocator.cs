using Autofac;
using BleBleBle.Shared.ViewModels;
using BleBleBle.Shared.ViewModels.Items;

namespace BleBleBle.Shared.Statics
{
    public static class ViewModelLocator
    {
        private static IContainer _container;

        internal static void RegisterViewModels(this ContainerBuilder builder)
        {
            builder.RegisterType<DashboardViewModel>().SingleInstance();
            builder.RegisterType<MainViewModel>().SingleInstance();
            builder.RegisterType<PermissionsViewModel>().SingleInstance();
            builder.RegisterType<ScannerPageViewModel>().SingleInstance();
            builder.RegisterType<DeviceDetailsViewModel>().SingleInstance();

            builder.RegisterType<ScannedDeviceViewModel>();
            builder.RegisterType<DeviceServiceViewModel>();
            builder.RegisterType<DeviceCharacteristicViewModel>();

            builder.RegisterBuildCallback(container => _container = container);
        }

        public static ILifetimeScope ObtainScope()
        {
            return _container.BeginLifetimeScope();
        }
    }

}
