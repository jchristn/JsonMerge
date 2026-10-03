# Change Log

## Current Version

v1.1.0

- Fix: duplicate property names are now rejected anywhere in either document.  Previously they were only detected in objects the merge touched, and were otherwise silently copied into the output with duplicates intact
- Fix: duplicate property names now throw `ArgumentException` whose `ParamName` is `inputJson` or `mergeJson` (previously `key`), with the original exception preserved as `InnerException`
- New `JsonMergeOptions` class, accepted by new `MergeJson` and `TryMergeJson` overloads (existing overloads are unchanged and produce identical output)
  - `MaxDepth` (default 64, range 1 to 1000): maximum nesting depth permitted when parsing
  - `NullRemovesProperty` (default false): a null merge value removes the property, and nulls inside newly added objects are omitted (JSON Merge Patch, RFC 7396, semantics)
  - `WriteIndented` (default false): indented output
  - `UseRelaxedEscaping` (default false): write non-ASCII and HTML-sensitive characters without `\uXXXX` escaping
- Input JSON is no longer parsed twice
- Test infrastructure migrated to Touchstone (`Test.Shared`, `Test.Automated`, `Test.Xunit`, `Test.Nunit`) with expanded positive and negative coverage

## Previous Versions

v1.0.1

- Retarget to include .NET 10

v1.0.0

- Initial release
