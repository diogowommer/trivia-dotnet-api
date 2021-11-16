using FluentValidation;
using TriviaDotNetApi.Application.Models;
using TriviaDotNetApi.Application.Validators.Constants;
using System;

namespace TriviaDotNetApi.Application.Validators.Models
{
    public class TriviaFilterValidator : AbstractValidator<TriviaFilterModel>
    {
        public TriviaFilterValidator()
        {
            base.RuleFor(x => x.amount)
                .NotEmpty()
                .WithMessage(String.Format("The field {0} is required", nameof(TriviaFilterModel.amount)))
                .WithErrorCode(ErrorCode.F050101);

            base.RuleFor(x => x.difficulty)
                .NotEmpty()
                .WithMessage(String.Format("The field {0} is required", nameof(TriviaFilterModel.difficulty)))
                .WithErrorCode(ErrorCode.F050101);

            base.RuleFor(x => x.type)
                .NotEmpty()
                .WithMessage(String.Format("The field {0} is required", nameof(TriviaFilterModel.type)))
                .WithErrorCode(ErrorCode.F050101);
        }
    }
}
