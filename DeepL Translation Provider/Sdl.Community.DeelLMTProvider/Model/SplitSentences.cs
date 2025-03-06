using System;
using System.ComponentModel;
using System.Reflection;

namespace Sdl.Community.DeepLMTProvider.Model
{
    public enum SplitSentences
    {
        [Description("No Splitting")]
        [ApiValue("0")]
        NoSplitting = 0,  // (Fixed typo: "NoSpliting" → "NoSplitting")

        [Description("Default")]
        [ApiValue("1")]
        Default = 1,

        [Description("No Newlines")]
        [ApiValue("no_newline")]
        NoNewlines = 2
    }

    // Custom attribute to store API values
    public class ApiValueAttribute : Attribute
    {
        public string Value { get; }
        public ApiValueAttribute(string value) => Value = value;
    }

    public static class EnumExtensions
    {
        // Get the API value from the enum
        public static string GetApiValue(this Enum enumValue)
        {
            var field = enumValue.GetType().GetField(enumValue.ToString());
            var attribute = field?.GetCustomAttribute<ApiValueAttribute>();
            return attribute != null ? attribute.Value : enumValue.ToString();
        }

        // Get the display-friendly name from the enum
        public static string GetDescription(this Enum enumValue)
        {
            var field = enumValue.GetType().GetField(enumValue.ToString());
            var attribute = field?.GetCustomAttribute<DescriptionAttribute>();
            return attribute != null ? attribute.Description : enumValue.ToString();
        }
    }
}
