using System;
using Autofac;

namespace BleBleBle.Shared.Statics
{
    public static class AppInitializationRoutines
    {
        public static void Initialize(Action<ContainerBuilder> adaptersRegistration)
        {
            var builder = new ContainerBuilder();

            builder.RegisterViewModels();
            builder.RegisterResources();
            adaptersRegistration(builder);

            builder.Build();
        }
    }

}
