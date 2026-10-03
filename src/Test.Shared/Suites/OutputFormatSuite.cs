namespace Test.Shared.Suites
{
    using System.Collections.Generic;
    using Touchstone.Core;
    using static Test.Shared.CaseFactory;

    /// <summary>
    /// Exact output shape: property order, compact formatting, number text preservation, and string escaping.
    /// </summary>
    public static class OutputFormatSuite
    {
        private const string Id = "OutputFormat";

        /// <summary>
        /// Create the suite.
        /// </summary>
        /// <returns>Suite descriptor.</returns>
        public static TestSuiteDescriptor Create()
        {
            return new TestSuiteDescriptor(
                suiteId: Id,
                displayName: "Output Format",
                cases: new List<TestCaseDescriptor>
                {
                    MergeExact(Id, "InputOrderPreserved", "Input property order preserved; new keys appended", "{\"z\":1,\"a\":2}", "{\"m\":3,\"z\":9}", "{\"z\":9,\"a\":2,\"m\":3}"),
                    MergeExact(Id, "NestedOrderPreserved", "Nested property order preserved; new keys appended", "{\"o\":{\"b\":1,\"a\":2}}", "{\"o\":{\"c\":3,\"b\":4}}", "{\"o\":{\"b\":4,\"a\":2,\"c\":3}}"),
                    MergeExact(Id, "CompactOutput", "Insignificant whitespace is removed from output", "  {\n  \"a\" : 1 ,\t\"b\" : [ 1 , 2 ]\n}  ", "\r\n{ \"c\" : { \"d\" : true } }\t", "{\"a\":1,\"b\":[1,2],\"c\":{\"d\":true}}"),
                    MergeExact(Id, "NumberTextPreserved", "Number text is preserved exactly", "{\"a\":1.10,\"b\":-0}", "{\"c\":1E+10,\"d\":0.000001}", "{\"a\":1.10,\"b\":-0,\"c\":1E+10,\"d\":0.000001}"),
                    MergeExact(Id, "LargeNumberPrecision", "Numbers beyond 64-bit range keep full precision", "{\"a\":1}", "{\"big\":123456789012345678901234567890,\"pi\":3.14159265358979323846264338327950288}", "{\"a\":1,\"big\":123456789012345678901234567890,\"pi\":3.14159265358979323846264338327950288}"),
                    MergeExact(Id, "NonAsciiEscaped", "Non-ASCII characters are emitted as \\u escapes", "{\"a\":\"中文\"}", "{}", "{\"a\":\"\\u4E2D\\u6587\"}"),
                    MergeExact(Id, "HtmlSensitiveEscaped", "HTML-sensitive characters are emitted as \\u escapes", "{}", "{\"a\":\"<&>'+\"}", "{\"a\":\"\\u003C\\u0026\\u003E\\u0027\\u002B\"}")
                });
        }
    }
}
