using BlazorComponentRegistry.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System.Reflection;

namespace BlazorComponentRegistry
{
    public abstract class RegisterableComponent : ComponentBase, IDisposable
    {
        public string ComponentGuid { get; internal set; }
        [Inject] IComponentRegistryService componentRegistry { get; set; }
        [Parameter] public string ParentComponentGuid { get; set; }
        //[CascadingParameter] public string RootComponentGuid { get; set; }

        protected override Task OnInitializedAsync()
        {
            ComponentGuid = componentRegistry.RegisterComponent(ParentComponentGuid, GetType(), routes: this.GetRoutes()).ComponentGuid;
            return base.OnInitializedAsync();
        }

        protected override void OnParametersSet()
        {
            componentRegistry.UpdateComponentProperties(ComponentGuid, this.GetPublicProperties());
            base.OnParametersSet();
        }

        public void Dispose()
        {
            componentRegistry.UnregisterComponent(ComponentGuid);
        }
    }
}
