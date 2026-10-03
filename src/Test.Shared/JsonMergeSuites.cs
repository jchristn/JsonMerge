namespace Test.Shared
{
    using System.Collections.Generic;
    using Test.Shared.Suites;
    using Touchstone.Core;

    /// <summary>
    /// Every JsonMerge test suite.
    /// </summary>
    public static class JsonMergeSuites
    {
        /// <summary>
        /// All suites, in execution order.
        /// </summary>
        public static IReadOnlyList<TestSuiteDescriptor> All
        {
            get
            {
                return new List<TestSuiteDescriptor>
                {
                    BasicMergeSuite.Create(),
                    NestedMergeSuite.Create(),
                    ArraySuite.Create(),
                    OutputFormatSuite.Create(),
                    ArgumentValidationSuite.Create(),
                    MalformedJsonSuite.Create(),
                    TryMergeSuite.Create(),
                    LimitsSuite.Create(),
                    DuplicateKeySuite.Create(),
                    OptionsSuite.Create(),
                    BehaviorSuite.Create()
                };
            }
        }
    }
}
