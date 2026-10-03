namespace Test.Shared.Suites
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json;
    using System.Text.Json.Nodes;
    using JsonMerge;
    using Touchstone.Core;
    using static Test.Shared.CaseFactory;

    /// <summary>
    /// Nesting depth, size, and duplicate-key limits inherited from System.Text.Json.
    /// </summary>
    public static class LimitsSuite
    {
        private const string Id = "Limits";

        /// <summary>
        /// Deepest object nesting System.Text.Json parses by default.
        /// </summary>
        private const int MaxDepth = 64;

        /// <summary>
        /// Create the suite.
        /// </summary>
        /// <returns>Suite descriptor.</returns>
        public static TestSuiteDescriptor Create()
        {
            return new TestSuiteDescriptor(
                suiteId: Id,
                displayName: "Limits",
                cases: new List<TestCaseDescriptor>
                {
                    Case(Id, "MaxDepthMerge", "Recursive merge at the maximum supported depth", () =>
                    {
                        // MaxDepth - 1 wrapper objects plus the leaf object = MaxDepth objects
                        string inputLeaf = Json.Nested(MaxDepth - 1, "{\"a\":1}");
                        string mergeLeaf = Json.Nested(MaxDepth - 1, "{\"b\":2}");
                        string expected = Json.Nested(MaxDepth - 1, "{\"a\":1,\"b\":2}");

                        string result = JsonMerger.MergeJson(inputLeaf, mergeLeaf);
                        Check.JsonEqual(expected, result, "deep merge result");
                    }),
                    Case(Id, "InputBeyondMaxDepth", "inputJson deeper than the maximum depth throws JsonException", () =>
                    {
                        Check.Throws<JsonException>(() => JsonMerger.MergeJson(Json.Nested(MaxDepth + 1), "{}"), "MergeJson");
                    }),
                    Case(Id, "MergeBeyondMaxDepth", "mergeJson deeper than the maximum depth throws JsonException", () =>
                    {
                        Check.Throws<JsonException>(() => JsonMerger.MergeJson("{}", Json.Nested(MaxDepth + 1)), "MergeJson");
                        VerifyTryFails("{}", Json.Nested(MaxDepth + 1));
                    }),
                    Case(Id, "ManyProperties", "Merge of objects with thousands of properties", () =>
                    {
                        const int count = 5000;
                        string input = Json.Flat("k", count);
                        string merge = Json.Flat("k", count, 1000000);
                        string extra = Json.Flat("n", count);

                        JsonObject result = JsonNode.Parse(JsonMerger.MergeJson(input, merge))!.AsObject();
                        Check.Equal(count, result.Count, "property count after overwrite");
                        Check.Equal(1000000, result["k0"]!.GetValue<int>(), "k0 overwritten");
                        Check.Equal(count - 1 + 1000000, result["k" + (count - 1)]!.GetValue<int>(), "last key overwritten");

                        result = JsonNode.Parse(JsonMerger.MergeJson(input, extra))!.AsObject();
                        Check.Equal(count * 2, result.Count, "property count after add");
                    }),
                    Case(Id, "LargeStringValue", "Large string values are copied intact", () =>
                    {
                        string big = new string('z', 1024 * 1024);
                        string result = JsonMerger.MergeJson("{\"a\":1}", "{\"big\":\"" + big + "\"}");
                        Check.Equal(big, JsonNode.Parse(result)!["big"]!.GetValue<string>(), "big value");
                    }),
                    Case(Id, "DuplicateKeyInput", "Duplicate keys in inputJson are rejected with ArgumentException", () =>
                    {
                        ArgumentException e = Check.ThrowsExactly<ArgumentException>(() => JsonMerger.MergeJson("{\"a\":1,\"a\":2}", "{\"b\":1}"), "MergeJson");
                        Check.Equal("inputJson", e.ParamName, "ParamName");
                        VerifyTryFails("{\"a\":1,\"a\":2}", "{\"b\":1}");
                    }),
                    Case(Id, "DuplicateKeyMerge", "Duplicate keys in mergeJson are rejected with ArgumentException", () =>
                    {
                        ArgumentException e = Check.ThrowsExactly<ArgumentException>(() => JsonMerger.MergeJson("{\"b\":1}", "{\"a\":1,\"a\":2}"), "MergeJson");
                        Check.Equal("mergeJson", e.ParamName, "ParamName");
                        VerifyTryFails("{\"b\":1}", "{\"a\":1,\"a\":2}");
                    })
                });
        }
    }
}
