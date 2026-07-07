# ADO-1679-RELEASE — CHANGELOG bump to 1.0.3

## Goal
Bump `CHANGELOG.md` to release the ADO-1679 PackageProjectUrl/README metadata fix as version
`1.0.3`, since the NuGet publish workflow reads the tag version from the CHANGELOG's first
`[x.y.z]` entry and the pushed tag must match it exactly.

## Scope and modifications
- Edited `CHANGELOG.md` only.
- Moved the (empty) `## [Unreleased]` section content into a new `## [1.0.3] - 2026-07-07`
  entry, placed directly below `## [Unreleased]`, which remains present and empty above it.
- Documented the ADO-1679 change under a `### Changed` subsection:
  - `PackageProjectUrl` corrected to `https://docs.xrmghost.tech/attributes/`.
  - `README.md` refreshed with differentiated first-party backlinks for the dual repo/NuGet
    surface.
- No other file was touched (csproj, README.md, *.cs, workflows left untouched).

## Commands executed
```
grep -n '^## \[' CHANGELOG.md | head -5
# => 17:## [Unreleased]
#    19:## [1.0.3] - 2026-07-07
#    26:## [1.0.2] - 2026-05-07
#    33:## [1.0.1] - 2026-05-07
#    40:## [1.0.0] - 2026-05-05

python3 -c "import re,pathlib; c=pathlib.Path('CHANGELOG.md').read_text(); m=re.search(r'\[(\d+\.\d+\.\d+)\]', c); assert m.group(1)=='1.0.3', m.group(1); print('version ok:', m.group(1))"
# => version ok: 1.0.3
```

## Acceptance criteria verified
- CHANGELOG.md's first `[x.y.z]` entry is exactly `1.0.3`. Confirmed via regex test above.
- `## [Unreleased]` heading remains present (empty) above the new entry. Confirmed by grep
  output (line 17 `Unreleased`, line 19 `1.0.3`, nothing in between).
- No other file modified. Confirmed via `git status` / `git diff --stat` before commit
  (only `CHANGELOG.md` plus this doc changed).

## Residual risks
- The publish workflow itself was not re-run/tested in this task (out of scope); correctness
  relies on the version-extraction regex `\[(\d+\.\d+\.\d+)\]` matching the new heading, which
  was verified locally.
- The eventual git tag pushed for release must be exactly `v1.0.3` to match this CHANGELOG
  entry — tagging/publishing itself is out of scope for this task.

## Notes for review
- Only `CHANGELOG.md` was edited, per `files_to_edit` / `files_not_to_touch` constraints.
- VersionPrefix/AssemblyVersion/FileVersion in the csproj were left untouched as instructed.
