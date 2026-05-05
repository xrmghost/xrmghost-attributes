# Changelog

All notable changes to the **XrmGhost.Attributes** library will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/), and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## Versioning Strategy

This library is published to **[NuGet.org](https://www.nuget.org/packages/XrmGhost.Attributes)** as the canonical public distribution channel.

**Publication trigger:** The publish workflow runs automatically when a Git tag matching the pattern `v*.*.*` is pushed to the repository (e.g., `git tag v1.0.0 && git push origin v1.0.0`).

**Version source:** The version number is extracted from the first `[x.y.z]` entry in this `CHANGELOG.md` file and applied to the `.csproj` before packing. The pushed tag and the CHANGELOG version must match.

**First public release:** The first tag pushed to nuget.org will be `v1.0.0`. Prior versions (0.6.25, 0.6.26, 0.6.27) were published only to GitHub Packages and are considered pre-public internal releases. Versioning restarts from `1.0.0` for the first public NuGet.org release to signal a stable, intentional public API.

**Prerequisite:** The `NUGET_API_KEY` secret must be configured in the GitHub repository settings before pushing the first `v*.*.*` tag.

## [Unreleased]

## [0.6.27] - 2025-09-04

### Added

- **Strong Name Signing:** Implemented strong name signing for the XrmGhost.Attributes assembly to eliminate CS8002 warnings when consumers reference the package. The assembly is now signed during the GitHub Actions build process using secure key management, ensuring assembly integrity and compatibility with strong naming policies.
- **Column Filtering for Entity Images:** Added optional `Attributes` property to both `PreImageAttribute` and `PostImageAttribute` to support column filtering during image registration. This enables better performance and supports automated registration tools that need to specify which columns to include in the entity images.
- **User Impersonation Support:** Added `ImpersonatingUserId` property to `PluginExecutionConfigAttribute` to specify under which user the plugin should be registered. Essential for automated registration tools to configure proper security context.
- **Solution Component Management:** Added new `SolutionComponentAttribute` to specify which solution(s) plugin components should be added to during automated registration. Includes `IsDefault` flag for primary solution identification and supports multiple solutions per plugin.

### Enhanced

- **Improved Documentation:** Enhanced XML documentation for `PreImageAttribute`, `PostImageAttribute`, and `PluginExecutionConfigAttribute` with detailed examples and usage scenarios.
- **README Examples:** Added comprehensive examples showcasing all new features including column filtering, user impersonation, and multi-solution deployment scenarios.

### Developer Experience

- All enhancements maintain full backward compatibility with existing code.
- New properties are optional/nullable, ensuring no breaking changes.
- Enhanced IntelliSense support with detailed XML documentation and code examples.

## [0.6.26] - 2025-08-04

- **PluginExecutionConfigAttribute Enhancement:** Updated `PluginExecutionConfigAttribute` to support multiple registrations on the same class by setting `AllowMultiple = true`. This enables plugins to be registered for multiple entities with different configurations.
- **Message Handling Support:** Extended `PluginExecutionConfigAttribute` to accept an optional `Messages` parameter that allows specifying multiple SDK messages (e.g., "Create", "Update") for a single entity registration. The messages parameter is optional - if not specified, the configuration applies to all messages for the entity.
- **Constructor Changes:** Modified the constructor to require `primaryEntityName` as a mandatory parameter and accept optional `messages` as a params array, improving the attribute's usability and clarity.

### Breaking Changes

- **Constructor Signature:** The `PluginExecutionConfigAttribute` constructor now requires a `primaryEntityName` parameter and optionally accepts message names, replacing the previous parameterless constructor with property setters.

## [0.6.25] - 2025-07-24

### Added

- **Initial Public Release:** This is the first version of the `XrmGhost.Attributes` library as a standalone NuGet package. The code was extracted from the main XrmGhost repository to improve modularity and dependency management.

### Feature History (Pre-Packaging)

This version consolidates the features developed prior to being packaged separately.

- **Initial Feature Set (from commit `9b30e25` on 2025-05-23):**
    - The library was first created as a .NET Standard 2.0 project.
    - The initial set of attributes was defined to aid the `generate-scenario` CLI command:
        - `PluginExecutionConfigAttribute`
        - `InputParameterAttribute`
        - `OutputParameterAttribute`
        - `SharedVariableAttribute`
        - `PreImageAttribute`
        - `PostImageAttribute`
        - `UnsecureConfigurationAttribute`
        - `SecureConfigurationAttribute`

- **Multi-Message Support (from commit `63b2eab` on 2025-05-23):**
    - Introduced `HandlesMessageAttribute` to allow a single plugin class to declare support for multiple SDK messages (e.g., "Create", "Update"). This attribute can be used multiple times on a single class.
    - This enhancement allows the `generate-scenario` command to create distinct test scenarios for each message a plugin handles.
