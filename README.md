<p align="center">
  <img src="assets/icon.png" alt="JsonMerge Logo" width="192" height="192">
</p>

<h1 align="center">JsonMerge</h1>

<p align="center">
  <strong>Simply and elegantly merge values into an existing JSON object</strong>
</p>

<p align="center">
  <a href="https://www.nuget.org/packages/JsonMerge"><img src="https://img.shields.io/nuget/v/JsonMerge?style=flat-square&logo=nuget&label=NuGet" alt="NuGet"></a>
  <a href="https://www.nuget.org/packages/JsonMerge"><img src="https://img.shields.io/nuget/dt/JsonMerge?style=flat-square&logo=nuget&label=Downloads" alt="NuGet Downloads"></a>
  <a href="https://github.com/jchristn/jsonmerge"><img src="https://img.shields.io/github/license/jchristn/jsonmerge?style=flat-square&label=License" alt="License"></a>
  <a href="https://github.com/jchristn/jsonmerge/stargazers"><img src="https://img.shields.io/github/stars/jchristn/jsonmerge?style=flat-square&logo=github&label=Stars" alt="GitHub Stars"></a>
</p>

---

## Overview

JsonMerge is a lightweight, high-performance .NET library for recursively merging JSON objects. It provides a simple API to combine JSON structures while intelligently handling nested objects, with merge values taking precedence over existing values.

Perfect for configuration management, API response merging, template systems, and any scenario where you need to overlay JSON data structures.

## Features

- **Recursive Deep Merge**: Automatically merges nested JSON objects at any depth
- **Type-Safe**: Leverages `System.Text.Json` for robust JSON parsing and manipulation
- **Flexible API**: Choose between exception-throwing or Try pattern methods
- **Smart Overwriting**: Merge values override existing values when keys match
- **Type Preservation**: Handles all JSON types (strings, numbers, booleans, null, objects, arrays)
- **Multi-Framework**: Supports .NET 8.0, .NET Standard 2.0, and .NET Standard 2.1

## Installation

Install via NuGet Package Manager:

```bash
dotnet add package JsonMerge
```

Or via Package Manager Console:

```powershell
Install-Package JsonMerge
```

Or add directly to your `.csproj` file:

```xml
<PackageReference Include="JsonMerge" Version="1.0.0" />
```

## Quick Start

```csharp
using JsonMerge;

// Basic merge - add new properties
string input = "{\"name\":\"John\",\"age\":30}";
string merge = "{\"city\":\"New York\",\"active\":true}";
string result = JsonMerger.MergeJson(input, merge);
// Result: {"name":"John","age":30,"city":"New York","active":true}

// Overwrite existing properties
string input2 = "{\"name\":\"John\",\"age\":30}";
string merge2 = "{\"age\":35,\"role\":\"admin\"}";
string result2 = JsonMerger.MergeJson(input2, merge2);
// Result: {"name":"John","age":35,"role":"admin"}

// Recursive merge of nested objects
string input3 = "{\"user\":{\"name\":\"John\",\"email\":\"john@example.com\"}}";
string merge3 = "{\"user\":{\"age\":30,\"verified\":true}}";
string result3 = JsonMerger.MergeJson(input3, merge3);
// Result: {"user":{"name":"John","email":"john@example.com","age":30,"verified":true}}
```

## API Reference

### `MergeJson(string inputJson, string mergeJson)`

Merges two JSON objects, with merge values overwriting input values for matching keys.

**Parameters:**
- `inputJson` - The base JSON object (required, non-null, non-empty)
- `mergeJson` - The JSON object to merge into the input (required, non-null, non-empty)

**Returns:** A JSON string containing the merged result

**Throws:**
- `ArgumentNullException` - If either parameter is null or empty
- `ArgumentException` - If either parameter is not a valid JSON object
- `JsonException` - If JSON parsing fails

### `TryMergeJson(string inputJson, string mergeJson, out string result)`

Attempts to merge JSON objects with safe error handling.

**Parameters:**
- `inputJson` - The base JSON object
- `mergeJson` - The JSON object to merge
- `result` - Output parameter containing the merged JSON string, or null if merge fails

**Returns:** `true` if merge succeeded, `false` otherwise

## Use Cases

### Configuration Management

```csharp
// Merge default config with user overrides
string defaultConfig = "{\"timeout\":30,\"retries\":3,\"logging\":{\"level\":\"info\"}}";
string userConfig = "{\"timeout\":60,\"logging\":{\"level\":\"debug\",\"verbose\":true}}";
string finalConfig = JsonMerger.MergeJson(defaultConfig, userConfig);
// Result: {"timeout":60,"retries":3,"logging":{"level":"debug","verbose":true}}
```

### API Response Merging

```csharp
// Combine partial API responses
string baseResponse = "{\"user\":{\"id\":123,\"name\":\"Alice\"}}";
string additionalData = "{\"user\":{\"email\":\"alice@example.com\",\"role\":\"admin\"},\"timestamp\":\"2025-01-15T10:30:00Z\"}";
string combined = JsonMerger.MergeJson(baseResponse, additionalData);
```

### Template Systems

```csharp
// Apply template overrides
string template = "{\"layout\":\"grid\",\"theme\":\"light\",\"settings\":{\"sidebar\":true}}";
string customizations = "{\"theme\":\"dark\",\"settings\":{\"sidebar\":false,\"animations\":true}}";
string applied = JsonMerger.MergeJson(template, customizations);
```

### Safe Merging with Error Handling

```csharp
string input = LoadFromDatabase();
string patch = GetUserInput();

if (JsonMerger.TryMergeJson(input, patch, out string result))
{
    Console.WriteLine($"Merge successful: {result}");
}
else
{
    Console.WriteLine("Merge failed - invalid JSON or incompatible types");
}
```

## Behavior Guide

### What Will Succeed ✓

- **Adding new properties**: New keys from merge JSON are added to result
- **Overwriting values**: Matching keys get overwritten with merge values
- **Nested object merging**: Objects are recursively merged at all depths
- **Type changes**: A property can change type (e.g., number → string)
- **All JSON types**: Strings, numbers, booleans, null, objects, and arrays
- **Deep structures**: Unlimited nesting depth supported
- **Special characters**: Unicode, whitespace, and escape sequences

### What Will Fail ✗

- **Null or empty inputs**: Both parameters must be non-null and non-empty
- **Invalid JSON syntax**: Malformed JSON throws exceptions
- **Array inputs**: Root elements must be objects `{}`, not arrays `[]`
- **Primitive inputs**: Root elements must be objects, not primitives (`123`, `"string"`, etc.)

### Important Constraints

#### Arrays are Replaced, Not Merged

Arrays are treated as atomic values and are replaced entirely:

```csharp
string input = "{\"tags\":[\"javascript\",\"nodejs\"]}";
string merge = "{\"tags\":[\"python\",\"django\"]}";
string result = JsonMerger.MergeJson(input, merge);
// Result: {"tags":["python","django"]} - NOT ["javascript","nodejs","python","django"]
```

#### Empty Objects

Merging with an empty object leaves the input unchanged:

```csharp
string input = "{\"a\":1,\"b\":2}";
string merge = "{}";
string result = JsonMerger.MergeJson(input, merge);
// Result: {"a":1,"b":2}
```

#### Type Replacement

When a value's type changes, the new type completely replaces the old:

```csharp
// Object replaced by primitive
string input = "{\"data\":{\"nested\":true}}";
string merge = "{\"data\":\"simple string\"}";
string result = JsonMerger.MergeJson(input, merge);
// Result: {"data":"simple string"}

// Primitive replaced by object
string input2 = "{\"data\":123}";
string merge2 = "{\"data\":{\"complex\":true}}";
string result2 = JsonMerger.MergeJson(input2, merge2);
// Result: {"data":{"complex":true}}
```

## Performance Characteristics

- **Time Complexity**: O(n + m) where n is the size of input and m is the size of merge
- **Space Complexity**: O(n + m) for the result object
- **Deep Cloning**: Values are deep cloned to prevent reference issues
- **Zero Allocations**: Minimal allocations beyond necessary result structures

## Framework Support

| Target Framework | Version |
|------------------|---------|
| .NET             | 8.0+    |
| .NET Standard    | 2.0+    |
| .NET Standard    | 2.1+    |

## Contributing

Contributions are welcome! Please see [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines.

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details.

## Attribution

Icon designed by [VeryIcon](https://www.veryicon.com/icons/miscellaneous/standard-general-linear-icon/merge-5.html).

## Links

- **NuGet Package**: https://nuget.org/packages/JsonMerge
- **GitHub Repository**: https://github.com/jchristn/jsonmerge
- **Issue Tracker**: https://github.com/jchristn/jsonmerge/issues

---

<p align="center">
  Made with ❤️ by <a href="https://github.com/jchristn">Joel Christner</a>
</p>
