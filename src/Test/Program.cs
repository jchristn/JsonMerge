using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using JsonMerge;

namespace Test
{
    internal static class Program
    {
        private static int _TestCount = 0;
        private static int _PassedCount = 0;
        private static int _FailedCount = 0;

        private static void Main(string[] args)
        {
            Console.WriteLine("=== JsonMerger Test Suite ===\n");

            // Test 1: Basic merge - add new property
            TestMerge(
                "Basic merge - add new property",
                "{\"a\":1}",
                "{\"b\":2}",
                "{\"a\":1,\"b\":2}"
            );

            // Test 2: Basic merge - overwrite existing property
            TestMerge(
                "Overwrite existing property",
                "{\"a\":1}",
                "{\"a\":2}",
                "{\"a\":2}"
            );

            // Test 3: Multiple properties merge
            TestMerge(
                "Multiple properties merge",
                "{\"a\":1,\"b\":2}",
                "{\"c\":3,\"d\":4}",
                "{\"a\":1,\"b\":2,\"c\":3,\"d\":4}"
            );

            // Test 4: Multiple properties with overwrite
            TestMerge(
                "Multiple properties with overwrite",
                "{\"a\":1,\"b\":2,\"c\":3}",
                "{\"b\":20,\"c\":30,\"d\":40}",
                "{\"a\":1,\"b\":20,\"c\":30,\"d\":40}"
            );

            // Test 5: String values
            TestMerge(
                "String values",
                "{\"name\":\"John\"}",
                "{\"surname\":\"Doe\"}",
                "{\"name\":\"John\",\"surname\":\"Doe\"}"
            );

            // Test 6: Boolean values
            TestMerge(
                "Boolean values",
                "{\"enabled\":true}",
                "{\"visible\":false}",
                "{\"enabled\":true,\"visible\":false}"
            );

            // Test 7: Null values
            TestMerge(
                "Null values",
                "{\"a\":1}",
                "{\"b\":null}",
                "{\"a\":1,\"b\":null}"
            );

            // Test 8: Mixed data types
            TestMerge(
                "Mixed data types",
                "{\"num\":42,\"str\":\"hello\",\"bool\":true}",
                "{\"nil\":null,\"float\":3.14}",
                "{\"num\":42,\"str\":\"hello\",\"bool\":true,\"nil\":null,\"float\":3.14}"
            );

            // Test 9: Nested objects - recursive merge
            TestMerge(
                "Nested objects - recursive merge",
                "{\"user\":{\"name\":\"John\"}}",
                "{\"user\":{\"age\":30}}",
                "{\"user\":{\"name\":\"John\",\"age\":30}}"
            );

            // Test 10: Array values
            TestMerge(
                "Array values",
                "{\"arr\":[1,2,3]}",
                "{\"arr\":[4,5,6]}",
                "{\"arr\":[4,5,6]}"  // Array gets replaced
            );

            // Test 11: Empty merge object
            TestMerge(
                "Empty merge object",
                "{\"a\":1,\"b\":2}",
                "{}",
                "{\"a\":1,\"b\":2}"
            );

            // Test 12: Complex nested structure
            TestMerge(
                "Complex nested structure",
                "{\"level1\":{\"level2\":{\"value\":1}},\"other\":\"data\"}",
                "{\"level1\":{\"level2\":{\"value\":2}},\"new\":\"field\"}",
                "{\"level1\":{\"level2\":{\"value\":2}},\"other\":\"data\",\"new\":\"field\"}"
            );

            // Test 13: Large object merge
            TestMerge(
                "Large object merge",
                "{\"a\":1,\"b\":2,\"c\":3,\"d\":4,\"e\":5}",
                "{\"f\":6,\"g\":7,\"h\":8,\"i\":9,\"j\":10}",
                "{\"a\":1,\"b\":2,\"c\":3,\"d\":4,\"e\":5,\"f\":6,\"g\":7,\"h\":8,\"i\":9,\"j\":10}"
            );

            // Test 14: Unicode and special characters
            TestMerge(
                "Unicode and special characters",
                "{\"emoji\":\"\ud83d\ude00\"}",
                "{\"chinese\":\"\u4e2d\u6587\"}",
                "{\"emoji\":\"\ud83d\ude00\",\"chinese\":\"\u4e2d\u6587\"}"
            );

            // Test 15: Numeric edge cases
            TestMerge(
                "Numeric edge cases",
                "{\"zero\":0,\"negative\":-42}",
                "{\"large\":999999999,\"decimal\":0.000001}",
                "{\"zero\":0,\"negative\":-42,\"large\":999999999,\"decimal\":0.000001}"
            );

            // Test 16: Overwrite different types
            TestMerge(
                "Overwrite different types",
                "{\"value\":123}",
                "{\"value\":\"string\"}",
                "{\"value\":\"string\"}"
            );

            // Test 17: Whitespace in strings
            TestMerge(
                "Whitespace in strings",
                "{\"text\":\"hello world\"}",
                "{\"tab\":\"hello\\tworld\"}",
                "{\"text\":\"hello world\",\"tab\":\"hello\\tworld\"}"
            );

            // Test 18: Empty strings
            TestMerge(
                "Empty strings",
                "{\"empty\":\"\"}",
                "{\"also\":\"\"}",
                "{\"empty\":\"\",\"also\":\"\"}"
            );

            // Test 19: Property name edge cases
            TestMerge(
                "Property names with special chars",
                "{\"normal\":1}",
                "{\"with-dash\":2,\"with.dot\":3,\"with_underscore\":4}",
                "{\"normal\":1,\"with-dash\":2,\"with.dot\":3,\"with_underscore\":4}"
            );

            // Test 20: Scientific notation
            TestMerge(
                "Scientific notation",
                "{\"small\":1e-10}",
                "{\"large\":1e10}",
                "{\"small\":1e-10,\"large\":1e10}"
            );

            // Test 21: Deep nested merge - multiple levels
            TestMerge(
                "Deep nested merge - multiple levels",
                "{\"a\":{\"b\":{\"c\":{\"d\":1}}}}",
                "{\"a\":{\"b\":{\"c\":{\"e\":2}}}}",
                "{\"a\":{\"b\":{\"c\":{\"d\":1,\"e\":2}}}}"
            );

            // Test 22: Nested object with sibling properties
            TestMerge(
                "Nested object with sibling properties",
                "{\"config\":{\"timeout\":30,\"retries\":3}}",
                "{\"config\":{\"retries\":5,\"verbose\":true}}",
                "{\"config\":{\"timeout\":30,\"retries\":5,\"verbose\":true}}"
            );

            // Test 23: Mixed nested and flat properties
            TestMerge(
                "Mixed nested and flat properties",
                "{\"user\":{\"id\":1},\"active\":true}",
                "{\"user\":{\"name\":\"joe\"},\"role\":\"admin\"}",
                "{\"user\":{\"id\":1,\"name\":\"joe\"},\"active\":true,\"role\":\"admin\"}"
            );

            // Test 24: Nested object replaced by primitive
            TestMerge(
                "Nested object replaced by primitive",
                "{\"data\":{\"value\":1}}",
                "{\"data\":\"string\"}",
                "{\"data\":\"string\"}"
            );

            // Test 25: Primitive replaced by nested object
            TestMerge(
                "Primitive replaced by nested object",
                "{\"data\":\"string\"}",
                "{\"data\":{\"value\":1}}",
                "{\"data\":{\"value\":1}}"
            );

            // Test 26: Three-level nested merge
            TestMerge(
                "Three-level nested merge",
                "{\"app\":{\"settings\":{\"ui\":{\"theme\":\"dark\"}}}}",
                "{\"app\":{\"settings\":{\"ui\":{\"lang\":\"en\"},\"cache\":true}}}",
                "{\"app\":{\"settings\":{\"ui\":{\"theme\":\"dark\",\"lang\":\"en\"},\"cache\":true}}}"
            );

            // Exception tests for MergeJson
            TestException("Null inputJson", null, "{}");
            TestException("Empty inputJson", "", "{}");
            TestException("Null mergeJson", "{}", null);
            TestException("Empty mergeJson", "{}", "");
            TestException("Invalid JSON inputJson", "{invalid}", "{}");
            TestException("Invalid JSON mergeJson", "{}", "{invalid}");
            TestException("Array as inputJson", "[1,2,3]", "{}");
            TestException("Array as mergeJson", "{}", "[1,2,3]");
            TestException("Primitive as inputJson", "123", "{}");
            TestException("Primitive as mergeJson", "{}", "123");

            // TryMergeJson tests
            TestTryMergeSuccess(
                "TryMerge - basic success",
                "{\"a\":1}",
                "{\"b\":2}",
                "{\"a\":1,\"b\":2}"
            );

            TestTryMergeFail("TryMerge - null inputJson", null, "{}");
            TestTryMergeFail("TryMerge - empty inputJson", "", "{}");
            TestTryMergeFail("TryMerge - null mergeJson", "{}", null);
            TestTryMergeFail("TryMerge - empty mergeJson", "{}", "");
            TestTryMergeFail("TryMerge - invalid inputJson", "{invalid}", "{}");
            TestTryMergeFail("TryMerge - invalid mergeJson", "{}", "{invalid}");
            TestTryMergeFail("TryMerge - array as inputJson", "[1,2,3]", "{}");
            TestTryMergeFail("TryMerge - array as mergeJson", "{}", "[1,2,3]");
            TestTryMergeFail("TryMerge - primitive as inputJson", "123", "{}");
            TestTryMergeFail("TryMerge - primitive as mergeJson", "{}", "123");

            // Summary
            Console.WriteLine("\n=== Test Summary ===");
            Console.WriteLine($"Total tests: {_TestCount}");
            Console.WriteLine($"Passed: {_PassedCount}");
            Console.WriteLine($"Failed: {_FailedCount}");

            if (_FailedCount > 0)
            {
                Console.WriteLine("\nTEST SUITE FAILED");
                Environment.Exit(1);
            }
            else
            {
                Console.WriteLine("\nALL TESTS PASSED");
                Environment.Exit(0);
            }
        }

        private static void TestMerge(string testName, string inputJson, string mergeJson, string expectedJson)
        {
            _TestCount++;
            Console.WriteLine($"Test {_TestCount}: {testName}");
            Console.WriteLine($"  Input:  {inputJson}");
            Console.WriteLine($"  Merge:  {mergeJson}");

            try
            {
                string result = JsonMerger.MergeJson(inputJson, mergeJson);
                Console.WriteLine($"  Result: {result}");

                // Parse and compare as JSON to handle property ordering
                if (JsonEquals(result, expectedJson))
                {
                    Console.WriteLine("  Status: PASS\n");
                    _PassedCount++;
                }
                else
                {
                    Console.WriteLine($"  Expected: {expectedJson}");
                    Console.WriteLine("  Status: FAIL\n");
                    _FailedCount++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  Exception: {ex.Message}");
                Console.WriteLine("  Status: FAIL (Exception)\n");
                _FailedCount++;
            }
        }

        private static void TestException(string testName, string inputJson, string mergeJson)
        {
            _TestCount++;
            Console.WriteLine($"Test {_TestCount}: {testName}");
            Console.WriteLine($"  Input:  {(inputJson ?? "null")}");
            Console.WriteLine($"  Merge:  {(mergeJson ?? "null")}");

            try
            {
                JsonMerger.MergeJson(inputJson, mergeJson);
                Console.WriteLine("  Status: FAIL (No exception thrown)\n");
                _FailedCount++;
            }
            catch (ArgumentNullException)
            {
                Console.WriteLine("  Status: PASS (ArgumentNullException)\n");
                _PassedCount++;
            }
            catch (ArgumentException)
            {
                Console.WriteLine("  Status: PASS (ArgumentException)\n");
                _PassedCount++;
            }
            catch (JsonException)
            {
                Console.WriteLine("  Status: PASS (JsonException)\n");
                _PassedCount++;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  Status: FAIL (Wrong exception: {ex.GetType().Name})\n");
                _FailedCount++;
            }
        }

        private static void TestTryMergeSuccess(string testName, string inputJson, string mergeJson, string expectedJson)
        {
            _TestCount++;
            Console.WriteLine($"Test {_TestCount}: {testName}");
            Console.WriteLine($"  Input:  {inputJson}");
            Console.WriteLine($"  Merge:  {mergeJson}");

            try
            {
                bool success = JsonMerger.TryMergeJson(inputJson, mergeJson, out string result);

                if (!success)
                {
                    Console.WriteLine("  Status: FAIL (TryMerge returned false)\n");
                    _FailedCount++;
                }
                else
                {
                    Console.WriteLine($"  Result: {result}");
                    if (JsonEquals(result, expectedJson))
                    {
                        Console.WriteLine("  Status: PASS\n");
                        _PassedCount++;
                    }
                    else
                    {
                        Console.WriteLine($"  Expected: {expectedJson}");
                        Console.WriteLine("  Status: FAIL\n");
                        _FailedCount++;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  Exception: {ex.Message}");
                Console.WriteLine("  Status: FAIL (Exception)\n");
                _FailedCount++;
            }
        }

        private static void TestTryMergeFail(string testName, string inputJson, string mergeJson)
        {
            _TestCount++;
            Console.WriteLine($"Test {_TestCount}: {testName}");
            Console.WriteLine($"  Input:  {(inputJson ?? "null")}");
            Console.WriteLine($"  Merge:  {(mergeJson ?? "null")}");

            try
            {
                bool success = JsonMerger.TryMergeJson(inputJson, mergeJson, out string result);

                if (!success)
                {
                    Console.WriteLine("  Status: PASS (Correctly returned false)\n");
                    _PassedCount++;
                }
                else
                {
                    Console.WriteLine($"  Got result: {result}");
                    Console.WriteLine("  Status: FAIL (TryMerge should have returned false)\n");
                    _FailedCount++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  Exception: {ex.Message}");
                Console.WriteLine("  Status: FAIL (Exception thrown)\n");
                _FailedCount++;
            }
        }

        private static bool JsonEquals(string json1, string json2)
        {
            try
            {
                JsonNode node1 = JsonNode.Parse(json1);
                JsonNode node2 = JsonNode.Parse(json2);
                return JsonNode.DeepEquals(node1, node2);
            }
            catch
            {
                return false;
            }
        }
    }
}