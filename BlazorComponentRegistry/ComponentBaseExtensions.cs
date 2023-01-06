using Microsoft.AspNetCore.Components;
using System.Reflection;

namespace BlazorComponentRegistry
{
    public static class ComponentBaseExtensions
    {
        public static Dictionary<string, object?> GetPublicProperties(this ComponentBase component, bool includeInherited = false, bool includeCascading = true, bool includeNonParameters = true)
        {
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance;
            if (!includeInherited)
            {
                bindingFlags = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance;
            }
            Dictionary<string, object?> parameters = new();
            List<PropertyInfo> properties = component
                .GetType()
                .GetProperties(bindingFlags).ToList();

            foreach (var property in properties)
            {
                if (includeCascading && property.GetCustomAttribute(typeof(CascadingParameterAttribute)) != null)
                {
                    var value = property.GetValue(component);
                    parameters.Add(property.Name, value);
                }
                if (property.GetCustomAttribute(typeof(ParameterAttribute)) != null)
                {
                    var value = property.GetValue(component);
                    parameters.Add(property.Name, value);
                }
                if (includeNonParameters && property.GetCustomAttribute(typeof(ParameterAttribute)) == null && property.GetCustomAttribute(typeof(CascadingParameterAttribute)) == null)
                {
                    var value = property.GetValue(component);
                    parameters.Add(property.Name, value);
                }
            }
            return parameters;
        }
    }
}
