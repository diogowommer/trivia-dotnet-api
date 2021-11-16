using FluentValidation;
using TriviaDotNetApi.Application.Validators.Constants;
using TriviaDotNetApi.Application.Validators.Models;
using System;

namespace TriviaDotNetApi.Application.Validators.Commands
{
    public class CreateTriviaCommandValidator : AbstractValidator<CreateTriviaCommand>
    {
        public CreateTriviaCommandValidator() =>
            base.RuleFor(x => x.Payload)
                .NotNull()
                .WithMessage(String.Format("The field {0} is required", nameof(CreateTriviaCommand.Payload)))
                .WithErrorCode(ErrorCode.F050101)
                .SetValidator(new TriviaValidator());
    }
}
