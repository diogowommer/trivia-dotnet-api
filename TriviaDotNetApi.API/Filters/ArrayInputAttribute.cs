using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace TriviaDotNetApi.API.Filters
{
    public class ArrayInputAttribute : ActionFilterAttribute
    {
        private readonly string[] _ParameterNames;
        /// <summary>
        /// 
        /// </summary>
        public string Separator { get; set; }
        /// <summary>
        /// cons
        /// </summary>
        /// <param name="parameterName"></param>
        public ArrayInputAttribute(params string[] parameterName)
        {
            _ParameterNames = parameterName;
            Separator = ",";
        }

        /// <summary>
        /// 
        /// </summary>
        public void ProcessArrayInput(ActionExecutingContext actionContext, string parameterName)
        {
            if (actionContext.ActionArguments.ContainsKey(parameterName))
            {
                var parameterDescriptor = actionContext.ActionDescriptor.Parameters.FirstOrDefault(param => param.Name == parameterName);
                if (parameterDescriptor != null && parameterDescriptor.ParameterType.IsArray)
                {
                    var type = parameterDescriptor.ParameterType.GetElementType();
                    var parameters = String.Empty;
                    if (actionContext.RouteData.Values.ContainsKey(parameterName))
                    {
                        parameters = (string)actionContext.RouteData.Values[parameterName];
                    }
                    else
                    {
                        var queryString = actionContext.HttpContext.Request.Query;
                        if (queryString[parameterName].FirstOrDefault() != null)
                        {
                            parameters = queryString[parameterName];
                        }
                    }

                    var values = parameters.Split(new[] { Separator }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(TypeDescriptor.GetConverter(type).ConvertFromString).ToArray();
                    var typedValues = Array.CreateInstance(type, values.Length);
                    values.CopyTo(typedValues, 0);
                    actionContext.ActionArguments[parameterName] = typedValues;
                }
            }
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {

            _ParameterNames.ToList().ForEach(parameterName => ProcessArrayInput(context, parameterName));
            base.OnActionExecuting(context);
        }

    }
}
