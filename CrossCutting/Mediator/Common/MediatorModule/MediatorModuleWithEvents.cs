using Autofac;
using FluentValidation;
using MediatR;
using System;
using System.Reflection;

namespace CrossCutting.Mediator.Common.MediatorModule
{
    public class MediatorModuleWithEvents : Autofac.Module
    {
        private string[] _assemblies;

        public MediatorModuleWithEvents(params string[] assemblies)
        {
            _assemblies = assemblies;
        }
        protected override void Load(ContainerBuilder builder)
        {
            foreach (string assembly in _assemblies)
            {
                string applicationAssemblyName = assembly;
                var applicationAssembly = AppDomain.CurrentDomain.Load(applicationAssemblyName);
                LoadForAssembly(builder, applicationAssembly);
            }

            builder.Register<ServiceFactory>(context =>
            {
                var componentContext = context.Resolve<IComponentContext>();
                return t => { return componentContext.TryResolve(t, out object o) ? o : null; };
            });
        }

        private void LoadForAssembly(ContainerBuilder builder, Assembly assembly)
        {
            builder.RegisterAssemblyTypes(typeof(IMediator).GetTypeInfo().Assembly).AsImplementedInterfaces();

            // Register all the Command classes (they implement IRequestHandler) in assembly holding the Commands
            builder.RegisterAssemblyTypes(assembly).AsClosedTypesOf(typeof(IRequestHandler<,>));

            // Register the DomainEventHandler classes (they implement INotificationHandler<>) in assembly holding the Domain Events
            builder.RegisterAssemblyTypes(assembly).AsClosedTypesOf(typeof(INotificationHandler<>));

            // Register the Command's Validators (Validators based on FluentValidation library)
            builder.RegisterAssemblyTypes(assembly).Where(t => t.IsClosedTypeOf(typeof(IValidator<>))).AsImplementedInterfaces();
        }
    }
}
