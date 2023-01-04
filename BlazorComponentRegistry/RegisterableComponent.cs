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

        protected override Task OnInitializedAsync()
        {
            ComponentGuid = componentRegistry.RegisterComponent(ParentComponentGuid, GetType()).ComponentGuid;
            return base.OnInitializedAsync();
        }

        protected override void OnParametersSet()
        {
            componentRegistry.UpdateComponentParameters(ComponentGuid, this.GetParameters());
            base.OnParametersSet();
        }

        public void Dispose()
        {
            componentRegistry.UnregisterComponent(ComponentGuid);
        }
    }
}
