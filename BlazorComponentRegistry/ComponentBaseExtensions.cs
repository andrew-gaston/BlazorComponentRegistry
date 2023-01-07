using Microsoft.AspNetCore.Components;
using System.Collections.Immutable;
using System.Reflection;

namespace BlazorComponentRegistry
{
    public static class ComponentBaseExtensions
    {
        public static ImmutableArray<string> GetRoutes(this ComponentBase component)
        {
            return component.GetType().CustomAttributes
                .Where(ca => ca.AttributeType == typeof(RouteAttribute))
                .Select(p => p.ConstructorArguments.FirstOrDefault())
                .Where(conArg => conArg != null)
                .Select(conArg => conArg.Value.ToString())
                .ToImmutableArray();
        }

        public static Dictionary<string, object?> GetPublicProperties(this ComponentBase component, bool includeInherited = true, bool includeCascading = true, bool includeNonParameters = true)
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
