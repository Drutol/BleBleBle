using Autofac;
using BleBleBle.Shared.ViewModels;

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

            builder.RegisterBuildCallback(container => _container = container);
        }

        public static ILifetimeScope ObtainScope()
        {
            return _container.BeginLifetimeScope();
        }
    }

}
