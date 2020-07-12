namespace Cake.Issues.Reporting.Generic
{
    using System.Collections.Generic;
    using System.Dynamic;

    public static class DictionaryExtensions
    {
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                collection.Add(item);
            }
        }

        public static dynamic ToExpandoObject(this IDictionary<string, object> source)
        {
            var ex = new ExpandoObject();
            ex.AddRange(source);
            return ex;
        }
    }
}