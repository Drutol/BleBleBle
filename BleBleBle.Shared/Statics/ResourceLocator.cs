using Autofac;

namespace BleBleBle.Shared.Statics
{
    public static class ResourceLocator
    {
        private static IContainer _container;

        internal static void RegisterResources(this ContainerBuilder builder)
        {
            builder.RegisterBuildCallback(container => _container = container);


        }

        public static ILifetimeScope ObtainScope()
        {
            return _container.BeginLifetimeScope();
        }
    }

}
