using System.Reflection;

namespace CharginAssignment.WithTests.Domain.Constants;

public static class AppMetadataConst
{
    public static readonly string AppVersion = Assembly.GetExecutingAssembly().GetName().Version!.ToString(3);

    public static readonly string AppName = "Chargin Assignment with Tests";

    public static readonly string SolutionName = "CharginAssignment.WithTests";
}