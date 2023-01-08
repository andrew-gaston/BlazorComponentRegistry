using System.Collections.Immutable;

namespace BlazorComponentRegistry
{
    public class ComponentRegistryEntry
    {
        public Type Type { get; set; }
        public string ComponentGuid { get; set; }
        public string ComponentName => Type.Name;
        public string ParentComponentGuid { get; set; }
        public ComponentRegistryEntry ParentComponent { get; set; }
        public List<ComponentRegistryEntry> ChildComponents { get; set; } = new();
        public ImmutableArray<ComponentRegistryEntryProperty> Properties { get; set; } = new();
        public ImmutableArray<string> Routes { get; set; } = new();
        public bool Expanded { get; set; } = false;
    }

    public class ComponentRegistryEntryProperty
    {
        public string Name { get; set; }
        public object? Value { get; set; }
        public string Category { get; set; }
    }
}
