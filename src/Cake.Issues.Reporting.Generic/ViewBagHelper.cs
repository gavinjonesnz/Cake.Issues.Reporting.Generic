namespace Cake.Issues.Reporting.Generic
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Helper for working with the ViewBag in templates.
    /// </summary>
    public static class ViewBagHelper
    {
        /// <summary>
        /// Returns the value or a default value if <paramref name="value"/> is null.
        /// </summary>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <param name="value">Value which should be returned.</param>
        /// <param name="defaultValue">Value which should be returned if <paramref name="value"/> is null.</param>
        /// <returns><paramref name="value"/> or <paramref name="defaultValue"/> if <paramref name="value"/> is null.</returns>
        public static T ValueOrDefault<T>(object value, T defaultValue)
        {
            if (value != null)
            {
                return (T)value;
            }

            return defaultValue;
        }

        /// <summary>
        /// Returns a list of ViewBag properties gleaned from the given template via regex.
        /// </summary>
        /// <param name="template">Razor template to examine.</param>
        /// <param name="defaultoptions">Default values for the returned dictionary.</param>
        /// <returns>Dictionary containing keys corresponding to ViewBag properties, with default null values</returns>
        public static IDictionary<string, object> ParsePropertiesFromTemplate(string template, Dictionary<string, object> defaultoptions)
        {
            Dictionary<string, object> ret = new Dictionary<string, object>(defaultoptions);
            Regex r = new Regex(@"ViewBag\.[a-zA-Z0-9_]*\b", RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);

            var matches = r.Matches(template);
            foreach (Match m in matches)
            {
                var key = m.Value.Replace("ViewBag.", string.Empty);
                if (!ret.ContainsKey(key))
                {
                    ret.Add(key, null);
                }
            }

            return ret;
        }
    }
}
