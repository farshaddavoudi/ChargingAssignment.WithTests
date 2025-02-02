namespace CharginAssignment.WithTests.Application.Common.Exceptions;

public class GroupCapacityExceedsException(int currentGroupCapacity, int currentConnectorsMaxCurrentSum)
    : BusinessLogicException(
        $"Group's capacity (currently = {currentGroupCapacity}) should be greater than or equal to the sum of max-current related Connectors (currently = {currentConnectorsMaxCurrentSum}) after operation. {currentGroupCapacity - currentConnectorsMaxCurrentSum} Amps are left.");