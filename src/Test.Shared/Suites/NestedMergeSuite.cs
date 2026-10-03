namespace Test.Shared.Suites
{
    using System.Collections.Generic;
    using Touchstone.Core;
    using static Test.Shared.CaseFactory;

    /// <summary>
    /// Recursive merging of nested objects and replacement when types differ.
    /// </summary>
    public static class NestedMergeSuite
    {
        private const string Id = "NestedMerge";

        /// <summary>
        /// Create the suite.
        /// </summary>
        /// <returns>Suite descriptor.</returns>
        public static TestSuiteDescriptor Create()
        {
            return new TestSuiteDescriptor(
                suiteId: Id,
                displayName: "Nested Merge",
                cases: new List<TestCaseDescriptor>
                {
                    Merge(Id, "RecursiveMerge", "Nested objects - recursive merge", "{\"user\":{\"name\":\"John\"}}", "{\"user\":{\"age\":30}}", "{\"user\":{\"name\":\"John\",\"age\":30}}"),
                    Merge(Id, "ComplexNestedStructure", "Complex nested structure", "{\"level1\":{\"level2\":{\"value\":1}},\"other\":\"data\"}", "{\"level1\":{\"level2\":{\"value\":2}},\"new\":\"field\"}", "{\"level1\":{\"level2\":{\"value\":2}},\"other\":\"data\",\"new\":\"field\"}"),
                    Merge(Id, "DeepMultipleLevels", "Deep nested merge - multiple levels", "{\"a\":{\"b\":{\"c\":{\"d\":1}}}}", "{\"a\":{\"b\":{\"c\":{\"e\":2}}}}", "{\"a\":{\"b\":{\"c\":{\"d\":1,\"e\":2}}}}"),
                    Merge(Id, "NestedSiblingProperties", "Nested object with sibling properties", "{\"config\":{\"timeout\":30,\"retries\":3}}", "{\"config\":{\"retries\":5,\"verbose\":true}}", "{\"config\":{\"timeout\":30,\"retries\":5,\"verbose\":true}}"),
                    Merge(Id, "MixedNestedAndFlat", "Mixed nested and flat properties", "{\"user\":{\"id\":1},\"active\":true}", "{\"user\":{\"name\":\"joe\"},\"role\":\"admin\"}", "{\"user\":{\"id\":1,\"name\":\"joe\"},\"active\":true,\"role\":\"admin\"}"),
                    Merge(Id, "ObjectReplacedByPrimitive", "Nested object replaced by primitive", "{\"data\":{\"value\":1}}", "{\"data\":\"string\"}", "{\"data\":\"string\"}"),
                    Merge(Id, "PrimitiveReplacedByObject", "Primitive replaced by nested object", "{\"data\":\"string\"}", "{\"data\":{\"value\":1}}", "{\"data\":{\"value\":1}}"),
                    Merge(Id, "ThreeLevelMerge", "Three-level nested merge", "{\"app\":{\"settings\":{\"ui\":{\"theme\":\"dark\"}}}}", "{\"app\":{\"settings\":{\"ui\":{\"lang\":\"en\"},\"cache\":true}}}", "{\"app\":{\"settings\":{\"ui\":{\"theme\":\"dark\",\"lang\":\"en\"},\"cache\":true}}}"),
                    Merge(Id, "ObjectReplacedByNull", "Nested object replaced by null (null does not delete the key)", "{\"a\":{\"b\":1},\"c\":2}", "{\"a\":null}", "{\"a\":null,\"c\":2}"),
                    Merge(Id, "NullReplacedByObject", "Null replaced by nested object", "{\"a\":null}", "{\"a\":{\"b\":1}}", "{\"a\":{\"b\":1}}"),
                    Merge(Id, "ObjectReplacedByArray", "Nested object replaced by array", "{\"a\":{\"b\":1}}", "{\"a\":[1,2]}", "{\"a\":[1,2]}"),
                    Merge(Id, "ArrayReplacedByObject", "Array replaced by nested object", "{\"a\":[1,2]}", "{\"a\":{\"b\":1}}", "{\"a\":{\"b\":1}}"),
                    Merge(Id, "EmptyNestedObjectPreservesTarget", "Empty nested merge object preserves target", "{\"a\":{\"b\":1,\"c\":2}}", "{\"a\":{}}", "{\"a\":{\"b\":1,\"c\":2}}"),
                    Merge(Id, "NestedIntoEmptyObject", "Nested properties merged into empty target object", "{\"a\":{}}", "{\"a\":{\"b\":1}}", "{\"a\":{\"b\":1}}"),
                    Merge(Id, "NewNestedObjectAdded", "New nested object added when key is absent", "{\"x\":1}", "{\"a\":{\"b\":{\"c\":1}}}", "{\"x\":1,\"a\":{\"b\":{\"c\":1}}}"),
                    Merge(Id, "SameKeyAtDifferentDepths", "Same key name at different depths merges independently", "{\"id\":1,\"child\":{\"id\":2}}", "{\"child\":{\"id\":3}}", "{\"id\":1,\"child\":{\"id\":3}}"),
                    Merge(Id, "SiblingSubtreesUntouched", "Untouched sibling subtrees are preserved", "{\"a\":{\"x\":1},\"b\":{\"y\":2}}", "{\"a\":{\"z\":3}}", "{\"a\":{\"x\":1,\"z\":3},\"b\":{\"y\":2}}")
                });
        }
    }
}
