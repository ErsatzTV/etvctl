# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/).

## [Unreleased]
### Added
- Allow renaming smart collections using `rename:` and `from:`. For example, to rename `Something Better` to `The Best`:
```yaml
- name: The Best
  query: type:episode AND minutes:[1 TO 10]
  rename:
    from: Something Better
```
- Add `organization` options to `config.yml` to influence export behavior
```yaml
# defines how template files should be organized on disk
organization:
  # default for any resource type not listed in overrides
  # options: single_file, file_per_type, file_per_resource
  default: single_file
  overrides:
    smart_collection: file_per_type
```

### Changed
- Change `config.yml` `template` to take a folder instead of a file

## [0.0.1] - 2025-10-12
- Initial release; supports exporting, planning, and applying smart collections

[Unreleased]: https://github.com/ErsatzTV/etvctl/compare/v0.0.1...HEAD
[0.0.1-prealpha]: https://github.com/ErsatzTV/etvctl/releases/tag/v0.0.1
