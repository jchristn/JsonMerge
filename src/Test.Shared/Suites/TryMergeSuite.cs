namespace Test.Shared.Suites
{
    using System.Collections.Generic;
    using JsonMerge;
    using Touchstone.Core;
    using static Test.Shared.CaseFactory;

    /// <summary>
    /// TryMergeJson success and failure semantics.
    /// </summary>
    public static class TryMergeSuite
    {
        private const string Id = "TryMerge";

        /// <summary>
        /// Create the suite.
        /// </summary>
        /// <returns>Suite descriptor.</returns>
        public static TestSuiteDescriptor Create()
        {
            return new TestSuiteDescriptor(
                suiteId: Id,
                displayName: "TryMergeJson",
                cases: new List<TestCaseDescriptor>
                {
                    Case(Id, "BasicSuccess", "Basic success", () =>
                    {
                        Check.True(JsonMerger.TryMergeJson("{\"a\":1}", "{\"b\":2}", out string result), "TryMergeJson returns true");
                        Check.JsonEqual("{\"a\":1,\"b\":2}", result, "result");
                    }),
                    Case(Id, "NestedSuccess", "Nested success", () =>
                    {
                        Check.True(JsonMerger.TryMergeJson("{\"a\":{\"x\":1}}", "{\"a\":{\"y\":2}}", out string result), "TryMergeJson returns true");
                        Check.JsonEqual("{\"a\":{\"x\":1,\"y\":2}}", result, "result");
                    }),
                    Case(Id, "EmptyObjectsSuccess", "Empty objects succeed", () =>
                    {
                        Check.True(JsonMerger.TryMergeJson("{}", "{}", out string result), "TryMergeJson returns true");
                        Check.Equal("{}", result, "result");
                    }),

                    Case(Id, "NullInput", "Null inputJson", () => VerifyTryFails(null, "{}")),
                    Case(Id, "EmptyInput", "Empty inputJson", () => VerifyTryFails("", "{}")),
                    Case(Id, "NullMerge", "Null mergeJson", () => VerifyTryFails("{}", null)),
                    Case(Id, "EmptyMerge", "Empty mergeJson", () => VerifyTryFails("{}", "")),
                    Case(Id, "BothNull", "Both null", () => VerifyTryFails(null, null)),
                    Case(Id, "InvalidInput", "Invalid inputJson", () => VerifyTryFails("{invalid}", "{}")),
                    Case(Id, "InvalidMerge", "Invalid mergeJson", () => VerifyTryFails("{}", "{invalid}")),
                    Case(Id, "ArrayInput", "Array as inputJson", () => VerifyTryFails("[1,2,3]", "{}")),
                    Case(Id, "ArrayMerge", "Array as mergeJson", () => VerifyTryFails("{}", "[1,2,3]")),
                    Case(Id, "PrimitiveInput", "Primitive as inputJson", () => VerifyTryFails("123", "{}")),
                    Case(Id, "PrimitiveMerge", "Primitive as mergeJson", () => VerifyTryFails("{}", "123")),
                    Case(Id, "NullLiteralInput", "JSON null literal as inputJson", () => VerifyTryFails("null", "{}")),
                    Case(Id, "WhitespaceOnlyInput", "Whitespace-only inputJson", () => VerifyTryFails("   ", "{}")),
                    Case(Id, "DuplicateKeyInput", "Duplicate keys in inputJson", () => VerifyTryFails("{\"a\":1,\"a\":2}", "{\"b\":1}")),
                    Case(Id, "ExcessiveDepth", "Input deeper than the parser limit", () => VerifyTryFails(Json.Nested(65), "{}")),

                    Case(Id, "ResultResetOnFailure", "Out result is reset to null after a prior success", () =>
                    {
                        Check.True(JsonMerger.TryMergeJson("{}", "{\"a\":1}", out string result), "first call succeeds");
                        Check.NotNull(result, "first result");
                        Check.False(JsonMerger.TryMergeJson("{}", "[]", out result), "second call fails");
                        Check.Null(result, "second result");
                    })
                });
        }
    }
}
