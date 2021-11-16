using CrossCutting.Mediator.Builder;
using CrossCutting.Mediator.DefaultBehaviors;

namespace CrossCutting.Mediator.DefaultBehaviorsExtensions
{    public static class LoggingBehaviorExtension
    {
        public static MediatorModuleBuilder AddLoggingBehavior(this MediatorModuleBuilder mediatorModuleBuilder)
        {
            mediatorModuleBuilder.AddBehavior(typeof(LoggingBehavior<,>));
            return mediatorModuleBuilder;
        }
    }
}
