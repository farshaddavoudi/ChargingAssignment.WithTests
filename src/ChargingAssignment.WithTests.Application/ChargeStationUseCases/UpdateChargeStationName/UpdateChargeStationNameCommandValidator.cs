using FluentValidation;

namespace CharginAssignment.WithTests.Application.ChargeStationUseCases.UpdateChargeStationName;

public class UpdateChargeStationNameCommandValidator : AbstractValidator<UpdateChargeStationNameCommand>
{
    public UpdateChargeStationNameCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name cannot be empty");
    }
}