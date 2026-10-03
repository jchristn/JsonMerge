namespace Test.Shared.Suites
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json;
    using System.Threading.Tasks;
    using JsonMerge;
    using Touchstone.Core;
    using static Test.Shared.CaseFactory;

    /// <summary>
    /// JsonMergeOptions: validation, depth, null removal, indentation, and escaping.
    /// </summary>
    public static class OptionsSuite
    {
        private const string Id = "Options";

        /// <summary>
        /// Create the suite.
        /// </summary>
        /// <returns>Suite descriptor.</returns>
        public static TestSuiteDescriptor Create()
        {
            return new TestSuiteDescriptor(
                suiteId: Id,
                displayName: "Options",
                cases: new List<TestCaseDescriptor>
                {
                    // defaults and validation
                    Case(Id, "Defaults", "Default option values", () =>
                    {
                        JsonMergeOptions options = new JsonMergeOptions();
                        Check.Equal(64, options.MaxDepth, "MaxDepth");
                        Check.Equal(JsonMergeOptions.DefaultMaxDepth, options.MaxDepth, "DefaultMaxDepth");
                        Check.False(options.NullRemovesProperty, "NullRemovesProperty");
                        Check.False(options.WriteIndented, "WriteIndented");
                        Check.False(options.UseRelaxedEscaping, "UseRelaxedEscaping");
                    }),
                    Case(Id, "NullOptionsMatchesDefaults", "Null options produce the same output as the parameterless overload", () =>
                    {
                        string input = "{\"a\":{\"b\":\"中\"},\"c\":1}";
                        string merge = "{\"a\":{\"d\":null},\"c\":null}";
                        string expected = JsonMerger.MergeJson(input, merge);
                        Check.Equal(expected, JsonMerger.MergeJson(input, merge, null), "MergeJson(null options)");
                        Check.Equal(expected, JsonMerger.MergeJson(input, merge, new JsonMergeOptions()), "MergeJson(default options)");
                        Check.True(JsonMerger.TryMergeJson(input, merge, null, out string result), "TryMergeJson(null options)");
                        Check.Equal(expected, result, "TryMergeJson(null options) result");
                    }),
                    Case(Id, "MaxDepthZeroRejected", "MaxDepth of 0 is rejected", () => MaxDepthRejected(0)),
                    Case(Id, "MaxDepthNegativeRejected", "Negative MaxDepth is rejected", () => MaxDepthRejected(-1)),
                    Case(Id, "MaxDepthAboveLimitRejected", "MaxDepth above 1000 is rejected", () => MaxDepthRejected(JsonMergeOptions.MaximumMaxDepth + 1)),
                    Case(Id, "MaxDepthBoundsAccepted", "MaxDepth of 1 and 1000 are accepted", () =>
                    {
                        JsonMergeOptions options = new JsonMergeOptions();
                        options.MaxDepth = 1;
                        Check.Equal(1, options.MaxDepth, "MaxDepth = 1");
                        options.MaxDepth = JsonMergeOptions.MaximumMaxDepth;
                        Check.Equal(1000, options.MaxDepth, "MaxDepth = 1000");
                    }),
                    Case(Id, "RejectedMaxDepthLeavesValueUnchanged", "A rejected MaxDepth does not change the current value", () =>
                    {
                        JsonMergeOptions options = new JsonMergeOptions { MaxDepth = 10 };
                        Check.ThrowsExactly<ArgumentOutOfRangeException>(() => options.MaxDepth = 0, "MaxDepth = 0");
                        Check.Equal(10, options.MaxDepth, "MaxDepth");
                    }),

                    // depth
                    Case(Id, "RaisedMaxDepthAllowsDeeper", "Raised MaxDepth allows documents deeper than 64", () =>
                    {
                        JsonMergeOptions options = new JsonMergeOptions { MaxDepth = 100 };
                        string input = Json.Nested(99, "{\"a\":1}");
                        string merge = Json.Nested(99, "{\"b\":2}");
                        Check.JsonEqualDeep(Json.Nested(99, "{\"a\":1,\"b\":2}"), JsonMerger.MergeJson(input, merge, options), 100, "MergeJson");
                        Check.Throws<JsonException>(() => JsonMerger.MergeJson(input, merge), "MergeJson with default options");
                    }),
                    Case(Id, "MaximumMaxDepthAllowsDeepest", "MaxDepth of 1000 allows a 1000-level document", () =>
                    {
                        JsonMergeOptions options = new JsonMergeOptions { MaxDepth = JsonMergeOptions.MaximumMaxDepth };
                        string input = Json.Nested(999, "{\"a\":1}");
                        string merge = Json.Nested(999, "{\"b\":2}");
                        Check.True(JsonMerger.TryMergeJson(input, merge, options, out string result), "TryMergeJson");
                        Check.JsonEqualDeep(Json.Nested(999, "{\"a\":1,\"b\":2}"), result, 1000, "result");
                        VerifyTryFails(Json.Nested(1001), "{}", options);
                    }),
                    Case(Id, "LoweredMaxDepthRejectsInput", "Lowered MaxDepth rejects a deeper inputJson", () =>
                    {
                        JsonMergeOptions options = new JsonMergeOptions { MaxDepth = 2 };
                        Check.JsonEqual("{\"a\":{\"b\":1,\"c\":2}}", JsonMerger.MergeJson("{\"a\":{\"b\":1}}", "{\"a\":{\"c\":2}}", options), "depth 2 accepted");
                        Check.Throws<JsonException>(() => JsonMerger.MergeJson("{\"a\":{\"b\":{}}}", "{}", options), "depth 3 inputJson");
                        VerifyTryFails("{\"a\":{\"b\":{}}}", "{}", options);
                    }),
                    Case(Id, "LoweredMaxDepthRejectsMerge", "Lowered MaxDepth rejects a deeper mergeJson", () =>
                    {
                        JsonMergeOptions options = new JsonMergeOptions { MaxDepth = 1 };
                        Check.Equal("{\"a\":1,\"b\":2}", JsonMerger.MergeJson("{\"a\":1}", "{\"b\":2}", options), "depth 1 accepted");
                        Check.Throws<JsonException>(() => JsonMerger.MergeJson("{\"a\":1}", "{\"b\":[]}", options), "depth 2 mergeJson");
                        VerifyTryFails("{\"a\":1}", "{\"b\":[]}", options);
                    }),

                    // null removal
                    MergeWith(Id, "NullRemovesTopLevel", "NullRemovesProperty removes a top-level property", NullRemoves(), "{\"a\":1,\"b\":2}", "{\"a\":null}", "{\"b\":2}"),
                    MergeWith(Id, "NullRemovesNested", "NullRemovesProperty removes a nested property", NullRemoves(), "{\"a\":{\"x\":1,\"y\":2},\"b\":2}", "{\"a\":{\"x\":null}}", "{\"a\":{\"y\":2},\"b\":2}"),
                    MergeWith(Id, "NullRemovesObject", "NullRemovesProperty removes an entire nested object", NullRemoves(), "{\"a\":{\"x\":1},\"b\":2}", "{\"a\":null}", "{\"b\":2}"),
                    MergeWith(Id, "NullRemovesAbsentKey", "NullRemovesProperty ignores a key that is absent", NullRemoves(), "{\"a\":1}", "{\"z\":null}", "{\"a\":1}"),
                    MergeWith(Id, "NullRemovesNullValue", "NullRemovesProperty removes an existing null value", NullRemoves(), "{\"a\":null,\"b\":1}", "{\"a\":null}", "{\"b\":1}"),
                    MergeWith(Id, "NullRemovesLeavesEmptyObject", "Removing every nested property leaves an empty object", NullRemoves(), "{\"a\":{\"x\":1}}", "{\"a\":{\"x\":null}}", "{\"a\":{}}"),
                    MergeWith(Id, "NullOmittedInAddedObject", "Nulls inside a newly added object are omitted", NullRemoves(), "{}", "{\"a\":{\"x\":null,\"y\":1,\"z\":{\"w\":null}}}", "{\"a\":{\"y\":1,\"z\":{}}}"),
                    MergeWith(Id, "NullOmittedInReplacingObject", "Nulls inside an object replacing a primitive are omitted", NullRemoves(), "{\"a\":5}", "{\"a\":{\"x\":null,\"y\":1}}", "{\"a\":{\"y\":1}}"),
                    MergeWith(Id, "NullKeptInArrays", "Nulls inside arrays are kept", NullRemoves(), "{}", "{\"a\":[null,{\"x\":null}]}", "{\"a\":[null,{\"x\":null}]}"),
                    MergeWith(Id, "NullRemovesMixed", "Null removal combined with additions and overwrites", NullRemoves(), "{\"keep\":1,\"drop\":2,\"o\":{\"k\":1,\"d\":2}}", "{\"drop\":null,\"add\":3,\"o\":{\"d\":null,\"k\":9}}", "{\"keep\":1,\"o\":{\"k\":9},\"add\":3}"),
                    MergeWith(Id, "NullKeptByDefault", "Without NullRemovesProperty, null is written as a value", new JsonMergeOptions(), "{\"a\":1,\"b\":2}", "{\"a\":null,\"c\":{\"x\":null}}", "{\"a\":null,\"b\":2,\"c\":{\"x\":null}}"),

                    // output formatting
                    Case(Id, "WriteIndented", "WriteIndented produces indented output", () =>
                    {
                        string nl = Environment.NewLine;
                        string expected = "{" + nl + "  \"a\": 1," + nl + "  \"b\": {" + nl + "    \"c\": [" + nl + "      1" + nl + "    ]" + nl + "  }" + nl + "}";
                        Check.Equal(expected, JsonMerger.MergeJson("{\"a\":1}", "{\"b\":{\"c\":[1]}}", new JsonMergeOptions { WriteIndented = true }), "MergeJson");
                    }),
                    Case(Id, "RelaxedEscaping", "UseRelaxedEscaping writes BMP non-ASCII and HTML-sensitive characters as-is", () =>
                    {
                        string result = JsonMerger.MergeJson("{\"a\":\"中文\"}", "{\"b\":\"<&>'+\",\"e\":\"😀\"}", new JsonMergeOptions { UseRelaxedEscaping = true });

                        // characters outside the Basic Multilingual Plane (such as emoji) remain escaped as surrogate pairs
                        Check.Equal("{\"a\":\"中文\",\"b\":\"<&>'+\",\"e\":\"\\uD83D\\uDE00\"}", result, "MergeJson");
                    }),
                    Case(Id, "RelaxedEscapingStillEscapesRequired", "UseRelaxedEscaping still escapes quotes, backslashes, and control characters", () =>
                    {
                        string result = JsonMerger.MergeJson("{}", "{\"a\":\"q\\\"b\\\\n\\n\\u0001\"}", new JsonMergeOptions { UseRelaxedEscaping = true });
                        Check.Equal("{\"a\":\"q\\\"b\\\\n\\n\\u0001\"}", result, "MergeJson");
                    }),
                    Case(Id, "IndentedAndRelaxed", "WriteIndented and UseRelaxedEscaping combined", () =>
                    {
                        string nl = Environment.NewLine;
                        string result = JsonMerger.MergeJson("{\"a\":\"中\"}", "{\"b\":\"<\"}", new JsonMergeOptions { WriteIndented = true, UseRelaxedEscaping = true });
                        Check.Equal("{" + nl + "  \"a\": \"中\"," + nl + "  \"b\": \"<\"" + nl + "}", result, "MergeJson");
                    }),
                    Case(Id, "AllOptionsCombined", "All options combined", () =>
                    {
                        JsonMergeOptions options = new JsonMergeOptions { MaxDepth = 3, NullRemovesProperty = true, WriteIndented = true, UseRelaxedEscaping = true };
                        string result = JsonMerger.MergeJson("{\"a\":{\"b\":{\"c\":1}},\"d\":\"x\"}", "{\"a\":{\"b\":{\"e\":\"é\"}},\"d\":null}", options);
                        Check.JsonEqual("{\"a\":{\"b\":{\"c\":1,\"e\":\"é\"}}}", result, "result");
                        Check.Contains(result, "é", "unescaped character");
                        Check.Contains(result, Environment.NewLine, "indentation");
                        Check.DoesNotContain(result, "\"d\"", "removed property");
                    }),
                    Case(Id, "OptionsDoNotAffectValidation", "Options do not relax argument validation", () =>
                    {
                        JsonMergeOptions options = new JsonMergeOptions { NullRemovesProperty = true, UseRelaxedEscaping = true };
                        Check.ThrowsExactly<ArgumentNullException>(() => JsonMerger.MergeJson(null!, "{}", options), "null inputJson");
                        Check.ThrowsExactly<ArgumentException>(() => JsonMerger.MergeJson("{}", "[]", options), "array mergeJson");
                        Check.ThrowsExactly<ArgumentException>(() => JsonMerger.MergeJson("{\"a\":{\"b\":1,\"b\":2}}", "{}", options), "duplicate keys");
                        Check.Throws<JsonException>(() => JsonMerger.MergeJson("{invalid}", "{}", options), "malformed");
                        VerifyTryFails(null, "{}", options);
                        VerifyTryFails("{}", "{invalid}", options);
                    }),
                    CaseAsync(Id, "SharedOptionsConcurrent", "A shared options instance is safe across concurrent merges", async ct =>
                    {
                        JsonMergeOptions options = new JsonMergeOptions { NullRemovesProperty = true, UseRelaxedEscaping = true };
                        List<Task> tasks = new List<Task>();
                        for (int t = 0; t < 8; t++)
                        {
                            int worker = t;
                            tasks.Add(Task.Run(() =>
                            {
                                for (int i = 0; i < 200; i++)
                                {
                                    ct.ThrowIfCancellationRequested();
                                    string result = JsonMerger.MergeJson("{\"w\":0,\"drop\":1,\"s\":\"é\"}", "{\"w\":" + worker + ",\"drop\":null,\"i\":" + i + "}", options);
                                    Check.Equal("{\"w\":" + worker + ",\"s\":\"é\",\"i\":" + i + "}", result, "worker " + worker + " iteration " + i);
                                }
                            }, ct));
                        }

                        await Task.WhenAll(tasks).ConfigureAwait(false);
                    })
                });
        }

        private static JsonMergeOptions NullRemoves()
        {
            return new JsonMergeOptions { NullRemovesProperty = true };
        }

        private static void MaxDepthRejected(int value)
        {
            JsonMergeOptions options = new JsonMergeOptions();
            ArgumentOutOfRangeException e = Check.ThrowsExactly<ArgumentOutOfRangeException>(() => options.MaxDepth = value, "MaxDepth = " + value);
            Check.Equal("MaxDepth", e.ParamName, "ParamName");
        }
    }
}
