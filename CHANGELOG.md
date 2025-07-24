# Changelog

All notable changes to the **GhostPlugin.Attributes** library will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/), and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0] - 2025-07-24

### Added

- **Initial Public Release:** This is the first version of the `GhostPlugin.Attributes` library as a standalone NuGet package. The code was extracted from the main GhostPlugin repository to improve modularity and dependency management.

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
