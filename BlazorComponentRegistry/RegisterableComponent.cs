using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System.Reflection;

namespace BlazorComponentRegistry
{
    public class RegisterableComponent : ComponentBase, IDisposable
    {
        public string ComponentGuid { get; internal set; }
        [Inject] IComponentRegistryService componentRegistry { get; set; }
        [Parameter] public string ParentComponentGuid { get; set; }
        [Parameter] public bool IncludeInheritedProperties { get; set; } = false;
        [Parameter] public bool IncludeNonParameterProperties { get; set; } = true;
        [Parameter] public bool IncludeCascadingParameterProperties { get; set; } = true;

        protected override Task OnInitializedAsync()
        {
            ComponentGuid = componentRegistry.RegisterComponent(ParentComponentGuid, GetType()).ComponentGuid;
            return base.OnInitializedAsync();
        }

        protected override void OnParametersSet()
        {
            componentRegistry.UpdateComponentParameters(ComponentGuid, this.GetPublicProperties(IncludeInheritedProperties, IncludeCascadingParameterProperties, IncludeNonParameterProperties));
            base.OnParametersSet();
        }

        public void Dispose()
        {
            componentRegistry.UnregisterComponent(ComponentGuid);
        }
    }
}
