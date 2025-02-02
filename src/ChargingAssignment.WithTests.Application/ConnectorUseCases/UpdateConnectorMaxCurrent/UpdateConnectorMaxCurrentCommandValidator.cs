using FluentValidation;

namespace CharginAssignment.WithTests.Application.ConnectorUseCases.UpdateConnectorMaxCurrent;

public class UpdateConnectorMaxCurrentCommandValidator : AbstractValidator<UpdateConnectorMaxCurrentCommand>
{
    public UpdateConnectorMaxCurrentCommandValidator()
    {
        RuleFor(x => x.MaxCurrent)
            .GreaterThan(0).WithMessage("Max current should be greater than zero");
    }
}