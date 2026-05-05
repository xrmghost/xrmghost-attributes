# ADO-1164 Review — Cycle 1

**Reviewer:** REVIEW_ALAB  
**Date:** 2026-05-05  
**Verdict:** rework  

---

## Checks Summary

| Check | Result |
|---|---|
| scope | pass |
| acceptance | fail |
| verification | pass |
| documentation | pass |

---

## Findings

### F-1

**id:** F-1  
**category:** correctness  
**severity:** high  
**location:** README.md lines 112-119, 140-141, 164-173, 191-192, 212-213  

**issue:**  
`InputParameterAttribute`, `PreImageAttribute`, `PostImageAttribute`, and `UnsecureConfigurationAttribute` all have `AttributeTargets.Class` (confirmed in their respective `.cs` source files). However, in multiple code examples in the README these attributes are applied to **properties** (not to the class), which would produce a compile-time error `CS0592: Attribute 'X' is not valid on this declaration type. It is only valid on 'class' declarations.`

Specific locations:
- Line 112: `[InputParameterAttribute(...)]` applied to property `TargetEntity`
- Line 115: `[PreImageAttribute(...)]` applied to property `PreImageEntity`
- Line 118: `[UnsecureConfigurationAttribute(...)]` applied to property `ConfigValue`
- Line 140: `[InputParameterAttribute(...)]` applied to property `TargetEntity`
- Line 164: `[InputParameterAttribute(...)]` applied to property `TargetEntity`
- Lines 167-168: `[PreImageAttribute(...)]` applied to property `PreImageEntity`
- Lines 171-172: `[PostImageAttribute(...)]` applied to property `PostImageEntity`
- Line 191: `[InputParameterAttribute(...)]` applied to property `TargetEntity`
- Line 212: `[InputParameterAttribute(...)]` applied to property `TargetEntity`

This directly violates the acceptance criterion: *"All code examples in README use constructor signatures matching the actual .cs source files"* — and more broadly the definition of done: *"All code snippets in README can be copy-pasted and would compile"*.

Note: `dotnet build XrmGhost.Attributes.csproj` passes because README.md is included as `PackageReadmeFile`, not compiled as C# code. The build test does not validate that the C# examples in the README are syntactically or semantically correct.

**required_action:**  
Move all property-level attribute usages up to the class level in each affected code example. The class-level application pattern is already correctly used in examples at lines 225-229 (Input/Output parameters with shared variables) and lines 242-244 (Secure/Unsecure config). Apply the same pattern consistently.

For the "Basic plugin" example:
```csharp
[PluginExecutionConfigAttribute("account", "Create", "Update")]
[PreImageAttribute("primary", "{ \"__entityName\": \"account\", ... }")]
[InputParameterAttribute("Target", "{ \"__entityName\": \"account\", ... }")]
[UnsecureConfigurationAttribute("{ \"featureFlag\": true }")]
public class AccountPlugin : IPlugin
{
    // Properties remain but without attribute decoration
    public Entity TargetEntity { get; set; }
    public Entity PreImageEntity { get; set; }
    public string ConfigValue { get; set; }

    public void Execute(IServiceProvider serviceProvider) { ... }
}
```

Apply the same fix to all other examples where these attributes appear on properties.

**acceptance_check:**  
After fix: every usage of `InputParameterAttribute`, `PreImageAttribute`, `PostImageAttribute`, `UnsecureConfigurationAttribute`, and `SecureConfigurationAttribute` in code examples must appear on the class (before `public class ...`), not on any property. Run: `grep -n '\[InputParameter\|\[PreImage\|\[PostImage\|\[UnsecureConfig\|\[SecureConfig' README.md` and verify all matches are on lines that precede a `public class` declaration (not a `public ... { get; set; }` line).

---

## Rework Commands

```bash
# Verify all attribute usages appear at class level (not property level) after fix
grep -n '\[InputParameter\|\[PreImage\|\[PostImage\|\[UnsecureConfig\|\[SecureConfig' README.md
# Each match should be immediately followed by (or near) a 'public class' line, not a property
```

---

## Passing Items (for reference)

- **Commit SHA** `e8448b7` exists and is valid. ✅
- **Scope**: Only `README.md` and `docs/swarm/ADO-1164.md` modified — no `.cs` or `.csproj` files touched. ✅
- **Constructor arg counts**: All `PreImageAttribute`, `PostImageAttribute`, `InputParameterAttribute` usages use 2 args; `UnsecureConfigurationAttribute` uses 1 arg. No single-arg violations remain. ✅
- **Standalone Use section**: Present at line 22. ✅
- **XrmGhost Ecosystem section**: Present at line 36 with framework-host and CLI subsections. ✅
- **CLI generate-scenario degradation note**: Present at line 48. ✅
- **Current version + NuGet source instructions**: Version `0.6.27` at line 7; GitHub Packages setup at lines 54-77. ✅
- **Stale footer removed**: No "generated as part of migration" text found. ✅
- **No internal references**: No ADO URLs, no internal infra found. ✅
- **dotnet build passes**: Confirmed live `Build succeeded. 0 Warning(s) 0 Error(s)`. ✅
- **docs/swarm/ADO-1164.md**: Exists and fully populated. ✅
- **Code in C# fenced blocks**: All examples use ` ```csharp ` fences. ✅
