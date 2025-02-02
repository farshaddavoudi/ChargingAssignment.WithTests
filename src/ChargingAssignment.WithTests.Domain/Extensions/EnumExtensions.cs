using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace CharginAssignment.WithTests.Domain.Extensions;

public static class EnumExtensions
{
    public static string? ToDisplayName(this Enum value, bool showEnumStringIfNoDisplayName = true)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value));

        var displayName = value.GetType()
            .GetMember(value.ToString())
            .First()
            ?.GetCustomAttributes<DisplayAttribute>()
            .FirstOrDefault()
            ?.GetName();
        return displayName ?? (showEnumStringIfNoDisplayName ? value.ToString() : null);
    }

    public static string? ToDisplayProperties(this System.Enum value, EnumDisplayProperty property = EnumDisplayProperty.Name)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value));

        var attribute = value.GetType()
            .GetField(value.ToString())?.GetCustomAttributes<DisplayAttribute>(false)
            .FirstOrDefault();

        if (attribute == null)
            return value.ToString();

        var propValue = attribute.GetType().GetProperty(property.ToString())?.GetValue(attribute, null);
        return propValue?.ToString();
    }

    public enum EnumDisplayProperty
    {
        Description,
        GroupName,
        Name,
        Prompt,
        ShortName,
        Order
    }
}