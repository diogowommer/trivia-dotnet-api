using CrossCutting.Mediator.Builder;
using CrossCutting.Validation.Behaviors;

namespace CrossCutting.Validation.Extensions
{

    public static class ValidatorBehaviorExtensions
    {
        public static MediatorModuleBuilder AddValidationBehavior(this MediatorModuleBuilder mediatorModuleBuilder)
        {
            mediatorModuleBuilder.AddBehavior(typeof(ValidatorBehavior<,>));
            return mediatorModuleBuilder;
        }


    }
}
