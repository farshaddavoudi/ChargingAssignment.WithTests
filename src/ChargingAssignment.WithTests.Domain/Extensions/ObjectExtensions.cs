using System.Text.Json;

namespace CharginAssignment.WithTests.Domain.Extensions;

public static class ObjectExtensions
{
    public static T Clone<T>(this object value)
    {
        return JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(value)) ?? throw new InvalidOperationException();
    }

    public static T? GetPropertyValue<T>(this object obj, string propName)
    {
        return (T?)obj.GetType().GetProperty(propName)?.GetValue(obj, null);
    }

    public static object? GetPropertyValue(this object obj, string propName)
    {
        return obj.GetType().GetProperty(propName)?.GetValue(obj, null);
    }
}