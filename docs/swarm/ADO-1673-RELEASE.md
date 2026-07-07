# ADO-1673-RELEASE

## Goal

Bump `XrmGhost.Attributes` `CHANGELOG.md` to release the ADO-1673 follow-up
(README backlinks to www/base repo/siblings) as version `1.0.4`, since the
NuGet publish workflow extracts the tag version from the CHANGELOG's first
`[x.y.z]` entry, and the pushed tag must match it exactly.

## Scope and modifications

- Edited only `CHANGELOG.md`.
- Inserted a new `## [1.0.4] - 2026-07-07` section directly below the (empty)
  `## [Unreleased]` heading and above the existing `## [1.0.3] - 2026-07-07`
  entry.
- New entry documents the ADO-1673 follow-up change: README now links
  `www.xrmghost.tech`, the base `xrmghost` repo, and the `xrmghost-skills` /
  `xrmghost-docs` sibling repos.
- No other file was touched (README.md, csproj, workflows left untouched per
  `files_not_to_touch`).

## Commands executed

```
grep -n '^## \[' CHANGELOG.md | head -5
```
Output confirms `## [1.0.4] - 2026-07-07` is now the first bracketed version
heading below `## [Unreleased]`.

```
python3 -c "import re,pathlib; c=pathlib.Path('CHANGELOG.md').read_text(); m=re.search(r'\[(\d+\.\d+\.\d+)\]', c); assert m.group(1)=='1.0.4', m.group(1); print('version ok:', m.group(1))"
```
Output: `version ok: 1.0.4`

## Acceptance criteria verified

- [x] `CHANGELOG.md`'s first `[x.y.z]` entry is exactly `1.0.4`.
- [x] `## [Unreleased]` heading remains present (empty) above the new entry.
- [x] No other file modified (`git status` / `git diff --stat` show only
  `CHANGELOG.md` changed, plus this doc).

## Residual risks

- The eventual git tag pushed for release must be exactly `v1.0.4` to match
  the CHANGELOG version, otherwise the publish workflow's version-extraction
  regex/tag-match check will fail. Tag creation/push is out of scope for this
  task (CHANGELOG-only per `files_to_edit`).

## Notes for review

- Only `CHANGELOG.md` (plus this doc under `docs/swarm/`) was modified.
- `VersionPrefix`/`AssemblyVersion`/`FileVersion` in the csproj were left
  untouched as instructed.
