# Changelog

All notable changes to the **XrmGhost.Attributes** library will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/), and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## Versioning Strategy

This library is published to **[NuGet.org](https://www.nuget.org/packages/XrmGhost.Attributes)** as the canonical public distribution channel.

**Publication trigger:** The publish workflow runs automatically when a Git tag matching the pattern `v*.*.*` is pushed to the repository (e.g., `git tag v1.0.0 && git push origin v1.0.0`).

**Version source:** The version number is extracted from the first `[x.y.z]` entry in this `CHANGELOG.md` file and applied during the release build. The pushed tag and the CHANGELOG version must match.

**First public release:** The first tag pushed to nuget.org was `v1.0.0`, marking the first stable public release of this package.

## [Unreleased]

## [1.1.0] - 2026-08-01

### Added

- **Filtering attributes on `PluginExecutionConfigAttribute`.** New optional `public string[]? FilteringAttributes { get; set; }` property declaring the columns whose change should trigger an `Update` step.
  - `null` and an empty array are **equivalent** and both mean "no filtering attributes". Downstream registrars must map both to a Dataverse `filteringattributes` value of `null`, never an empty string. Existing consumers are unaffected: this is a purely additive, backward-compatible change.
  - Filtering attributes are meaningful **only for the `Update` message**. Downstream registrars must ignore the property for any other message declared on the same configuration.
  - The attribute stores values **verbatim** (source order, casing, and duplicates preserved) and performs no normalization. Consumers must canonicalize before comparing against or writing to Dataverse: trim and drop blanks, lowercase invariant, de-duplicate, sort ordinally, comma-join without spaces, and emit `null` for an empty result. See `README.md` and the XML docs on the property for the normative rule.
- Repo-local net8.0 test coverage for the new contract, including reflection-level assertions on the metadata actually baked into the assembly (default `null`, empty array, mixed-message declarations, and order/casing preservation).

### Changed

- Aligned `VersionPrefix`/`AssemblyVersion`/`FileVersion` in `XrmGhost.Attributes.csproj` to `1.1.0`. These values are informational for local builds only: `.github/workflows/publish.yml` overrides all of them via `/p:` at release time, using the version parsed from the first `[x.y.z]` entry of this file.
- CI (`.github/workflows/ci.yml`) now installs the .NET 8 SDK alongside .NET 9 and no longer masks test failures with `continue-on-error: true`. The test project added in `1.0.0` is now genuinely gating.

### Release handoff

- **Publication is not automatic on merge.** After this branch is merged, someone must run `git tag v1.1.0 && git push origin v1.1.0` to trigger the publish workflow and push the package to NuGet.org. The tag must match the version of the top `[x.y.z]` entry above or the workflow fails its verification step.
- **Downstream consumers must wait for the released package.** `xrmghost-dataverse` cannot consume `FilteringAttributes` until the `1.1.0` NuGet package is published by the tag step above.
- **Public documentation is out of scope for this repo.** The attribute reference page on `docs.xrmghost.tech` lives in `xrmghost-docs` and still needs a separate follow-up task to document `FilteringAttributes`.

## [1.0.4] - 2026-07-07

### Changed

- Added README backlinks to `www.xrmghost.tech`, the base `xrmghost` repository, and the `xrmghost-skills` and `xrmghost-docs` sibling repositories.

## [1.0.3] - 2026-07-07

### Changed

- Corrected `PackageProjectUrl` to point to `https://docs.xrmghost.tech/attributes/`.
- Refreshed `README.md` with differentiated first-party backlinks to distinguish the dual repo/NuGet surface.

## [1.0.2] - 2026-05-07

### Changed

- Removed internal swarm notes and the strong-name setup guide from the public repository.
- Kept the cleanup scoped to repository documentation only, with no source or workflow behavior changes.

## [1.0.1] - 2026-05-07

### Fixed

- Updated the NuGet publish workflow to improve reliability for tag-triggered releases.
- Removed the tag self-mutation behavior from the publish workflow and made package discovery/signing validation deterministic for release runs.

## [1.0.0] - 2026-05-05

### Added

- **First public release of XrmGhost.Attributes.** This release marks the first stable public API published to NuGet.org.
- Consolidates all attributes developed during the pre-public phase: `PluginExecutionConfigAttribute`, `InputParameterAttribute`, `OutputParameterAttribute`, `SharedVariableAttribute`, `PreImageAttribute`, `PostImageAttribute`, `UnsecureConfigurationAttribute`, `SecureConfigurationAttribute`, `HandlesMessageAttribute`, `SolutionComponentAttribute`.
- Strong name signing enabled for the assembly.
- Full xUnit smoke test coverage included.
- CI/CD pipeline configured for NuGet.org publication on tag push.

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
