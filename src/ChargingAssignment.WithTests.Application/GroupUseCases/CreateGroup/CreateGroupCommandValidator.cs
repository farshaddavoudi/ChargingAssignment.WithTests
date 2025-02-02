using FluentValidation;

namespace CharginAssignment.WithTests.Application.GroupUseCases.CreateGroup;

public class CreateGroupCommandValidator : AbstractValidator<CreateGroupCommand>
{
    public CreateGroupCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Group name cannot be empty");

        RuleFor(x => x.Capacity)
            .GreaterThan(0).WithMessage("Capacity value should be greater than zero");
    }
}