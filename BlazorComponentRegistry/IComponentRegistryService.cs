using System.Collections.Immutable;
using static BlazorComponentRegistry.ComponentRegistryService;

namespace BlazorComponentRegistry
{
    public interface IComponentRegistryService
    {
        public event ComponentTreeChangedEventHandler ComponentTreeChanged;
        public ComponentRegistryEntry RegisterComponent(string parentComponentGuid, Type type, string guid = null, Dictionary<string, object?> parameters = null);
        public void UnregisterComponent(string guid);
        public void UpdateComponentParameters(string guid, Dictionary<string, object?> parameters);
        public IImmutableList<ComponentRegistryEntry> GetComponents();
    }
}
