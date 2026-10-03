namespace Test.Shared.Suites
{
    using System.Collections.Generic;
    using Touchstone.Core;
    using static Test.Shared.CaseFactory;

    /// <summary>
    /// Arrays are atomic values: they are replaced, never concatenated or merged element-wise.
    /// </summary>
    public static class ArraySuite
    {
        private const string Id = "Array";

        /// <summary>
        /// Create the suite.
        /// </summary>
        /// <returns>Suite descriptor.</returns>
        public static TestSuiteDescriptor Create()
        {
            return new TestSuiteDescriptor(
                suiteId: Id,
                displayName: "Arrays",
                cases: new List<TestCaseDescriptor>
                {
                    Merge(Id, "ArrayReplaced", "Array values are replaced", "{\"arr\":[1,2,3]}", "{\"arr\":[4,5,6]}", "{\"arr\":[4,5,6]}"),
                    Merge(Id, "ArrayReplacedByShorter", "Array replaced by shorter array", "{\"arr\":[1,2,3]}", "{\"arr\":[9]}", "{\"arr\":[9]}"),
                    Merge(Id, "ArrayReplacedByEmpty", "Array replaced by empty array", "{\"arr\":[1,2,3]}", "{\"arr\":[]}", "{\"arr\":[]}"),
                    Merge(Id, "EmptyArrayReplaced", "Empty array replaced by populated array", "{\"arr\":[]}", "{\"arr\":[1]}", "{\"arr\":[1]}"),
                    Merge(Id, "ArrayOfObjectsNotMerged", "Arrays of objects are replaced, not merged element-wise", "{\"arr\":[{\"a\":1,\"b\":2}]}", "{\"arr\":[{\"a\":9}]}", "{\"arr\":[{\"a\":9}]}"),
                    Merge(Id, "ArrayAdded", "Array added when key is absent", "{\"x\":1}", "{\"arr\":[\"a\",null,true,{\"k\":1},[2]]}", "{\"x\":1,\"arr\":[\"a\",null,true,{\"k\":1},[2]]}"),
                    Merge(Id, "ArrayPreservedWhenNotMerged", "Array preserved when merge does not mention it", "{\"arr\":[1,2],\"x\":1}", "{\"x\":2}", "{\"arr\":[1,2],\"x\":2}"),
                    Merge(Id, "ArrayReplacedByPrimitive", "Array replaced by primitive", "{\"arr\":[1,2]}", "{\"arr\":5}", "{\"arr\":5}"),
                    Merge(Id, "PrimitiveReplacedByArray", "Primitive replaced by array", "{\"arr\":5}", "{\"arr\":[1,2]}", "{\"arr\":[1,2]}"),
                    Merge(Id, "NestedArrayReplaced", "Array inside nested object is replaced", "{\"a\":{\"tags\":[\"x\",\"y\"],\"n\":1}}", "{\"a\":{\"tags\":[\"z\"]}}", "{\"a\":{\"tags\":[\"z\"],\"n\":1}}")
                });
        }
    }
}
