namespace CharginAssignment.WithTests.Application.GroupUseCases.GetGroupById;

public record GetGroupByIdQueryResponse(Guid Id, string? Name, int Capacity);