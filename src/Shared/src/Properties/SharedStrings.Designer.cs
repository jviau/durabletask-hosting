// <auto-generated />

using System.Resources;
using System.Globalization;

namespace DurableTask.Shared
{
    internal static class SharedStrings
    {
        private static readonly ResourceManager _resourceManager
            = new ResourceManager("Shared.Properties.SharedStrings", typeof(SharedStrings).Assembly);

        /// <summary>
        ///     Type ['{type}'] must inherit from '{baseType}', be a class, and not be abstract"
        /// </summary>
        public static string InvalidType(object type, object baseType)
            => string.Format(
                GetString("InvalidType", nameof(type), nameof(baseType)),
                type, baseType,
                CultureInfo.CurrentUICulture);

        /// <summary>
        ///     Type ['{type}'] must be an interface.
        /// </summary>
        public static string NotInterface(object type)
            => string.Format(
                GetString("NotInterface", nameof(type)),
                type,
                CultureInfo.CurrentUICulture);

        /// <summary>
        ///     '{name}' cannot be an empty string or start with the null character.
        /// </summary>
        public static string StringEmpty(object name)
            => string.Format(
                GetString("StringEmpty", nameof(name)),
                name,
                CultureInfo.CurrentUICulture);

        private static string GetString(string name, params string[] formatterNames)
        {
            var value = _resourceManager.GetString(name);
            for (var i = 0; i < formatterNames.Length; i++)
            {
                value = value.Replace("{" + formatterNames[i] + "}", "{" + i + "}");
            }

            return value;
        }
    }
}
