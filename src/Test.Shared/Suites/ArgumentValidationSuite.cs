namespace Test.Shared.Suites
{
    using System.Collections.Generic;
    using Touchstone.Core;
    using static Test.Shared.CaseFactory;

    /// <summary>
    /// Null, empty, and non-object arguments are rejected with the documented exception and parameter name.
    /// </summary>
    public static class ArgumentValidationSuite
    {
        private const string Id = "ArgumentValidation";

        /// <summary>
        /// Create the suite.
        /// </summary>
        /// <returns>Suite descriptor.</returns>
        public static TestSuiteDescriptor Create()
        {
            return new TestSuiteDescriptor(
                suiteId: Id,
                displayName: "Argument Validation",
                cases: new List<TestCaseDescriptor>
                {
                    // null and empty
                    NullArgument(Id, "NullInput", "Null inputJson", null, "{}", "inputJson"),
                    NullArgument(Id, "EmptyInput", "Empty inputJson", "", "{}", "inputJson"),
                    NullArgument(Id, "NullMerge", "Null mergeJson", "{}", null, "mergeJson"),
                    NullArgument(Id, "EmptyMerge", "Empty mergeJson", "{}", "", "mergeJson"),
                    NullArgument(Id, "BothNull", "Both null reports inputJson first", null, null, "inputJson"),
                    NullArgument(Id, "NullMergeWithInvalidInput", "Null mergeJson reported before inputJson is parsed", "{invalid}", null, "mergeJson"),

                    // root is not an object
                    NotObject(Id, "ArrayInput", "Array as inputJson", "[1,2,3]", "{}", "inputJson"),
                    NotObject(Id, "ArrayMerge", "Array as mergeJson", "{}", "[1,2,3]", "mergeJson"),
                    NotObject(Id, "NumberInput", "Number as inputJson", "123", "{}", "inputJson"),
                    NotObject(Id, "NumberMerge", "Number as mergeJson", "{}", "123", "mergeJson"),
                    NotObject(Id, "StringInput", "String as inputJson", "\"text\"", "{}", "inputJson"),
                    NotObject(Id, "StringMerge", "String as mergeJson", "{}", "\"text\"", "mergeJson"),
                    NotObject(Id, "BooleanInput", "Boolean as inputJson", "true", "{}", "inputJson"),
                    NotObject(Id, "BooleanMerge", "Boolean as mergeJson", "{}", "false", "mergeJson"),
                    NotObject(Id, "NullLiteralInput", "JSON null literal as inputJson", "null", "{}", "inputJson"),
                    NotObject(Id, "NullLiteralMerge", "JSON null literal as mergeJson", "{}", "null", "mergeJson"),
                    NotObject(Id, "EmptyArrayInput", "Empty array as inputJson", "[]", "{}", "inputJson"),
                    NotObject(Id, "BothArrays", "Both arrays reports inputJson first", "[1]", "[2]", "inputJson")
                });
        }
    }
}
