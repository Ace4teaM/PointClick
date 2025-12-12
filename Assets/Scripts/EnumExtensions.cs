using System;
using System.ComponentModel;
using System.Reflection;

public static class EnumExtensions
{
    public static string GetDescription(this Enum value)
    {
        FieldInfo fi = value.GetType().GetField(value.ToString());
        DescriptionAttribute[] attributes =
            (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

        return (attributes.Length > 0) ? attributes[0].Description : value.ToString();
    }

    public static bool TryParseFromDescription<T>(string description, bool ignoreCase, out object value)
    {
        foreach (var field in typeof(T).GetFields())
        {
            if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
            {
                if (String.Compare(attribute.Description, description, ignoreCase) == 0)
                {
                    value = (T)field.GetValue(null);
                    return true;
                }
            }
        }

        value = null;
        return false;
    }

}