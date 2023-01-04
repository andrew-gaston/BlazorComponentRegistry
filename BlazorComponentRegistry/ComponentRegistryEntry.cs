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
        public Dictionary<string, object?> Parameters { get; set; } = new();
        public bool Expanded { get; set; } = false;
    };
}
