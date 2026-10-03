namespace Test.Shared
{
    using System;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using JsonMerge;
    using Touchstone.Core;

    /// <summary>
    /// Builds Touchstone descriptors for the common case shapes.
    /// </summary>
    public static class CaseFactory
    {
        /// <summary>
        /// A case whose body is synchronous.
        /// </summary>
        /// <param name="suiteId">Suite id.</param>
        /// <param name="caseId">Case id.</param>
        /// <param name="displayName">Display name.</param>
        /// <param name="body">Case body.  Throws on failure.</param>
        /// <returns>Descriptor.</returns>
        public static TestCaseDescriptor Case(string suiteId, string caseId, string displayName, Action body)
        {
            return new TestCaseDescriptor(
                suiteId: suiteId,
                caseId: caseId,
                displayName: displayName,
                executeAsync: ct =>
                {
                    ct.ThrowIfCancellationRequested();
                    body();
                    return Task.CompletedTask;
                });
        }

        /// <summary>
        /// A case whose body is asynchronous.
        /// </summary>
        /// <param name="suiteId">Suite id.</param>
        /// <param name="caseId">Case id.</param>
        /// <param name="displayName">Display name.</param>
        /// <param name="body">Case body.  Throws on failure.</param>
        /// <returns>Descriptor.</returns>
        public static TestCaseDescriptor CaseAsync(string suiteId, string caseId, string displayName, Func<CancellationToken, Task> body)
        {
            return new TestCaseDescriptor(
                suiteId: suiteId,
                caseId: caseId,
                displayName: displayName,
                executeAsync: body);
        }

        /// <summary>
        /// A case asserting that merging succeeds with a semantically equal result through both MergeJson and TryMergeJson.
        /// </summary>
        /// <param name="suiteId">Suite id.</param>
        /// <param name="caseId">Case id.</param>
        /// <param name="displayName">Display name.</param>
        /// <param name="inputJson">Input JSON.</param>
        /// <param name="mergeJson">Merge JSON.</param>
        /// <param name="expectedJson">Expected merged JSON.</param>
        /// <returns>Descriptor.</returns>
        public static TestCaseDescriptor Merge(string suiteId, string caseId, string displayName, string inputJson, string mergeJson, string expectedJson)
        {
            return Case(suiteId, caseId, displayName, () =>
            {
                string result = JsonMerger.MergeJson(inputJson, mergeJson);
                Check.JsonEqual(expectedJson, result, "MergeJson result");

                Check.True(JsonMerger.TryMergeJson(inputJson, mergeJson, out string tryResult), "TryMergeJson returns true");
                Check.Equal(result, tryResult, "TryMergeJson result matches MergeJson result");
            });
        }

        /// <summary>
        /// A case asserting that merging with the given options produces exactly the expected string through both MergeJson and TryMergeJson.
        /// </summary>
        /// <param name="suiteId">Suite id.</param>
        /// <param name="caseId">Case id.</param>
        /// <param name="displayName">Display name.</param>
        /// <param name="options">Merge options.</param>
        /// <param name="inputJson">Input JSON.</param>
        /// <param name="mergeJson">Merge JSON.</param>
        /// <param name="expectedJson">Exact expected output.</param>
        /// <returns>Descriptor.</returns>
        public static TestCaseDescriptor MergeWith(string suiteId, string caseId, string displayName, JsonMergeOptions options, string inputJson, string mergeJson, string expectedJson)
        {
            return Case(suiteId, caseId, displayName, () =>
            {
                Check.Equal(expectedJson, JsonMerger.MergeJson(inputJson, mergeJson, options), "MergeJson result");

                Check.True(JsonMerger.TryMergeJson(inputJson, mergeJson, options, out string tryResult), "TryMergeJson returns true");
                Check.Equal(expectedJson, tryResult, "TryMergeJson result");
            });
        }

        /// <summary>
        /// A case asserting that merging produces exactly the expected string (property order and escaping included).
        /// </summary>
        /// <param name="suiteId">Suite id.</param>
        /// <param name="caseId">Case id.</param>
        /// <param name="displayName">Display name.</param>
        /// <param name="inputJson">Input JSON.</param>
        /// <param name="mergeJson">Merge JSON.</param>
        /// <param name="expectedJson">Exact expected output.</param>
        /// <returns>Descriptor.</returns>
        public static TestCaseDescriptor MergeExact(string suiteId, string caseId, string displayName, string inputJson, string mergeJson, string expectedJson)
        {
            return Case(suiteId, caseId, displayName, () =>
            {
                Check.Equal(expectedJson, JsonMerger.MergeJson(inputJson, mergeJson), "MergeJson result");

                Check.True(JsonMerger.TryMergeJson(inputJson, mergeJson, out string tryResult), "TryMergeJson returns true");
                Check.Equal(expectedJson, tryResult, "TryMergeJson result");
            });
        }

        /// <summary>
        /// A case asserting that MergeJson throws ArgumentNullException for the named parameter and that TryMergeJson returns false.
        /// </summary>
        /// <param name="suiteId">Suite id.</param>
        /// <param name="caseId">Case id.</param>
        /// <param name="displayName">Display name.</param>
        /// <param name="inputJson">Input JSON.</param>
        /// <param name="mergeJson">Merge JSON.</param>
        /// <param name="paramName">Expected parameter name.</param>
        /// <returns>Descriptor.</returns>
        public static TestCaseDescriptor NullArgument(string suiteId, string caseId, string displayName, string? inputJson, string? mergeJson, string paramName)
        {
            return Case(suiteId, caseId, displayName, () =>
            {
                ArgumentNullException e = Check.ThrowsExactly<ArgumentNullException>(
                    () => JsonMerger.MergeJson(inputJson!, mergeJson!), "MergeJson");
                Check.Equal(paramName, e.ParamName, "ParamName");
                VerifyTryFails(inputJson, mergeJson);
            });
        }

        /// <summary>
        /// A case asserting that MergeJson throws exactly ArgumentException (not a derived type) for the named parameter because the root is not an object, and that TryMergeJson returns false.
        /// </summary>
        /// <param name="suiteId">Suite id.</param>
        /// <param name="caseId">Case id.</param>
        /// <param name="displayName">Display name.</param>
        /// <param name="inputJson">Input JSON.</param>
        /// <param name="mergeJson">Merge JSON.</param>
        /// <param name="paramName">Expected parameter name.</param>
        /// <returns>Descriptor.</returns>
        public static TestCaseDescriptor NotObject(string suiteId, string caseId, string displayName, string inputJson, string mergeJson, string paramName)
        {
            return Case(suiteId, caseId, displayName, () =>
            {
                ArgumentException e = Check.ThrowsExactly<ArgumentException>(
                    () => JsonMerger.MergeJson(inputJson, mergeJson), "MergeJson");
                Check.Equal(paramName, e.ParamName, "ParamName");
                VerifyTryFails(inputJson, mergeJson);
            });
        }

        /// <summary>
        /// A case asserting that MergeJson throws JsonException (or a derived type) because the JSON is malformed, and that TryMergeJson returns false.
        /// </summary>
        /// <param name="suiteId">Suite id.</param>
        /// <param name="caseId">Case id.</param>
        /// <param name="displayName">Display name.</param>
        /// <param name="inputJson">Input JSON.</param>
        /// <param name="mergeJson">Merge JSON.</param>
        /// <returns>Descriptor.</returns>
        public static TestCaseDescriptor Malformed(string suiteId, string caseId, string displayName, string inputJson, string mergeJson)
        {
            return Case(suiteId, caseId, displayName, () =>
            {
                Check.Throws<JsonException>(() => JsonMerger.MergeJson(inputJson, mergeJson), "MergeJson");
                VerifyTryFails(inputJson, mergeJson);
            });
        }

        /// <summary>
        /// Assert that TryMergeJson returns false, sets the result to null, and does not throw.
        /// </summary>
        /// <param name="inputJson">Input JSON.</param>
        /// <param name="mergeJson">Merge JSON.</param>
        public static void VerifyTryFails(string? inputJson, string? mergeJson)
        {
            VerifyTryFails(inputJson, mergeJson, null);
        }

        /// <summary>
        /// Assert that TryMergeJson with options returns false, sets the result to null, and does not throw.
        /// </summary>
        /// <param name="inputJson">Input JSON.</param>
        /// <param name="mergeJson">Merge JSON.</param>
        /// <param name="options">Merge options, or null for the parameterless overload.</param>
        public static void VerifyTryFails(string? inputJson, string? mergeJson, JsonMergeOptions? options)
        {
            bool success;
            string result;

            try
            {
                if (options == null)
                    success = JsonMerger.TryMergeJson(inputJson!, mergeJson!, out result);
                else
                    success = JsonMerger.TryMergeJson(inputJson!, mergeJson!, options, out result);
            }
            catch (Exception e)
            {
                throw new AssertionFailedException("TryMergeJson must not throw but threw " + e.GetType().Name + ": " + e.Message);
            }

            Check.False(success, "TryMergeJson returns false");
            Check.Null(result, "TryMergeJson out result");
        }
    }
}
