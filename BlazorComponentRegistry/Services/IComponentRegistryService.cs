using System.Collections.Immutable;
using static BlazorComponentRegistry.Services.ComponentRegistryService;

namespace BlazorComponentRegistry.Services
{
    public interface IComponentRegistryService
    {
        public event ComponentTreeChangedEventHandler ComponentTreeChanged;
        public ComponentRegistryEntry RegisterComponent(string parentComponentGuid, Type type, string guid = null, ImmutableArray<ComponentRegistryEntryProperty> properties = new(), ImmutableArray<string> routes = new());
        public void UnregisterComponent(string guid);
        public void UpdateComponentProperties(string guid, ImmutableArray<ComponentRegistryEntryProperty> properties);
        public IImmutableList<ComponentRegistryEntry> GetComponents();
        public void CollapseTree();
    }
}
