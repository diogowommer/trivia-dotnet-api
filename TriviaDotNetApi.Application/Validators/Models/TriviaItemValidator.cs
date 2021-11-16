using FluentValidation;
using TriviaDotNetApi.Application.Models;
using TriviaDotNetApi.Application.Validators.Constants;
using System;
using System.Collections.Generic;
using System.Text;

namespace TriviaDotNetApi.Application.Validators.Models
{
    public class TriviaItemValidator : AbstractValidator<TriviaItemModel>
    {
        public TriviaItemValidator()
        {
            base.RuleFor(x => x.category)
                .NotEmpty()
                .WithMessage(String.Format("The field {0} is required", nameof(TriviaItemModel.category)))
                .WithErrorCode(ErrorCode.F050101);

            base.RuleFor(x => x.correct_answer)
                .NotEmpty()
                .WithMessage(String.Format("The field {0} is required", nameof(TriviaItemModel.correct_answer)))
                .WithErrorCode(ErrorCode.F050101);

            base.RuleFor(x => x.difficulty)
                .NotEmpty()
                .WithMessage(String.Format("The field {0} is required", nameof(TriviaItemModel.difficulty)))
                .WithErrorCode(ErrorCode.F050101);

            base.RuleFor(x => x.question)
                .NotEmpty()
                .WithMessage(String.Format("The field {0} is required", nameof(TriviaItemModel.question)))
                .WithErrorCode(ErrorCode.F050101);

            base.RuleFor(x => x.type)
                .NotEmpty()
                .WithMessage(String.Format("The field {0} is required", nameof(TriviaItemModel.type)))
                .WithErrorCode(ErrorCode.F050101);

        }
    }
}
