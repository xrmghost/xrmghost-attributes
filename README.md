# XrmGhost.Attributes

**XrmGhost.Attributes** is a .NET library of custom attributes for declarative Dataverse plugin development. Decorate your plugin classes with these attributes to declare execution context, message handling, entity images, input/output parameters, and configuration — without writing plumbing code.

The package targets **netstandard2.0** and has no runtime dependencies beyond the BCL, so it can be referenced by any Dataverse plugin project.

- **Documentation:** [https://docs.xrmghost.tech](https://docs.xrmghost.tech)
- **Releases:** [GitHub Releases](https://github.com/xrmghost/xrmghost-attributes/releases)

---

## Table of Contents

1. [Standalone Use](#standalone-use)
2. [XrmGhost Ecosystem](#xrmghost-ecosystem)
3. [Installation](#installation)
4. [Included Attributes](#included-attributes)
5. [Usage Examples](#usage-examples)
6. [Contributing](#contributing)

---

## Standalone Use

You can reference **XrmGhost.Attributes** in any Dataverse plugin project — no other XrmGhost component is required.

The attributes are pure .NET attribute classes. Adding them to your plugin decorates the assembly with structured metadata that can be read at runtime or build time via reflection. On their own they impose **zero runtime overhead** inside the Dataverse sandbox: a `[PreImage]` or `[InputParameter]` attribute on a class does nothing unless something reads it.

Typical standalone uses:

- **Custom tooling.** Write a registration or scaffolding tool that reads attributes to automate plugin step registration.
- **Documentation generation.** Produce an audit of all messages, entities, and images a plugin assembly handles, without parsing code.
- **Test harnesses.** Read `[InputParameter]` and `[PreImage]` attributes to reconstruct a plausible `IPluginExecutionContext` in unit tests.

---

## XrmGhost Ecosystem

Within the XrmGhost ecosystem this package is consumed by two components:

### xrmghost-framework-host

`xrmghost-framework-host` is a thin hosting layer that wraps plugin execution. At runtime it reads the attributes on a plugin class via reflection and automatically populates the corresponding properties before calling `Execute`. This removes the standard boilerplate of extracting `Target`, pre/post images, and configuration strings from `IPluginExecutionContext`.

### xrmghost-cli — `generate-scenario`

The `generate-scenario` command of the XrmGhost CLI introspects a plugin assembly and generates test scenario files. It uses the attribute values (entity JSON, parameter JSON, image aliases) as the starting data for each scenario.

> **Note:** `generate-scenario` expects plugins to be decorated with XrmGhost.Attributes attributes. If the attributes are absent the CLI degrades gracefully with a warning, but no scenario data will be pre-populated.

---

## Installation

Install the package from NuGet:

```bash
dotnet add package XrmGhost.Attributes
```

Or add the reference directly to your `.csproj`:

```xml
<PackageReference Include="XrmGhost.Attributes" />
```

---

## Included Attributes

All attributes target **`AttributeTargets.Class`** — they must be placed on the plugin class, not on individual members.

| Attribute | Description |
|---|---|
| `PluginExecutionConfigAttribute` | Declares entity + messages the plugin handles; supports stage, mode, and impersonation. Repeatable. |
| `HandlesMessageAttribute` | Declares a single SDK message the plugin handles. Repeatable. |
| `InputParameterAttribute` | Declares an input parameter name and its default JSON value. Repeatable. |
| `OutputParameterAttribute` | Declares an expected output parameter name and its expected JSON value. Repeatable. |
| `PreImageAttribute` | Declares a pre-operation entity image alias and its JSON seed value. Repeatable. |
| `PostImageAttribute` | Declares a post-operation entity image alias and its JSON seed value. Repeatable. |
| `SecureConfigurationAttribute` | Declares the default secure configuration string. Single use. |
| `UnsecureConfigurationAttribute` | Declares the default unsecure configuration string. Single use. |
| `SharedVariableAttribute` | Declares a shared variable name and its default JSON value. Repeatable. |
| `SolutionComponentAttribute` | Declares which solution(s) the plugin should be registered in. Repeatable. |

---

## Usage Examples

All attributes live in the `XrmGhost.Attributes` namespace.

### Basic plugin — single entity, multiple messages

```csharp
using XrmGhost.Attributes;
using Microsoft.Xrm.Sdk;

[PluginExecutionConfigAttribute("account", "Create", "Update")]
[PreImageAttribute("primary", "{ \"__entityName\": \"account\", \"accountid\": \"00000000-0000-0000-0000-000000000001\", \"name\": \"Old Name\" }")]
[InputParameterAttribute("Target", "{ \"__entityName\": \"account\", \"accountid\": \"00000000-0000-0000-0000-000000000001\" }")]
public class AccountPlugin : IPlugin
{
    public Entity TargetEntity { get; set; }
    public Entity PreImageEntity { get; set; }

    public void Execute(IServiceProvider serviceProvider)
    {
        // When used with xrmghost-framework-host, TargetEntity and PreImageEntity
        // are populated automatically before Execute is called.
    }
}
```

### Multiple entities and messages

```csharp
using XrmGhost.Attributes;
using Microsoft.Xrm.Sdk;

[PluginExecutionConfigAttribute("account", "Create", "Update")]
[PluginExecutionConfigAttribute("contact", "Create")]
[PluginExecutionConfigAttribute("opportunity", "Update", "Delete")]
[InputParameterAttribute("Target", "{ \"__entityName\": \"account\", \"accountid\": \"00000000-0000-0000-0000-000000000001\" }")]
public class MultiEntityPlugin : IPlugin
{
    public Entity TargetEntity { get; set; }

    public void Execute(IServiceProvider serviceProvider)
    {
        // Registered for:
        //   Account   — Create, Update
        //   Contact   — Create
        //   Opportunity — Update, Delete
    }
}
```

### Column-filtered entity images

Filtering images to specific columns improves performance by reducing the payload transmitted to the plugin.

```csharp
using XrmGhost.Attributes;
using Microsoft.Xrm.Sdk;

[PluginExecutionConfigAttribute("account", "Update")]
[InputParameterAttribute("Target", "{ \"__entityName\": \"account\", \"accountid\": \"00000000-0000-0000-0000-000000000001\" }")]
[PreImageAttribute("primary", "{ \"__entityName\": \"account\", \"name\": \"Old Name\", \"revenue\": 1000.00 }",
    Attributes = new[] { "name", "accountid", "createdon", "revenue" })]
[PostImageAttribute("updated", "{ \"__entityName\": \"account\", \"name\": \"New Name\", \"revenue\": 2000.00 }",
    Attributes = new[] { "name", "modifiedon", "revenue" })]
public class AccountUpdatePlugin : IPlugin
{
    public Entity TargetEntity { get; set; }
    public Entity PreImageEntity { get; set; }
    public Entity PostImageEntity { get; set; }

    public void Execute(IServiceProvider serviceProvider) { }
}
```

### Stage, mode, and user impersonation

```csharp
using XrmGhost.Attributes;
using Microsoft.Xrm.Sdk;

[PluginExecutionConfigAttribute("lead", "Create", "Update",
    Stage = 20, Mode = 0,
    ImpersonatingUserId = "12345678-1234-1234-1234-123456789012")]
[SolutionComponentAttribute("LeadManagement")]
[InputParameterAttribute("Target", "{ \"__entityName\": \"lead\", \"leadid\": \"00000000-0000-0000-0000-000000000002\" }")]
public class LeadProcessorPlugin : IPlugin
{
    public Entity TargetEntity { get; set; }

    public void Execute(IServiceProvider serviceProvider) { }
}
```

`Stage` values: `10` PreValidation · `20` PreOperation · `40` PostOperation.  
`Mode` values: `0` Synchronous · `1` Asynchronous.

### Multiple solutions

```csharp
using XrmGhost.Attributes;
using Microsoft.Xrm.Sdk;

[SolutionComponentAttribute("CoreCRM", IsDefault = true)]
[SolutionComponentAttribute("SalesEnhancements")]
[PluginExecutionConfigAttribute("opportunity", "Create", "Update", "Delete")]
[InputParameterAttribute("Target", "{ \"__entityName\": \"opportunity\", \"opportunityid\": \"00000000-0000-0000-0000-000000000003\" }")]
public class OpportunityPlugin : IPlugin
{
    public Entity TargetEntity { get; set; }

    public void Execute(IServiceProvider serviceProvider) { }
}
```

### Input and output parameters with shared variables

```csharp
using XrmGhost.Attributes;
using Microsoft.Xrm.Sdk;

[PluginExecutionConfigAttribute("none", "CalculatePrice")]
[InputParameterAttribute("Quantity", "5")]
[InputParameterAttribute("UnitPrice", "19.99")]
[OutputParameterAttribute("TotalPrice", "99.95")]
[SharedVariableAttribute("PricingRuleVersion", "\"v2\"")]
public class CalculatePricePlugin : IPlugin
{
    public void Execute(IServiceProvider serviceProvider) { }
}
```

### Secure and unsecure configuration

```csharp
using XrmGhost.Attributes;
using Microsoft.Xrm.Sdk;

[PluginExecutionConfigAttribute("account", "Create")]
[SecureConfigurationAttribute("{ \"apiKey\": \"secret-value\" }")]
[UnsecureConfigurationAttribute("{ \"endpoint\": \"https://example.com/api\" }")]
public class ConfiguredPlugin : IPlugin
{
    public void Execute(IServiceProvider serviceProvider) { }
}
```

---

## Contributing

Contributions are welcome. Please refer to [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines.
