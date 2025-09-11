using System.ComponentModel.DataAnnotations;

namespace ECommerce.Presentation.Enums.Extensions;

public static class EnumExtensions
{
    public static string GetDisplayName(this Enum enumValue)
    {
        var fieldInfo = enumValue
            .GetType()
            .GetField(enumValue.ToString());

        if (fieldInfo is not null)
        {
            var descriptionAttributes =
                fieldInfo.GetCustomAttributes(typeof(DisplayAttribute), false) as DisplayAttribute[];

            if (descriptionAttributes is not null && descriptionAttributes.Length > 0)
            {
                return descriptionAttributes[0].Name!;
            }
        }

        return enumValue.ToString();
    }
}