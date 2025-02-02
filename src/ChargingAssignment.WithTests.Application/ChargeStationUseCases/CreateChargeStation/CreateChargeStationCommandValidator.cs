using FluentValidation;
using CharginAssignment.WithTests.Application.ChargeStationUseCases.GetChargeStationById;

namespace CharginAssignment.WithTests.Application.ChargeStationUseCases.CreateChargeStation;

public class CreateChargeStationCommandValidator : AbstractValidator<CreateChargeStationCommand>
{
    public CreateChargeStationCommandValidator()
    {
        RuleFor(x => x.GroupId).NotEmpty().WithMessage("Please enter the GroupId");

        RuleFor(x => x.Name).NotEmpty().WithMessage("Charge Station name cannot be empty");

        RuleFor(x => x.Connectors).NotEmpty().WithMessage("Creating a Charge Station needs at least one Connector");

        RuleFor(x => x.Connectors).Must(DoNotBeMoreThan5Connectors).WithMessage("A Charge Station cannot have more than 5 Connectors");

        RuleFor(x => x.Connectors).Must(EachConnectorBeValid).WithMessage("At least one Connector isn't valid");

        RuleFor(x => x.Connectors).Must(HaveNoDuplicatedId).WithMessage("Connector IDs have to be unique");
    }

    private bool HaveNoDuplicatedId(List<ConnectorDto>? connectors)
    {
        if (connectors is null) return true;

        if (connectors.Count != connectors.Select(c => c.Id).Distinct().Count())
        {
            return false;
        }

        return true;
    }

    private bool EachConnectorBeValid(List<ConnectorDto>? connectors)
    {
        if (connectors is null) return true; //Unrelated

        // ConnectorId range of values are from 1 to 5
        if (connectors.Any(c => c.Id is < 1 or > 5))
        {
            return false;
        }

        // MaxCapacity should be greater than zero
        if (connectors.Any(c => c.MaxCurrent < 1))
        {
            return false;
        }

        return true;
    }

    private bool DoNotBeMoreThan5Connectors(List<ConnectorDto>? connectors)
    {
        if (connectors is null) return true; //Unrelated

        return connectors.Count <= 5;
    }
}