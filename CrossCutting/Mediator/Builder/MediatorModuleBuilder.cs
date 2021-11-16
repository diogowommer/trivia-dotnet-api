using CrossCutting.Mediator.Common.MediatorModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrossCutting.Mediator.Builder
{
    public class MediatorModuleBuilder
    {
        private List<string> _assemblies;
        private List<Type> _behaviors;

        public MediatorModuleBuilder()
        {
            _assemblies = new List<string>();
            _behaviors = new List<Type>();
        }

        public MediatorModule Build()
        {
            return new MediatorModule(_behaviors.Distinct().ToArray(), _assemblies.Distinct().ToArray());
        }

        public MediatorModuleBuilder AddAssembly(params string[] assembly)
        {
            _assemblies.AddRange(assembly);
            return this;
        }

        public MediatorModuleBuilder AddBehavior(params Type[] behaviors)
        {
            _behaviors.AddRange(behaviors);
            return this;
        }
    }
}
