using System;
using System.Collections.Immutable;

namespace BlazorComponentRegistry.Services
{
    public class ComponentRegistryService : IComponentRegistryService
    {
        private List<ComponentRegistryEntry> _components = new List<ComponentRegistryEntry>();
        public event ComponentTreeChangedEventHandler ComponentTreeChanged;
        public delegate void ComponentTreeChangedEventHandler();
        protected virtual void OnComponentTreeChanged()
        {
            ComponentTreeChanged?.Invoke();
        }

        public ComponentRegistryEntry RegisterComponent(string parentComponentGuid, Type type, string guid = null, ImmutableArray<ComponentRegistryEntryProperty> properties = new(), ImmutableArray<string> routes = new())
        {
            if (type == null)
            {
                throw new ArgumentException("Type cannot be null.", nameof(type));
            }
            if (guid == null)
            {
                guid = Guid.NewGuid().ToString();
            }
            if (parentComponentGuid == guid)
            {
                throw new ArgumentException("parentComponentGuid matches componentGuid. An instance of a component cannot be its own parent.", nameof(guid));
            }
            ComponentRegistryEntry entry = new()
            {
                Type = type,
                ComponentGuid = guid,
                //ComponentName = componentName,
                ParentComponentGuid = parentComponentGuid,
                Properties = properties,
                Routes = routes
            };
            var selfEntry = FindEntryInTree(guid);
            if (selfEntry != null)
            {
                throw new Exception("A component with specified componentGuid already exists in the tree.");
            }
            entry.ParentComponent = FindEntryInTree(parentComponentGuid);
            if (entry.ParentComponent != null)
            {
                entry.ParentComponent.ChildComponents.Add(entry);
            }
            else
            {
                _components.Add(entry);
            }
            OnComponentTreeChanged();
            return entry;
        }

        public void UpdateComponentProperties(string guid, ImmutableArray<ComponentRegistryEntryProperty> properties = new())
        {
            var selfEntry = FindEntryInTree(guid);
            selfEntry.Properties = properties;
            OnComponentTreeChanged();
        }

        public ComponentRegistryEntry FindEntryInTree(string guid)
        {
            ComponentRegistryEntry foundComponent = null;
            Queue<ComponentRegistryEntry> componentQueue = new();
            foreach (var component in _components)
            {
                componentQueue.Enqueue(component);
            }
            while (componentQueue.Count > 0)
            {
                var component = componentQueue.Dequeue();
                if (component.ComponentGuid == guid)
                {
                    foundComponent = component;
                    break;
                }
                else
                {
                    foreach (ComponentRegistryEntry childComponent in component.ChildComponents)
                    {
                        componentQueue.Enqueue(childComponent);
                    }
                }
            }
            return foundComponent;
        }

        private void DeleteEntryInTree(string guid)
        {
            Queue<ComponentRegistryEntry> componentQueue = new();
            foreach (var component in _components)
            {
                if (component.ComponentGuid == guid)
                {
                    _components.Remove(component);
                    return;
                }
                else
                {
                    componentQueue.Enqueue(component);
                }
            }
            while (componentQueue.Count > 0)
            {
                var component = componentQueue.Dequeue();
                if (component.ComponentGuid == guid)
                {
                    component.ParentComponent.ChildComponents.Remove(component);
                    break;
                }
                else
                {
                    foreach (ComponentRegistryEntry childComponent in component.ChildComponents)
                    {
                        componentQueue.Enqueue(childComponent);
                    }
                }
            }
        }

        public void CollapseTree()
        {
            Queue<ComponentRegistryEntry> componentQueue = new();
            foreach (var component in _components)
            {
                componentQueue.Enqueue(component);
                component.Expanded = false;
            }
            while (componentQueue.Count > 0)
            {
                var component = componentQueue.Dequeue();
                component.Expanded = false;
                foreach (ComponentRegistryEntry childComponent in component.ChildComponents)
                {
                    componentQueue.Enqueue(childComponent);
                }
            }
        }

        public void UnregisterComponent(string guid)
        {
            DeleteEntryInTree(guid);
            OnComponentTreeChanged();
        }

        public IImmutableList<ComponentRegistryEntry> GetComponents()
        {
            return _components.ToImmutableList();
        }
    }
}
