namespace Test.Shared.Suites
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using JsonMerge;
    using Touchstone.Core;
    using static Test.Shared.CaseFactory;

    /// <summary>
    /// Algebraic and operational properties: determinism, idempotence, identity, and thread safety.
    /// </summary>
    public static class BehaviorSuite
    {
        private const string Id = "Behavior";

        private const string Input = "{\"name\":\"svc\",\"timeout\":30,\"logging\":{\"level\":\"info\",\"sinks\":[\"console\"]},\"flags\":{\"a\":true}}";
        private const string Patch = "{\"timeout\":60,\"logging\":{\"level\":\"debug\",\"verbose\":true},\"flags\":null,\"extra\":[1,2]}";
        private const string Expected = "{\"name\":\"svc\",\"timeout\":60,\"logging\":{\"level\":\"debug\",\"sinks\":[\"console\"],\"verbose\":true},\"flags\":null,\"extra\":[1,2]}";

        /// <summary>
        /// Create the suite.
        /// </summary>
        /// <returns>Suite descriptor.</returns>
        public static TestSuiteDescriptor Create()
        {
            return new TestSuiteDescriptor(
                suiteId: Id,
                displayName: "Behavior",
                cases: new List<TestCaseDescriptor>
                {
                    Merge(Id, "ConfigurationOverlay", "Realistic configuration overlay", Input, Patch, Expected),
                    Case(Id, "Deterministic", "Repeated merges produce identical output", () =>
                    {
                        string first = JsonMerger.MergeJson(Input, Patch);
                        for (int i = 0; i < 100; i++)
                            Check.Equal(first, JsonMerger.MergeJson(Input, Patch), "iteration " + i);
                    }),
                    Case(Id, "Idempotent", "Applying the same merge twice equals applying it once", () =>
                    {
                        string once = JsonMerger.MergeJson(Input, Patch);
                        string twice = JsonMerger.MergeJson(once, Patch);
                        Check.Equal(once, twice, "merge(merge(a,b),b)");
                    }),
                    Case(Id, "SelfMerge", "Merging an object with itself returns an equal object", () =>
                    {
                        Check.JsonEqual(Input, JsonMerger.MergeJson(Input, Input), "merge(a,a)");
                    }),
                    Case(Id, "EmptyIsRightIdentity", "Merging an empty object returns an equal object", () =>
                    {
                        Check.JsonEqual(Input, JsonMerger.MergeJson(Input, "{}"), "merge(a,{})");
                    }),
                    Case(Id, "EmptyIsLeftIdentity", "Merging into an empty object returns the merge object", () =>
                    {
                        Check.JsonEqual(Patch, JsonMerger.MergeJson("{}", Patch), "merge({},b)");
                    }),
                    Case(Id, "NotCommutative", "Merge order matters when keys conflict", () =>
                    {
                        Check.JsonEqual("{\"a\":2}", JsonMerger.MergeJson("{\"a\":1}", "{\"a\":2}"), "merge(a,b)");
                        Check.JsonEqual("{\"a\":1}", JsonMerger.MergeJson("{\"a\":2}", "{\"a\":1}"), "merge(b,a)");
                    }),
                    Case(Id, "Associative", "Sequential merges are associative for objects", () =>
                    {
                        string a = "{\"x\":{\"p\":1,\"q\":1},\"y\":1}";
                        string b = "{\"x\":{\"q\":2,\"r\":2},\"z\":2}";
                        string c = "{\"x\":{\"r\":3,\"s\":3},\"y\":3}";
                        string left = JsonMerger.MergeJson(JsonMerger.MergeJson(a, b), c);
                        string right = JsonMerger.MergeJson(a, JsonMerger.MergeJson(b, c));
                        Check.JsonEqual(left, right, "(a+b)+c vs a+(b+c)");
                    }),
                    Case(Id, "ResultIsReparseable", "Output is valid JSON that can be merged again", () =>
                    {
                        string result = JsonMerger.MergeJson(Input, Patch);
                        Check.JsonEqual(result, JsonMerger.MergeJson(result, "{}"), "re-merge of output");
                    }),
                    CaseAsync(Id, "ConcurrentMerges", "Concurrent merges are independent and correct", async ct =>
                    {
                        List<Task> tasks = new List<Task>();
                        for (int t = 0; t < 16; t++)
                        {
                            int worker = t;
                            tasks.Add(Task.Run(() =>
                            {
                                for (int i = 0; i < 200; i++)
                                {
                                    ct.ThrowIfCancellationRequested();
                                    string merge = "{\"worker\":" + worker + ",\"logging\":{\"i\":" + i + "}}";
                                    string expected = "{\"name\":\"svc\",\"timeout\":30,\"logging\":{\"level\":\"info\",\"sinks\":[\"console\"],\"i\":" + i + "},\"flags\":{\"a\":true},\"worker\":" + worker + "}";
                                    Check.JsonEqual(expected, JsonMerger.MergeJson(Input, merge), "worker " + worker + " iteration " + i);
                                    Check.True(JsonMerger.TryMergeJson(Input, merge, out string tryResult), "TryMergeJson worker " + worker);
                                    Check.JsonEqual(expected, tryResult, "TryMergeJson worker " + worker + " iteration " + i);
                                }
                            }, ct));
                        }

                        await Task.WhenAll(tasks).ConfigureAwait(false);
                    })
                });
        }
    }
}
