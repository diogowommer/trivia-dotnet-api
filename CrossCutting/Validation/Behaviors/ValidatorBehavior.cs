using CrossCutting.MessageHelpers;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CrossCutting.Validation.Behaviors
{    public class ValidatorBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
           where TRequest : IRequest<TResponse> where TResponse : Response
    {
        private readonly IEnumerable<IValidator> validators;

        public ValidatorBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            this.validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var failures = this.validators
                .Select(v => v.Validate(request))
                .SelectMany(result => result.Errors)
                .Where(f => f != null)
                .ToList();


            if (failures.Any())
                return await Errors(failures);
            else
                return await next();
        }

        private static Task<TResponse> Errors(IEnumerable<ValidationFailure> failures)
        {
            var response = new Response();

            foreach (var failure in failures)
            {
                if (!String.IsNullOrEmpty(failure.ErrorCode))
                    response.AddError(failure.PropertyName, String.Format("{0}: {1}", failure.ErrorCode, failure.ErrorMessage));
                else
                    response.AddError(failure.PropertyName, String.Format("0000: {0}", failure.ErrorMessage));
            }

            return Task.FromResult(response as TResponse);
        }
    }
}
