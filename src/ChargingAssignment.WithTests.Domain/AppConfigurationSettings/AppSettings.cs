namespace CharginAssignment.WithTests.Domain.AppConfigurationSettings;

public class AppSettings
{
    public ConnStrSettings? ConnStrSettings { get; set; }

    /// <summary>
    /// Is development environment, if false it is production
    /// </summary>
    public bool IsDevelopment { get; set; }

    public bool IsProduction => IsDevelopment is false;
}