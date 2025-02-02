using FluentValidation;

namespace CharginAssignment.WithTests.Application.ConnectorUseCases.CreateConnector;

public class CreateConnectorCommandValidator : AbstractValidator<CreateConnectorCommand>
{
    public CreateConnectorCommandValidator()
    {
        RuleFor(x => x.MaxCurrent).GreaterThan(0)
            .WithMessage("Max current in Amps needs to be greater than zero");

        RuleFor(x => x.ConnectorId)
            .GreaterThan(1).WithMessage("Connector ID should be greater or equal to 1")
            .LessThanOrEqualTo(5).WithMessage("Connector ID should not be greater than 5");
    }
}