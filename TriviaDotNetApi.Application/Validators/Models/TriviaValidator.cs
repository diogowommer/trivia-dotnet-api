using FluentValidation;
using TriviaDotNetApi.Application.Models;
using TriviaDotNetApi.Application.Validators.Constants;
using System;
using System.Collections.Generic;
using System.Text;

namespace TriviaDotNetApi.Application.Validators.Models
{
    public class TriviaValidator : AbstractValidator<TriviaModel>
    {
        public TriviaValidator()
        {
            //base.RuleFor(x => x.response_code)
            //    .NotEmpty()
            //    .WithMessage(String.Format("The field {0} is required", nameof(TriviaModel.response_code)))
            //    .WithErrorCode(ErrorCode.F050101);

            base.RuleFor(x => x.results)
                .NotEmpty()
                .WithMessage(String.Format("The field {0} is required", nameof(TriviaModel.results)))
                .WithErrorCode(ErrorCode.F050101);
        }
    }
}
