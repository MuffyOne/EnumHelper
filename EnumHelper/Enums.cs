using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace EnumHelper
{
    public static class Enums
    {
        #region methods
        /// <summary>
        /// Parses a string into an enum.
        /// </summary>
        /// <typeparam name="T">The enum type.</typeparam>
        /// <param name="value">The string to parse.</param>
        /// <returns>An enum value of type T.</returns>
        /// <remarks>This method always ignores the case of the string.</remarks>
        public static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        /// <summary>
        /// Tries to parse a string into an enum. If the conversion is not successful it does not throw an Exception.
        /// </summary>
        /// <typeparam name="TEnum">The enum type.</typeparam>
        /// <param name="value">The string to try and parse.</param>
        /// <param name="returnValue">An object for receiving the value of the enum.</param>
        /// <returns>true if the string could be successfully parsed into an enum. In this case the return value holds the parsed value. false if it could not be parsed. In that case the result is null.</returns>
        public static bool TryParseEnum<TEnum>(string value, out Enum returnValue)
        {
            if (!Enum.IsDefined(typeof(TEnum), value))
            {
                returnValue = null;
                return false;
            }

            returnValue = (Enum)Enum.Parse(typeof(TEnum), value);
            return true;
        }

        /// <summary>
        /// Gets a custom attribute from an enum value.
        /// </summary>
        /// <typeparam  name="TAttribute">The type of the custom attribute.</typeparam>
        /// <param name="value">The enum value.</param>
        /// <returns>The custom attribute forthat enum value. Returns null if it cannot be found.</returns>
        public static TAttribute GetCustomAttribute<TAttribute>(object value)
        {
            Type type = value.GetType();
            return (TAttribute)type.GetField(value.ToString()).GetCustomAttributes(typeof(TAttribute), false).FirstOrDefault();
        }

        /// <summary>
        /// Gets a list of all the custom attributes of a particular type in one enum.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the custom attribute.</typeparam>
        /// <typeparam name="TEnum">The enum type to search through.</typeparam>
        /// <returns>A list of all the custom attributes of that type in the specified enum type.</returns>
        public static List<TAttribute> GetCustomAttributesList<TAttribute, TEnum>()
        {
            var type = typeof(TEnum);
            Array enumValues = Enum.GetValues(type);
            List<TAttribute> result = new List<TAttribute>(enumValues.Length);
            foreach (object enumValue in enumValues)
            {
                var attribute = GetCustomAttribute<TAttribute>(enumValue);
                if (attribute != null)
                {
                    result.Add(attribute);
                }
            }
            return result;
        }

        /// <summary>
        /// Get the Description of a Enum
        /// </summary>
        /// <param name="value">The Enum value of which you want the description</param>
        public static string GetEnumDescription(object value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            if (fi == null)
            {
                return value.ToString();
            }
            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            if (attributes.Length > 0)
                return attributes[0].Description;
            return value.ToString();
        }

        /// <summary>
        /// Gets all descriptions for the enum type.
        /// </summary>
        /// <param name="enumType">The enum type to get the descriptions for.</param>
        /// <returns>An array with all descriptions.</returns>
        public static string[] GetAllEnumDescriptions(Type enumType)
        {
            List<string> descriptions = new List<string>();
            foreach (var value in Enum.GetValues(enumType))
            {
                descriptions.Add(GetEnumDescription(value));
            }
            return descriptions.ToArray();
        }

        /// <summary>
        /// Try the get the Description of a Enum. it rutern true if succed, false if it fails. If it fails the description returned will be empty
        /// </summary>
        /// <param name="value">The Enum value of which you want the description</param>
        /// <param name="enumDescription">Out parameter that contains the description .</param>
        public static bool TryGetEnumDescription(Enum value, out string enumDescription)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            if (fi == null)
            {
                enumDescription = "";
                return false;
            }
            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            if (attributes.Length > 0)
            {
                enumDescription = attributes[0].Description;
                return true;
            }
            enumDescription = "";
            return false;
        }

        /// <summary>
        /// Get the Enum starting from the value of the description  attribute
        /// </summary>
        /// <param name="description">The value you are searching.</param>
        /// <typeparam name="TEnum">The Enum type</typeparam>
        public static TEnum GetValueFromDescription<TEnum>(string description)
        {
            return (TEnum)GetValueFromDescription(typeof(TEnum), description);
        }

        /// <summary>
        /// Get the Enum starting from the value of the description attribute
        /// </summary>
        /// <param name="enumType">The Enum type</param>
        /// <param name="description">The value you are searching.</param>
        public static object GetValueFromDescription(Type enumType, string description)
        {
            if (!enumType.IsEnum) throw new InvalidOperationException();
            foreach (var field in enumType.GetFields())
            {
                var attribute = Attribute.GetCustomAttribute(field,
                    typeof(DescriptionAttribute)) as DescriptionAttribute;
                if (attribute != null)
                {
                    if (attribute.Description == description)
                        return field.GetValue(null);
                }
                else
                {
                    if (field.Name == description)
                        return field.GetValue(null);
                }
            }
            throw new ArgumentException("Not found.", "description");
        }



        /// <summary>
        /// Get the Enum starting from a value of a custom attribute
        /// </summary>
        /// <typeparam name="TEnum">The Enum type that needs to be returned.</typeparam>
        /// <typeparam name="TAttribute">The Custom attribute type.</typeparam>
        /// <typeparam name="TPropertyType">The type of the value you are searching.</typeparam>
        /// <param name="valueToSearch">The value you are searching.</param>
        /// <param name="valueFunc">How to get the value in your custom attribute (Ex. a => a.Value).</param>
        public static TEnum GetValueFromCustomAttribute<TEnum, TAttribute, TPropertyType>(TPropertyType valueToSearch, Func<TAttribute, TPropertyType> valueFunc) where TAttribute : Attribute
        {
            var type = typeof(TEnum);
            if (!type.IsEnum) throw new InvalidOperationException();
            foreach (var field in type.GetFields())
            {
                var attribute = Attribute.GetCustomAttribute(field, typeof(TAttribute)) as TAttribute;
                if (attribute != null)
                {
                    object currentValue = valueFunc.Invoke(attribute);
                    if (currentValue.Equals(valueToSearch))
                        return (TEnum)field.GetValue(null);
                }
            }
            throw new ArgumentException("Not found.", "valueToSearch");
            // or return default(T);
        }

        #endregion
    }
}

