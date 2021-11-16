using Autofac;
using CrossCutting.Mediator.Builder;
using CrossCutting.Mediator.DefaultBehaviorsExtensions;
using CrossCutting.Validation.Extensions;
using Microsoft.Extensions.Configuration;

namespace TriviaDotNetApi.Application.Services
{
    public static class LocalServiceRegisters
    {
        public static void AddMediatorModules(this ContainerBuilder builder, IConfiguration configuration)
        {

            builder.RegisterModule(new MediatorModuleBuilder()
                                       .AddAssembly("TriviaDotNetApi.Application")
                                       .AddLoggingBehavior()
                                       .AddValidationBehavior()
                                       .Build());


        }
    }
}
    