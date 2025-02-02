using FluentValidation;

namespace CharginAssignment.WithTests.Application.GroupUseCases.UpdateGroup;

public class UpdateGroupCommandValidator : AbstractValidator<UpdateGroupCommand>
{
    public UpdateGroupCommandValidator()
    {
        RuleFor(x => x.GroupId).NotEmpty();

        RuleFor(x => x.Name).NotEmpty().WithMessage("Group name cannot be empty");

        RuleFor(x => x.Capacity)
            .GreaterThan(0).WithMessage("Capacity value should be greater than zero");
    }
}