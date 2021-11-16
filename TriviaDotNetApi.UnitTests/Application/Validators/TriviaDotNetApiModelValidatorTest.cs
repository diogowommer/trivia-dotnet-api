using AutoMapper;
using MediatR;
using Microsoft.Extensions.Configuration;
using Moq;
using TriviaDotNetApi.Application;
using TriviaDotNetApi.Application.Models;
using TriviaDotNetApi.Application.Validators.Models;
using TriviaDotNetApi.Domain.AggregatesModel;
using System;
using System.Threading.Tasks;
using Xunit;
using System.Collections.Generic;

namespace TriviaDotNetApi.UnitTests.Application.Validators
{
    public class TriviaValidatorTest
    {

        [Fact]
        public void ValidateSuccess()
        {

            var payload = new TriviaModel()
            {
                response_code = 123,
                results = new List<TriviaItemModel>
                    {
                        new TriviaItemModel
                        {
                            category = "Abc",
                            correct_answer = "Abc",
                            difficulty = "Abc",
                            question = "Abc",
                            type = "Abc",
                        }
                    }
            };



            var validator = new TriviaValidator();

            var result = validator.Validate(payload);

            Assert.True(result.IsValid);

        }

        [Fact]
        public void ValidateEmptyValue()
        {

            var payload = new TriviaModel()
            {
                response_code = 10
            };

            var validator = new TriviaValidator();

            var result = validator.Validate(payload);

            Assert.True(!result.IsValid);

        }
    }
}
