using System.Reflection;
using Domain.Models;

namespace Domain.Extensions;

public static class Helpers
{
    public static IEnumerable<KeyValuePair<string, string>> ParseParameters(this string[] strings)
    {
        var props = typeof(ApplicationParameters).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var propertyInfo in props)
        {
            var i = Array.FindIndex(strings,
                s => s.Equals($"--{propertyInfo.Name.ToLower()}", StringComparison.OrdinalIgnoreCase));
            if (i < 0)
                continue;

            if (propertyInfo.PropertyType.IsAssignableFrom(typeof(bool)))
            {
                if (i < strings.Length - 1 &&
                    bool.TryParse(strings[i + 1], out var boolValue))
                    yield return new KeyValuePair<string, string>(propertyInfo.Name, boolValue.ToString());
                else
                    yield return new KeyValuePair<string, string>(propertyInfo.Name, true.ToString());
            }
            else if (propertyInfo.PropertyType.IsAssignableFrom(typeof(int)))
            {
                var index = Array.FindIndex(strings,
                    s => s.Equals($"--{propertyInfo.Name.ToLower()}", StringComparison.OrdinalIgnoreCase));
                if (index >= 0 && index < strings.Length - 1 &&
                    int.TryParse(strings[index + 1], out var intValue))
                    yield return new KeyValuePair<string, string>(propertyInfo.Name, intValue.ToString());
            }
            else if (propertyInfo.PropertyType.IsAssignableFrom(typeof(string)))
            {
                var index = Array.FindIndex(strings,
                    s => s.Equals($"--{propertyInfo.Name.ToLower()}", StringComparison.OrdinalIgnoreCase));
                if (index >= 0 && index < strings.Length - 1)
                    yield return new KeyValuePair<string, string>(propertyInfo.Name, strings[index + 1]);
            }
        }
    }
}