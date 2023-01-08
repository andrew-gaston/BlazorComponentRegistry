namespace andrewgastondev.Tests
{
    public class ComponentRegistryServiceTests
    {
        private class TopLevelComponent : ComponentBase { }
        private class ChildComponent : ComponentBase { }

        [Fact]
        public void RegisterComponent_WithoutComponentGuid_GeneratesComponentGuid()
        {
            // Arrange
            ComponentRegistryService componentRegistry = new();

            // Act
            ComponentRegistryEntry entry = componentRegistry.RegisterComponent(null, typeof(TopLevelComponent));

            // Assert
            Assert.NotNull(entry.ComponentGuid);
            Assert.True(Guid.TryParse(entry.ComponentGuid, out _));
        }

        [Fact]
        public void RegisterComponent_WithComponentGuid_TakesPrecedenceOverGeneratedComponentGuid()
        {
            // Arrange
            ComponentRegistryService componentRegistry = new();
            string componentGuid = Guid.NewGuid().ToString();
            // Act
            ComponentRegistryEntry entry = componentRegistry.RegisterComponent(null, typeof(TopLevelComponent), componentGuid);

            // Assert
            Assert.NotNull(entry.ComponentGuid);
            Assert.Equal(componentGuid, entry.ComponentGuid);
        }

        [Fact]
        public void RegisterComponent_WithoutParentComponentGuid_AddsToTopLevel()
        {
            // Arrange
            ComponentRegistryService componentRegistry = new();

            // Act
            ComponentRegistryEntry entry = componentRegistry.RegisterComponent(null, typeof(TopLevelComponent));

            // Assert
            Assert.Null(entry.ParentComponent);
            Assert.NotEmpty(componentRegistry.GetComponents());
        }

        [Fact]
        public void RegisterComponent_WithParentComponentGuid_PopulatesParentEntry()
        {
            // Arrange
            ComponentRegistryService componentRegistry = new();

            // Act
            ComponentRegistryEntry baseEntry = componentRegistry.RegisterComponent(null, typeof(TopLevelComponent));
            ComponentRegistryEntry subEntry = componentRegistry.RegisterComponent(baseEntry.ComponentGuid, typeof(ChildComponent));

            // Assert
            Assert.NotNull(subEntry.ParentComponent);
        }

        [Fact]
        public void RegisterComponent_WithParentComponentGuid_PopulatesParentsChildren()
        {
            // Arrange
            ComponentRegistryService componentRegistry = new();

            // Act
            ComponentRegistryEntry baseEntry = componentRegistry.RegisterComponent(null, typeof(TopLevelComponent));
            componentRegistry.RegisterComponent(baseEntry.ComponentGuid, typeof(ChildComponent));

            // Assert
            Assert.NotEmpty(baseEntry.ChildComponents);
        }

        public void RegisterComponent_AllowsMultipleOfSameType()
        {
            // Arrange
            ComponentRegistryService componentRegistry = new();

            // Act
            componentRegistry.RegisterComponent(null, typeof(TopLevelComponent));
            componentRegistry.RegisterComponent(null, typeof(TopLevelComponent));

            // Assert
            IReadOnlyList<ComponentRegistryEntry> registeredComponents = componentRegistry.GetComponents();
            Assert.Equal(2, registeredComponents.Count);
        }

        [Fact]
        public void RegisterComponent_SupportsDeepNesting()
        {
            // Arrange
            ComponentRegistryService componentRegistry = new();

            // Act
            ComponentRegistryEntry baseComponent = componentRegistry.RegisterComponent(null, typeof(TopLevelComponent));
            ComponentRegistryEntry component1 = componentRegistry.RegisterComponent(baseComponent.ComponentGuid, typeof(ChildComponent));
            ComponentRegistryEntry component2 = componentRegistry.RegisterComponent(component1.ComponentGuid, typeof(ChildComponent));
            ComponentRegistryEntry component3 = componentRegistry.RegisterComponent(component2.ComponentGuid, typeof(ChildComponent));
            ComponentRegistryEntry component4 = componentRegistry.RegisterComponent(component3.ComponentGuid, typeof(ChildComponent));
            ComponentRegistryEntry component5 = componentRegistry.RegisterComponent(component4.ComponentGuid, typeof(ChildComponent));
            ComponentRegistryEntry component6 = componentRegistry.RegisterComponent(component5.ComponentGuid, typeof(ChildComponent));
            ComponentRegistryEntry component7 = componentRegistry.RegisterComponent(component6.ComponentGuid, typeof(ChildComponent));
            ComponentRegistryEntry component8 = componentRegistry.RegisterComponent(component7.ComponentGuid, typeof(ChildComponent));
            ComponentRegistryEntry component9 = componentRegistry.RegisterComponent(component8.ComponentGuid, typeof(ChildComponent));
            ComponentRegistryEntry component10 = componentRegistry.RegisterComponent(component9.ComponentGuid, typeof(ChildComponent));

            // Assert
            Assert.NotEmpty(baseComponent.ChildComponents);
            Assert.NotEmpty(component1.ChildComponents);
            Assert.NotEmpty(component2.ChildComponents);
            Assert.NotEmpty(component3.ChildComponents);
            Assert.NotEmpty(component4.ChildComponents);
            Assert.NotEmpty(component5.ChildComponents);
            Assert.NotEmpty(component6.ChildComponents);
            Assert.NotEmpty(component7.ChildComponents);
            Assert.NotEmpty(component8.ChildComponents);
            Assert.NotEmpty(component9.ChildComponents);
            Assert.Empty(component10.ChildComponents);
        }

        [Fact]
        public void RegisterComponent_WithEqualParentComponentGuidAndComponentGuid_ThrowsArgumentException()
        {
            // Arrange
            ComponentRegistryService componentRegistry = new();
            componentRegistry.RegisterComponent(null, typeof(TopLevelComponent));
            string componentGuid = Guid.NewGuid().ToString();
            // Assert
            Assert.Throws<ArgumentException>(() =>
            {
                // Act
                // Passing componentGuid as both parentComponentGuid and guid parameter
                componentRegistry.RegisterComponent(componentGuid, typeof(ChildComponent), componentGuid);
            });
        }

        [Fact]
        public void RegisterComponent_WithDuplicateComponentGuid_ThrowsException()
        {
            // Arrange
            ComponentRegistryService componentRegistry = new();
            var baseComponent = componentRegistry.RegisterComponent(null, typeof(TopLevelComponent));

            // Assert
            Assert.Throws<Exception>(() =>
            {
                // Act
                // Passing baseComponent.ComponentGuid as guid parameter of child Component
                componentRegistry.RegisterComponent(null, typeof(ChildComponent), baseComponent.ComponentGuid);
            });
        }

        //[Fact]
        //public void RegisterComponent_RaisesComponentTreeChanged()
        //{
        //    // Arrange
        //    ComponentRegistryService componentRegistry = new();
        //    // Assert
        //    Assert.RaisesAny<ComponentTreeChanged>();
        //    var baseComponent = componentRegistry.RegisterComponent(null, "BaseComponent");

        //    // Assert
        //    Assert.Throws<Exception>(() =>
        //    {
        //        // Act
        //        // Passing baseComponent.ComponentGuid as guid parameter of child Component
        //        componentRegistry.RegisterComponent(null, "SubComponent", baseComponent.ComponentGuid);
        //    });
        //}

        [Fact]
        public void UnregisterComponent_RemovesTopLevelComponents()
        {
            // Arrange
            ComponentRegistryService componentRegistry = new();
            var baseComponent = componentRegistry.RegisterComponent(null, typeof(TopLevelComponent));

            // Act
            componentRegistry.UnregisterComponent(baseComponent.ComponentGuid);

            // Assert
            Assert.Empty(componentRegistry.GetComponents());
        }

        [Fact]
        public void UnregisterComponent_RemovesDeeplyNestedComponents()
        {
            // Arrange
            ComponentRegistryService componentRegistry = new();
            var baseComponent = componentRegistry.RegisterComponent(null, typeof(TopLevelComponent));
            var component1 = componentRegistry.RegisterComponent(baseComponent.ComponentGuid, typeof(ChildComponent));
            var component2 = componentRegistry.RegisterComponent(component1.ComponentGuid, typeof(ChildComponent));
            var component3 = componentRegistry.RegisterComponent(component2.ComponentGuid, typeof(ChildComponent));
            var component4 = componentRegistry.RegisterComponent(component3.ComponentGuid, typeof(ChildComponent));
            var component5 = componentRegistry.RegisterComponent(component4.ComponentGuid, typeof(ChildComponent));

            // Act
            componentRegistry.UnregisterComponent(component5.ComponentGuid);

            // Assert
            Assert.Empty(component4.ChildComponents);
        }
    }
}