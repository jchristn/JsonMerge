namespace Test.Shared.Suites
{
    using System.Collections.Generic;
    using Touchstone.Core;
    using static Test.Shared.CaseFactory;

    /// <summary>
    /// Flat (top-level) merges: adding, overwriting, and every JSON value type.
    /// </summary>
    public static class BasicMergeSuite
    {
        private const string Id = "BasicMerge";

        /// <summary>
        /// Create the suite.
        /// </summary>
        /// <returns>Suite descriptor.</returns>
        public static TestSuiteDescriptor Create()
        {
            return new TestSuiteDescriptor(
                suiteId: Id,
                displayName: "Basic Merge",
                cases: new List<TestCaseDescriptor>
                {
                    Merge(Id, "AddNewProperty", "Add new property", "{\"a\":1}", "{\"b\":2}", "{\"a\":1,\"b\":2}"),
                    Merge(Id, "OverwriteExistingProperty", "Overwrite existing property", "{\"a\":1}", "{\"a\":2}", "{\"a\":2}"),
                    Merge(Id, "MultipleProperties", "Multiple properties merge", "{\"a\":1,\"b\":2}", "{\"c\":3,\"d\":4}", "{\"a\":1,\"b\":2,\"c\":3,\"d\":4}"),
                    Merge(Id, "MultiplePropertiesWithOverwrite", "Multiple properties with overwrite", "{\"a\":1,\"b\":2,\"c\":3}", "{\"b\":20,\"c\":30,\"d\":40}", "{\"a\":1,\"b\":20,\"c\":30,\"d\":40}"),
                    Merge(Id, "StringValues", "String values", "{\"name\":\"John\"}", "{\"surname\":\"Doe\"}", "{\"name\":\"John\",\"surname\":\"Doe\"}"),
                    Merge(Id, "BooleanValues", "Boolean values", "{\"enabled\":true}", "{\"visible\":false}", "{\"enabled\":true,\"visible\":false}"),
                    Merge(Id, "NullValues", "Null values", "{\"a\":1}", "{\"b\":null}", "{\"a\":1,\"b\":null}"),
                    Merge(Id, "MixedDataTypes", "Mixed data types", "{\"num\":42,\"str\":\"hello\",\"bool\":true}", "{\"nil\":null,\"float\":3.14}", "{\"num\":42,\"str\":\"hello\",\"bool\":true,\"nil\":null,\"float\":3.14}"),
                    Merge(Id, "EmptyMergeObject", "Empty merge object leaves input unchanged", "{\"a\":1,\"b\":2}", "{}", "{\"a\":1,\"b\":2}"),
                    Merge(Id, "EmptyInputObject", "Empty input object takes all merge properties", "{}", "{\"a\":1,\"b\":{\"c\":2}}", "{\"a\":1,\"b\":{\"c\":2}}"),
                    Merge(Id, "BothEmpty", "Both objects empty", "{}", "{}", "{}"),
                    Merge(Id, "LargeObject", "Large object merge", "{\"a\":1,\"b\":2,\"c\":3,\"d\":4,\"e\":5}", "{\"f\":6,\"g\":7,\"h\":8,\"i\":9,\"j\":10}", "{\"a\":1,\"b\":2,\"c\":3,\"d\":4,\"e\":5,\"f\":6,\"g\":7,\"h\":8,\"i\":9,\"j\":10}"),
                    Merge(Id, "UnicodeAndSpecialCharacters", "Unicode and special characters", "{\"emoji\":\"😀\"}", "{\"chinese\":\"中文\"}", "{\"emoji\":\"😀\",\"chinese\":\"中文\"}"),
                    Merge(Id, "NumericEdgeCases", "Numeric edge cases", "{\"zero\":0,\"negative\":-42}", "{\"large\":999999999,\"decimal\":0.000001}", "{\"zero\":0,\"negative\":-42,\"large\":999999999,\"decimal\":0.000001}"),
                    Merge(Id, "OverwriteDifferentTypes", "Overwrite number with string", "{\"value\":123}", "{\"value\":\"string\"}", "{\"value\":\"string\"}"),
                    Merge(Id, "OverwriteStringWithBoolean", "Overwrite string with boolean", "{\"value\":\"yes\"}", "{\"value\":true}", "{\"value\":true}"),
                    Merge(Id, "OverwriteValueWithNull", "Overwrite value with null", "{\"value\":123}", "{\"value\":null}", "{\"value\":null}"),
                    Merge(Id, "OverwriteNullWithValue", "Overwrite null with value", "{\"value\":null}", "{\"value\":123}", "{\"value\":123}"),
                    Merge(Id, "WhitespaceInStrings", "Whitespace in strings", "{\"text\":\"hello world\"}", "{\"tab\":\"hello\\tworld\"}", "{\"text\":\"hello world\",\"tab\":\"hello\\tworld\"}"),
                    Merge(Id, "EscapeSequences", "Escape sequences in strings", "{\"a\":\"line1\\nline2\"}", "{\"b\":\"quote\\\"and\\\\slash\"}", "{\"a\":\"line1\\nline2\",\"b\":\"quote\\\"and\\\\slash\"}"),
                    Merge(Id, "EmptyStrings", "Empty strings", "{\"empty\":\"\"}", "{\"also\":\"\"}", "{\"empty\":\"\",\"also\":\"\"}"),
                    Merge(Id, "PropertyNamesWithSpecialChars", "Property names with special chars", "{\"normal\":1}", "{\"with-dash\":2,\"with.dot\":3,\"with_underscore\":4}", "{\"normal\":1,\"with-dash\":2,\"with.dot\":3,\"with_underscore\":4}"),
                    Merge(Id, "EmptyPropertyName", "Empty property name is a valid, overwritable key", "{\"\":1}", "{\"\":2}", "{\"\":2}"),
                    Merge(Id, "EscapedPropertyNameMatches", "Escaped property name matches its unescaped form", "{\"a\\u0062\":1}", "{\"ab\":2}", "{\"ab\":2}"),
                    Merge(Id, "ScientificNotation", "Scientific notation", "{\"small\":1e-10}", "{\"large\":1e10}", "{\"small\":1e-10,\"large\":1e10}"),
                    Merge(Id, "CaseSensitiveKeys", "Keys are case-sensitive", "{\"A\":1}", "{\"a\":2}", "{\"A\":1,\"a\":2}")
                });
        }
    }
}
