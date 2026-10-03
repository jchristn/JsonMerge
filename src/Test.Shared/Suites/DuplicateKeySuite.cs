namespace Test.Shared.Suites
{
    using System;
    using System.Collections.Generic;
    using JsonMerge;
    using Touchstone.Core;
    using static Test.Shared.CaseFactory;

    /// <summary>
    /// Duplicate property names are rejected wherever they appear, not only in objects the merge touches.
    /// </summary>
    public static class DuplicateKeySuite
    {
        private const string Id = "DuplicateKey";

        /// <summary>
        /// Create the suite.
        /// </summary>
        /// <returns>Suite descriptor.</returns>
        public static TestSuiteDescriptor Create()
        {
            return new TestSuiteDescriptor(
                suiteId: Id,
                displayName: "Duplicate Keys",
                cases: new List<TestCaseDescriptor>
                {
                    Duplicate(Id, "TopLevelInput", "Top-level duplicate in inputJson", "{\"a\":1,\"a\":2}", "{\"b\":1}", "inputJson"),
                    Duplicate(Id, "TopLevelMerge", "Top-level duplicate in mergeJson", "{\"b\":1}", "{\"a\":1,\"a\":2}", "mergeJson"),
                    Duplicate(Id, "NestedInputUntouched", "Nested duplicate in inputJson that the merge does not touch", "{\"a\":{\"b\":1,\"b\":2}}", "{\"c\":1}", "inputJson"),
                    Duplicate(Id, "NestedInputTouched", "Nested duplicate in inputJson that the merge touches", "{\"a\":{\"b\":1,\"b\":2}}", "{\"a\":{\"c\":1}}", "inputJson"),
                    Duplicate(Id, "NestedMergeAdded", "Nested duplicate in a new object from mergeJson", "{\"c\":1}", "{\"a\":{\"b\":1,\"b\":2}}", "mergeJson"),
                    Duplicate(Id, "NestedMergeReplacing", "Nested duplicate in an object from mergeJson replacing a primitive", "{\"a\":1}", "{\"a\":{\"b\":1,\"b\":2}}", "mergeJson"),
                    Duplicate(Id, "InsideArrayInput", "Duplicate in an object inside an array in inputJson", "{\"a\":[{\"b\":1,\"b\":2}]}", "{\"c\":1}", "inputJson"),
                    Duplicate(Id, "InsideNestedArrayMerge", "Duplicate in an object inside nested arrays in mergeJson", "{}", "{\"a\":[[1,{\"b\":1,\"b\":2}]]}", "mergeJson"),
                    Duplicate(Id, "DeepInput", "Duplicate ten levels deep in inputJson", Json.Nested(10, "{\"k\":1,\"k\":2}"), "{\"y\":1}", "inputJson"),
                    Duplicate(Id, "EscapedDuplicate", "Escaped and unescaped spellings of the same name are duplicates", "{\"ab\":1,\"a\\u0062\":2}", "{}", "inputJson"),
                    Duplicate(Id, "BothDuplicateReportsInput", "Duplicates in both documents report inputJson first", "{\"a\":1,\"a\":2}", "{\"b\":1,\"b\":2}", "inputJson"),

                    Merge(Id, "SameNameDifferentObjects", "Same name in sibling objects is not a duplicate", "{\"a\":{\"x\":1},\"b\":{\"x\":2}}", "{\"c\":{\"x\":3}}", "{\"a\":{\"x\":1},\"b\":{\"x\":2},\"c\":{\"x\":3}}"),
                    Merge(Id, "SameNameInArrayElements", "Same name in separate array elements is not a duplicate", "{\"a\":[{\"x\":1},{\"x\":2}]}", "{}", "{\"a\":[{\"x\":1},{\"x\":2}]}"),
                    Merge(Id, "DifferentCaseNotDuplicate", "Names differing only by case are not duplicates", "{\"a\":1,\"A\":2}", "{}", "{\"a\":1,\"A\":2}"),

                    Case(Id, "InnerExceptionPreserved", "Original exception is preserved as the inner exception", () =>
                    {
                        ArgumentException e = Check.ThrowsExactly<ArgumentException>(
                            () => JsonMerger.MergeJson("{\"a\":{\"b\":1,\"b\":2}}", "{}"), "MergeJson");
                        Check.NotNull(e.InnerException, "InnerException");
                        Check.Contains(e.Message, "duplicate", "message");
                    })
                });
        }

        private static TestCaseDescriptor Duplicate(string suiteId, string caseId, string displayName, string inputJson, string mergeJson, string paramName)
        {
            return Case(suiteId, caseId, displayName, () =>
            {
                ArgumentException e = Check.ThrowsExactly<ArgumentException>(
                    () => JsonMerger.MergeJson(inputJson, mergeJson), "MergeJson");
                Check.Equal(paramName, e.ParamName, "ParamName");
                VerifyTryFails(inputJson, mergeJson);
            });
        }
    }
}
