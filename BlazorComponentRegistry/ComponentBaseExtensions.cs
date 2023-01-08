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
                .Where(prop => prop.AttributeType == typeof(RouteAttribute))
                .Select(prop => prop.ConstructorArguments.FirstOrDefault())
                    .Where(conArg => conArg != null)
                    .Select(conArg => conArg.Value.ToString())
                    .ToImmutableArray();
        }

        public static ImmutableArray<ComponentRegistryEntryProperty> GetPublicProperties(this ComponentBase component, bool includeInherited = true, bool includeCascading = true, bool includeNonParameters = true)
        {
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance;
            if (!includeInherited)
            {
                bindingFlags = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance;
            }
            List<ComponentRegistryEntryProperty> returnProperties = new();
            List<PropertyInfo> properties = component
                .GetType()
                .GetProperties(bindingFlags).ToList();

            foreach (var property in properties)
            {
                if (property.GetCustomAttribute(typeof(CascadingParameterAttribute)) != null)
                {
                    var value = property.GetValue(component);
                    returnProperties.Add(new ComponentRegistryEntryProperty() { 
                        Name = property.Name, 
                        Value = value, 
                        Category = "CascadingParameter" 
                    });
                }
                if (property.GetCustomAttribute(typeof(ParameterAttribute)) != null)
                {
                    var value = property.GetValue(component);
                    returnProperties.Add(new ComponentRegistryEntryProperty()
                    {
                        Name = property.Name,
                        Value = value,
                        Category = "Parameter"
                    });
                }
                if (property.GetCustomAttribute(typeof(ParameterAttribute)) == null && property.GetCustomAttribute(typeof(CascadingParameterAttribute)) == null)
                {
                    var value = property.GetValue(component);
                    returnProperties.Add(new ComponentRegistryEntryProperty()
                    {
                        Name = property.Name,
                        Value = value,
                        Category = "Uncategorized"
                    });
                }
            }
            return returnProperties.ToImmutableArray();
        }
    }
}
