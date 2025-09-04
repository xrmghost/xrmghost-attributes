# XrmGhost.Attributes

**XrmGhost.Attributes** is a dedicated .NET library providing a set of custom attributes designed to streamline and simplify the development of plugins for Microsoft Dataverse. This package allows developers to use a declarative, attribute-based approach to define plugin configurations, message handling, and data context, reducing boilerplate code and improving maintainability.

This library is a core component of the XrmGhost ecosystem and is now delivered as a standalone NuGet package to facilitate isolated and version-controlled development.

## Overview

In traditional Dataverse plugin development, developers often write significant boilerplate code to handle input/output parameters, retrieve pre/post entity images, and manage secure/unsecure configurations. This library replaces that imperative code with a clean, declarative syntax.

By decorating your plugin classes and properties with these attributes, you can clearly define the plugin's execution context, making the code more readable, self-documenting, and easier to manage.

## Key Features

- **Declarative Syntax:** Define plugin behavior with simple .NET attributes instead of complex code.
- **Simplified Parameter Handling:** Automatically map input and output parameters to class properties using `[InputParameter]` and `[OutputParameter]`.
- **Easy Image Retrieval:** Access Pre and Post entity images effortlessly with `[PreImage]` and `[PostImage]` attributes.
- **Configuration Management:** Seamlessly inject secure and unsecure configurations via `[SecureConfiguration]` and `[UnsecureConfiguration]`.
- **Message Specificity:** Clearly indicate which message your plugin handles with the `[HandlesMessage]` attribute.
- **Reduced Boilerplate:** Focus on your business logic, not on the repetitive plumbing of plugin infrastructure.

## Included Attributes

This package provides the following attributes:

- `HandlesMessageAttribute`: Specifies the SDK message that the plugin will handle (e.g., "Create", "Update").
- `InputParameterAttribute`: Maps a plugin's input parameter to a class property.
- `OutputParameterAttribute`: Maps a plugin's output parameter to a class property.
- `PluginExecutionConfigAttribute`: A general-purpose attribute for configuring plugin execution behavior, including user impersonation.
- `PreImageAttribute`: Specifies that a property should be populated with the Pre-operation entity image, with optional column filtering.
- `PostImageAttribute`: Specifies that a property should be populated with the Post-operation entity image, with optional column filtering.
- `SecureConfigurationAttribute`: Injects the secure configuration string into a property.
- `UnsecureConfigurationAttribute`: Injects the unsecure configuration string into a property.
- `SharedVariableAttribute`: Manages shared variables within the plugin execution context.
- `SolutionComponentAttribute`: Specifies which solution(s) the plugin component should be added to during automated registration.

## Getting Started

To use this library, add the `XrmGhost.Attributes` NuGet package to your Dataverse plugin project.

```bash
dotnet add package XrmGhost.Attributes
```

### Example Usage

Here is a conceptual example of how you might use these attributes in a plugin class:

```csharp
using XrmGhost.Attributes;

// Example 1: Single entity with multiple messages
[PluginExecutionConfigAttribute("account", "Create", "Update")]
[PreImage("primary")]
public class AccountPlugin : IPlugin
{
    [InputParameter("Target")]
    public Entity TargetEntity { get; set; }

    [PreImage("primary")]
    public Entity PreImageEntity { get; set; }

    [UnsecureConfiguration]
    public string MyConfigValue { get; set; }

    public void Execute(IServiceProvider serviceProvider)
    {
        // The XrmGhost framework will automatically populate the properties
        // decorated with attributes before this method is called.

        // Your business logic here...
        // You can now directly use TargetEntity, PreImageEntity, and MyConfigValue.
    }
}

// Example 2: Multiple entities with different configurations
[PluginExecutionConfigAttribute("account", "Create", "Update")]
[PluginExecutionConfigAttribute("contact", "Create")]
[PluginExecutionConfigAttribute("opportunity", "Update", "Delete")]
public class MultiEntityPlugin : IPlugin
{
    [InputParameter("Target")]
    public Entity TargetEntity { get; set; }

    public void Execute(IServiceProvider serviceProvider)
    {
        // This plugin will be registered for:
        // - Account: Create and Update messages
        // - Contact: Create message only
        // - Opportunity: Update and Delete messages
    }
}

// Example 3: Entity registration without specific messages (applies to all messages)
[PluginExecutionConfigAttribute("account")]
public class AllMessagesPlugin : IPlugin
{
    [InputParameter("Target")]
    public Entity TargetEntity { get; set; }

    public void Execute(IServiceProvider serviceProvider)
    {
        // This plugin will be registered for all messages on the account entity
    }
}
```

### Enhanced Features Examples

```csharp
using XrmGhost.Attributes;

// Example 4: Column filtering for entity images
[PluginExecutionConfigAttribute("account", "Update")]
[SolutionComponent("CoreBusinessLogic", IsDefault = true)]
[SolutionComponent("ExtendedFeatures")]
public class AccountUpdatePlugin : IPlugin
{
    [InputParameter("Target")]
    public Entity TargetEntity { get; set; }

    // Pre-image with specific columns only for better performance
    [PreImage("primary", "{...json...}", Attributes = new[] { "name", "accountid", "createdon", "revenue" })]
    public Entity PreImageEntity { get; set; }

    // Post-image with filtered columns
    [PostImage("updated", "{...json...}", Attributes = new[] { "name", "modifiedon", "revenue" })]
    public Entity PostImageEntity { get; set; }

    public void Execute(IServiceProvider serviceProvider)
    {
        // Business logic with optimized image retrieval
    }
}

// Example 5: User impersonation for plugin registration
[PluginExecutionConfigAttribute("lead", "Create", "Update", 
    Stage = 20, Mode = 0, 
    ImpersonatingUserId = "12345678-1234-1234-1234-123456789012")]
[SolutionComponent("LeadManagement")]
public class LeadProcessorPlugin : IPlugin
{
    [InputParameter("Target")]
    public Entity TargetEntity { get; set; }

    public void Execute(IServiceProvider serviceProvider)
    {
        // This plugin will run under the specified user context
        // regardless of who triggers the operation
    }
}

// Example 6: Multiple solutions with default priority
[SolutionComponent("CoreCRM", IsDefault = true)]  // Primary solution
[SolutionComponent("SalesEnhancements")]          // Additional solution
[SolutionComponent("TestEnvironment")]            // Test deployment
[PluginExecutionConfigAttribute("opportunity", "Create", "Update", "Delete")]
public class OpportunityPlugin : IPlugin
{
    [InputParameter("Target")]
    public Entity TargetEntity { get; set; }

    public void Execute(IServiceProvider serviceProvider)
    {
        // Plugin will be added to multiple solutions
        // with CoreCRM being the primary/default solution
    }
}

// Example 7: Complex multi-entity plugin with all enhancements
[PluginExecutionConfigAttribute("account", "Create", "Update", Stage = 20, Mode = 0)]
[PluginExecutionConfigAttribute("contact", "Create", Stage = 20, Mode = 0, 
    ImpersonatingUserId = "87654321-4321-4321-4321-210987654321")]
[SolutionComponent("CustomerManagement", IsDefault = true)]
[SolutionComponent("DataIntegration")]
public class CustomerDataPlugin : IPlugin
{
    [InputParameter("Target")]
    public Entity TargetEntity { get; set; }

    [PreImage("primary", "{...json...}", 
        Attributes = new[] { "name", "emailaddress1", "telephone1", "createdon" })]
    public Entity PreImageEntity { get; set; }

    [UnsecureConfiguration]
    public string ConfigValue { get; set; }

    public void Execute(IServiceProvider serviceProvider)
    {
        // Comprehensive plugin with:
        // - Multi-entity registration
        // - Column-filtered images
        // - User impersonation for contact operations
        // - Multi-solution deployment
    }
}
```

## Contributing

This project is part of the larger XrmGhost ecosystem. For information on contributing, please refer to the main repository's contribution guidelines.

---
*This README was generated as part of the project's migration to a standalone NuGet package.*
