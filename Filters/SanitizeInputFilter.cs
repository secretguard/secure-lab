using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;
using System.Collections;

public class SanitizeInputFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        // Only sanitize for POST requests
        if (!string.Equals(context.HttpContext.Request.Method, "POST", System.StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        foreach (var kvp in context.ActionArguments.ToArray())
        {
            var value = kvp.Value;
            if (value is null) continue;
            if (value is string s)
            {
                context.ActionArguments[kvp.Key] = CleanString(s);
            }
            else
            {
                SanitizeObjectGraph(value, new HashSet<object>());
            }
        }
    }

    public void OnActionExecuted(ActionExecutedContext context) { }

    private static void SanitizeObjectGraph(object obj, HashSet<object> visited)
    {
        if (obj == null) return;
        if (!visited.Add(obj)) return; // prevent cycles

        // Don't attempt to sanitize primitive/value types other than string (already handled)
        var type = obj.GetType();
        if (type.IsPrimitive || type.IsEnum)
            return;

        if (obj is IEnumerable enumerable && obj is not string)
        {
            foreach (var item in enumerable)
            {
                if (item != null)
                    SanitizeObjectGraph(item, visited);
            }
        }

        var props = type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
            .Where(p => p.CanRead && p.CanWrite);
        foreach (var p in props)
        {
            try
            {
                var current = p.GetValue(obj);
                if (current is string str)
                {
                    p.SetValue(obj, CleanString(str));
                }
                else if (current != null)
                {
                    SanitizeObjectGraph(current, visited);
                }
            }
            catch { /* ignore property set exceptions */ }
        }
    }

    private static string CleanString(string input)
    {
        if (string.IsNullOrEmpty(input)) return string.Empty;
        input = input.Trim();
        var sb = new StringBuilder(input.Length);
        foreach (var c in input)
        {
            if (c == '\n' || c == '\t' || !char.IsControl(c))
                sb.Append(c);
        }
        // Optional: enforce a max length globally, e.g., 512
        const int max = 512;
        if (sb.Length > max)
            return sb.ToString(0, max);
        return sb.ToString();
    }
}
