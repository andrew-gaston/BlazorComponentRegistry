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
        //[Parameter] public bool IncludeInheritedProperties { get; set; } = false;
        //[Parameter] public bool IncludeNonParameterProperties { get; set; } = true;
        //[Parameter] public bool IncludeCascadingParameterProperties { get; set; } = true;

        protected override Task OnInitializedAsync()
        {
            ComponentGuid = componentRegistry.RegisterComponent(ParentComponentGuid, GetType(), routes: this.GetRoutes()).ComponentGuid;
            return base.OnInitializedAsync();
        }

        protected override void OnParametersSet()
        {
            componentRegistry.UpdateComponentParameters(ComponentGuid, this.GetPublicProperties());
            base.OnParametersSet();
        }

        public void Dispose()
        {
            componentRegistry.UnregisterComponent(ComponentGuid);
        }

        //protected override void BuildRenderTree(RenderTreeBuilder builder)
        //{
        //    base.BuildRenderTree(builder);
        //    builder.OpenRegion(0);
        //    builder.OpenComponent<CascadingValue<string>>(1);
        //    builder.AddAttribute(1, "TValue", typeof(string));
        //    builder.AddAttribute(2, "Value", ComponentGuid);
        //    builder.AddAttribute(3, "Name", "ParentComponentGuid");
        //    builder.CloseComponent();
        //    builder.CloseRegion();
        //}
    }
}
