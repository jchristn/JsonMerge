namespace Test.Shared.Suites
{
    using System.Collections.Generic;
    using Touchstone.Core;
    using static Test.Shared.CaseFactory;

    /// <summary>
    /// Syntactically invalid JSON is rejected with JsonException, and TryMergeJson returns false.
    /// </summary>
    public static class MalformedJsonSuite
    {
        private const string Id = "MalformedJson";

        /// <summary>
        /// Create the suite.
        /// </summary>
        /// <returns>Suite descriptor.</returns>
        public static TestSuiteDescriptor Create()
        {
            return new TestSuiteDescriptor(
                suiteId: Id,
                displayName: "Malformed JSON",
                cases: new List<TestCaseDescriptor>
                {
                    Malformed(Id, "InvalidInput", "Invalid JSON inputJson", "{invalid}", "{}"),
                    Malformed(Id, "InvalidMerge", "Invalid JSON mergeJson", "{}", "{invalid}"),
                    Malformed(Id, "WhitespaceOnlyInput", "Whitespace-only inputJson", "   ", "{}"),
                    Malformed(Id, "WhitespaceOnlyMerge", "Whitespace-only mergeJson", "{}", "\t\n "),
                    Malformed(Id, "TruncatedInput", "Truncated inputJson", "{\"a\":", "{}"),
                    Malformed(Id, "UnclosedObjectMerge", "Unclosed object in mergeJson", "{}", "{\"a\":1"),
                    Malformed(Id, "UnclosedStringInput", "Unclosed string in inputJson", "{\"a\":\"text}", "{}"),
                    Malformed(Id, "TrailingCommaInput", "Trailing comma in inputJson", "{\"a\":1,}", "{}"),
                    Malformed(Id, "TrailingCommaMerge", "Trailing comma in mergeJson", "{}", "{\"a\":[1,2,],\"b\":1}"),
                    Malformed(Id, "CommentInput", "Comments are not allowed", "{/*c*/\"a\":1}", "{}"),
                    Malformed(Id, "LineCommentMerge", "Line comments are not allowed", "{}", "{\"a\":1 // c\n}"),
                    Malformed(Id, "SingleQuotes", "Single-quoted strings are not allowed", "{'a':1}", "{}"),
                    Malformed(Id, "UnquotedKey", "Unquoted property names are not allowed", "{}", "{a:1}"),
                    Malformed(Id, "MultipleRootValues", "Multiple root values", "{}{}", "{}"),
                    Malformed(Id, "TrailingGarbage", "Trailing garbage after the object", "{}", "{\"a\":1}x"),
                    Malformed(Id, "LeadingByteOrderMark", "Leading byte order mark character", "\uFEFF{\"a\":1}", "{}"),
                    Malformed(Id, "InvalidLiteral", "Invalid literal value", "{\"a\":tru}", "{}"),
                    Malformed(Id, "NaNLiteral", "NaN is not valid JSON", "{}", "{\"a\":NaN}"),
                    Malformed(Id, "LeadingZeroNumber", "Number with leading zero", "{\"a\":01}", "{}"),
                    Malformed(Id, "InvalidEscape", "Invalid escape sequence", "{}", "{\"a\":\"\\x\"}"),
                    Malformed(Id, "ValidInputInvalidNestedMerge", "Error deep within mergeJson", "{\"a\":{\"b\":1}}", "{\"a\":{\"b\":}}")
                });
        }
    }
}
