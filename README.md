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
- `PluginExecutionConfigAttribute`: A general-purpose attribute for configuring plugin execution behavior.
- `PreImageAttribute`: Specifies that a property should be populated with the Pre-operation entity image.
- `PostImageAttribute`: Specifies that a property should be populated with the Post-operation entity image.
- `SecureConfigurationAttribute`: Injects the secure configuration string into a property.
- `UnsecureConfigurationAttribute`: Injects the unsecure configuration string into a property.
- `SharedVariableAttribute`: Manages shared variables within the plugin execution context.

## Getting Started

To use this library, add the `XrmGhost.Attributes` NuGet package to your Dataverse plugin project.

```bash
dotnet add package XrmGhost.Attributes
```

### Example Usage

Here is a conceptual example of how you might use these attributes in a plugin class:

```csharp
using XrmGhost.Attributes;

[HandlesMessage("Update")]
[PreImage("primary")]
public class MyAwesomePlugin : IPlugin
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
```

## Contributing

This project is part of the larger XrmGhost ecosystem. For information on contributing, please refer to the main repository's contribution guidelines.

---
*This README was generated as part of the project's migration to a standalone NuGet package.*
