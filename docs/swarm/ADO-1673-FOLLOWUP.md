# ADO-1673-FOLLOWUP — Close missing edges in public discovery backlink mesh

## Goal

Close missing edges in the ADO-1669 public discovery backlink mesh on the
`xrmghost-attributes` dual-use (repo + NuGet) README: add first-party links to
`www.xrmghost.tech`, the base repo `github.com/xrmghost/xrmghost`, and sibling
repos `xrmghost-skills` and `xrmghost-docs`. These were in the Epic's target
adjacency matrix but out of scope for the original ADO-1679 task, which only
added a Documentation/Source/Releases link block.

## Scope and changes

- Edited `README.md` only.
- Added a new **"Part of the XrmGhost suite"** block directly after the
  existing Documentation/Source/Releases block (and its NuGet metadata-lag
  note), before the Table of Contents.
- New block links, with differentiated copy from the prior block:
  - **Website:** `https://www.xrmghost.tech`
  - **Suite home:** `https://github.com/xrmghost/xrmghost`
  - **Companion tooling:** `https://github.com/xrmghost/xrmghost-skills`
  - **Docs source:** `https://github.com/xrmghost/xrmghost-docs`
- Added a short note clarifying this new section, like the rest of the
  README, only reflects on the NuGet package page as of the next tagged
  version (`v1.0.4`), consistent with the existing package-metadata note
  already present from the prior cycle.
- All links are absolute (`https://...`); no repo-relative paths were
  introduced, so the README remains valid both as a GitHub README and as a
  NuGet package README.
- No other files touched: `XrmGhost.Attributes.csproj`, `CHANGELOG.md`, `.cs`
  files, and `.github/workflows/**` are untouched (version bump to v1.0.4 is
  handled separately, out of scope here).

## Commands executed

```
git diff -- README.md
rg -n 'https://' README.md
git status --porcelain
```

Output confirmed:
- Diff limited to `README.md`.
- All `https://` link occurrences are absolute URLs (no relative asset/link
  paths added).
- `git status --porcelain` showed only `README.md` as modified before commit.

## Acceptance criteria verified

- [x] README.md includes first-party links to www, base repo, skills repo,
      and docs repo.
- [x] Copy is differentiated from the existing Documentation/Source/Releases
      block (different section heading, framing as "Part of the XrmGhost
      suite" vs. per-package doc links) and from other surfaces' wording.
- [x] Dual GitHub/NuGet renderability preserved (absolute links only, no
      relative image/asset paths added).
- [x] Only `README.md` modified.

## Residual risks

- The new links will not be visible on the NuGet package page until v1.0.4
  is tagged and published — expected, handled in a separate release step per
  the task's stated risk, not part of this follow-up.

## Notes for review

- Section placed between the existing intro/metadata-note block and the
  Table of Contents, so it reads as a natural continuation of the
  "where to find things" info at the top of the README, without disrupting
  the TOC-driven document structure below it.
