using Microsoft.AspNetCore.Components;
using System.Reflection;

namespace BlazorComponentRegistry
{
    public static class ComponentBaseExtensions
    {
        public static Dictionary<string, object?> GetParameters(this ComponentBase component)
        {
            Dictionary<string, object?> parameters = new();
            var properties = component
                .GetType()
                .GetProperties();
            foreach (var property in properties)
            {
                if (property.GetCustomAttribute(typeof(ParameterAttribute)) != null || property.GetCustomAttribute(typeof(CascadingParameterAttribute)) != null)
                {
                    var value = property.GetValue(component);
                    parameters.Add(property.Name, value);
                }
            }
            return parameters;
        }
    }
}
