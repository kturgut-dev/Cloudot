using Cloudot.Shared.Results;
using Cloudot.Shared.Enums;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Localization;

[AttributeUsage(AttributeTargets.Method)]
public class ValidateAttribute : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var localizer = context.HttpContext.RequestServices.GetService<IStringLocalizer<ValidateAttribute>>();

        foreach (var argument in context.ActionArguments.Values)
        {
            if (argument is null) continue;

            var validatorType = typeof(IValidator<>).MakeGenericType(argument.GetType());
            var validator = context.HttpContext.RequestServices.GetService(validatorType);
            if (validator is null) continue;

            var validationContextType = typeof(ValidationContext<>).MakeGenericType(argument.GetType());
            var validationContext = Activator.CreateInstance(validationContextType, argument)!;

            ValidationResult? validationResult = null;

            // async
            var validateAsyncMethod = validatorType.GetMethod("ValidateAsync", new[] { validationContextType, typeof(CancellationToken) });
            if (validateAsyncMethod != null)
            {
                var task = (Task)validateAsyncMethod.Invoke(validator, new object[] { validationContext, context.HttpContext.RequestAborted })!;
                await task.ConfigureAwait(false);
                var resultProp = task.GetType().GetProperty("Result")!;
                validationResult = (ValidationResult)resultProp.GetValue(task)!;
            }
            else
            {
                // sync fallback
                var validateMethod = validatorType.GetMethod("Validate", new[] { validationContextType });
                if (validateMethod != null)
                {
                    validationResult = (ValidationResult)validateMethod.Invoke(validator, new object[] { validationContext })!;
                }
            }

            if (validationResult is not null && !validationResult.IsValid)
            {
                var failMessage = localizer?["ValidationFailed"] ?? "Validation failed";
                var result = Result.Fail(failMessage, 400);

                result.ValidationErrors = validationResult.Errors.Select(error => new MessageItem
                {
                    Code = error.PropertyName,
                    Message = localizer?[error.ErrorMessage] ?? error.ErrorMessage,
                    Level = MessageLevel.Error
                }).ToList();

                context.Result = new JsonResult(result)
                {
                    StatusCode = 400
                };

                return;
            }
        }

        await next();
    }
}
