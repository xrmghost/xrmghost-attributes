# XrmGhost.Attributes

<p align="center">
  <picture>
    <source media="(prefers-color-scheme: dark)" srcset="./assets/XG_Audiowide_transparent_negative.svg">
    <source media="(prefers-color-scheme: light)" srcset="./assets/XG_Audiowide_transparent.svg">
    <img alt="Xrm Ghost" src="./assets/XG_Audiowide_transparent_negative.svg" width="300" />
  </picture>
</p>

**XrmGhost.Attributes** is a .NET library of custom attributes for declarative Dataverse plugin development. Decorate your plugin classes with these attributes to declare execution context, message handling, entity images, input/output parameters, and configuration — without writing plumbing code.

The package targets **netstandard2.0** and has no runtime dependencies beyond the BCL, so it can be referenced by any Dataverse plugin project.

- **Documentation:** [Attributes reference](https://docs.xrmghost.tech/attributes/)
- **Source:** [xrmghost-attributes on GitHub](https://github.com/xrmghost/xrmghost-attributes)
- **Releases:** [GitHub Releases](https://github.com/xrmghost/xrmghost-attributes/releases)

> **Package metadata note:** this package's NuGet listing points to the documentation above as of this revision. The corrected metadata is only observable on a newly published NuGet version — historical versions of `XrmGhost.Attributes` already on nuget.org cannot be changed retroactively.

**Part of the XrmGhost suite:**

- **Website:** [www.xrmghost.tech](https://www.xrmghost.tech) — product overview, pricing, and getting-started guides.
- **Suite home:** [xrmghost/xrmghost](https://github.com/xrmghost/xrmghost) — the umbrella repo tying together the CLI, hosts, and supporting libraries this package plugs into.
- **Companion tooling:** [xrmghost-skills](https://github.com/xrmghost/xrmghost-skills) — reusable automation skills built on top of the XrmGhost stack.
- **Docs source:** [xrmghost-docs](https://github.com/xrmghost/xrmghost-docs) — the repo behind the documentation site linked above.

> **Note on NuGet rendering:** this section is part of the same README rendered on both GitHub and the NuGet package page. On NuGet it only reflects the state as of the next published version (`v1.0.4`); the historical NuGet page for earlier versions is unaffected.

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

[PluginExecutionConfig("account", "Create", "Update")]
[PreImage("primary", "{ \"__entityName\": \"account\", \"accountid\": \"00000000-0000-0000-0000-000000000001\", \"name\": \"Old Name\" }")]
[InputParameter("Target", "{ \"__entityName\": \"account\", \"accountid\": \"00000000-0000-0000-0000-000000000001\" }")]
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

[PluginExecutionConfig("account", "Create", "Update")]
[PluginExecutionConfig("contact", "Create")]
[PluginExecutionConfig("opportunity", "Update", "Delete")]
[InputParameter("Target", "{ \"__entityName\": \"account\", \"accountid\": \"00000000-0000-0000-0000-000000000001\" }")]
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

[PluginExecutionConfig("account", "Update")]
[InputParameter("Target", "{ \"__entityName\": \"account\", \"accountid\": \"00000000-0000-0000-0000-000000000001\" }")]
[PreImage("primary", "{ \"__entityName\": \"account\", \"name\": \"Old Name\", \"revenue\": 1000.00 }",
    Attributes = new[] { "name", "accountid", "createdon", "revenue" })]
[PostImage("updated", "{ \"__entityName\": \"account\", \"name\": \"New Name\", \"revenue\": 2000.00 }",
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

[PluginExecutionConfig("lead", "Create", "Update",
    Stage = 20, Mode = 0,
    ImpersonatingUserId = "12345678-1234-1234-1234-123456789012")]
[SolutionComponent("LeadManagement")]
[InputParameter("Target", "{ \"__entityName\": \"lead\", \"leadid\": \"00000000-0000-0000-0000-000000000002\" }")]
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

[SolutionComponent("CoreCRM", IsDefault = true)]
[SolutionComponent("SalesEnhancements")]
[PluginExecutionConfig("opportunity", "Create", "Update", "Delete")]
[InputParameter("Target", "{ \"__entityName\": \"opportunity\", \"opportunityid\": \"00000000-0000-0000-0000-000000000003\" }")]
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

[PluginExecutionConfig("none", "CalculatePrice")]
[InputParameter("Quantity", "5")]
[InputParameter("UnitPrice", "19.99")]
[OutputParameter("TotalPrice", "99.95")]
[SharedVariable("PricingRuleVersion", "\"v2\"")]
public class CalculatePricePlugin : IPlugin
{
    public void Execute(IServiceProvider serviceProvider) { }
}
```

### Secure and unsecure configuration

```csharp
using XrmGhost.Attributes;
using Microsoft.Xrm.Sdk;

[PluginExecutionConfig("account", "Create")]
[SecureConfiguration("{ \"apiKey\": \"secret-value\" }")]
[UnsecureConfiguration("{ \"endpoint\": \"https://example.com/api\" }")]
public class ConfiguredPlugin : IPlugin
{
    public void Execute(IServiceProvider serviceProvider) { }
}
```

---

## Contributing

Contributions are welcome. Please refer to [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines.
