using FluentValidation;

namespace CharginAssignment.WithTests.Application.Common.PipelineBehaviours;

public class HasResponseValidationBehaviour<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var validationBehaviourUtility = new ValidationBehaviourUtility<TRequest, TResponse>();

        return await validationBehaviourUtility.ValidateCommandOrQuery(validators, request, next, cancellationToken);
    }
}

public class WithoutResponseValidationBehaviour<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var validationBehaviourUtility = new ValidationBehaviourUtility<TRequest, TResponse>();

        return await validationBehaviourUtility.ValidateCommandOrQuery(validators, request, next, cancellationToken);
    }
}

public class ValidationBehaviourUtility<TRequest, TResponse>
{
    public async Task<TResponse> ValidateCommandOrQuery(IEnumerable<IValidator<TRequest>> validators, TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var validatorsList = validators.ToList();

        if (validatorsList.Any())
        {
            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(
                validatorsList.Select(v =>
                    v.ValidateAsync(context, cancellationToken)));

            var failures = validationResults
                .Where(r => r.Errors.Any())
                .SelectMany(r => r.Errors)
                .ToList();

            if (failures.Any())
                throw new ValidationException(failures);
        }

        return await next();
    }
}

