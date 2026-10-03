namespace JsonMerge
{
    using System;
    using System.Collections.Generic;
    using System.Text.Encodings.Web;
    using System.Text.Json;
    using System.Text.Json.Nodes;

    /// <summary>
    /// Simply and elegantly merge values into an existing JSON object.
    /// </summary>
    public static class JsonMerger
    {
        private static readonly JsonMergeOptions _DefaultOptions = new JsonMergeOptions();

        private static readonly JsonSerializerOptions _Indented = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        private static readonly JsonSerializerOptions _Relaxed = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        private static readonly JsonSerializerOptions _IndentedRelaxed = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        /// <summary>
        /// Merges JSON objects together, with merge values overwriting input values for matching keys.
        /// </summary>
        /// <param name="inputJson">The input JSON object.</param>
        /// <param name="mergeJson">The JSON object to merge into the input.  If the input already contains a key specified in mergeJson, it will be overwritten by the value in mergeJson.</param>
        /// <returns>A JSON string based on inputJson with merged and overwritten values based on mergeJson.</returns>
        /// <exception cref="ArgumentNullException">Thrown when inputJson or mergeJson is null or empty.</exception>
        /// <exception cref="ArgumentException">Thrown when inputJson or mergeJson is not a JSON object, or contains duplicate property names.</exception>
        /// <exception cref="JsonException">Thrown when inputJson or mergeJson is not valid JSON or exceeds the maximum depth.</exception>
        public static string MergeJson(string inputJson, string mergeJson)
        {
            return MergeJson(inputJson, mergeJson, null);
        }

        /// <summary>
        /// Merges JSON objects together, with merge values overwriting input values for matching keys.
        /// </summary>
        /// <param name="inputJson">The input JSON object.</param>
        /// <param name="mergeJson">The JSON object to merge into the input.  If the input already contains a key specified in mergeJson, it will be overwritten by the value in mergeJson.</param>
        /// <param name="options">Merge options.  Null uses the defaults.</param>
        /// <returns>A JSON string based on inputJson with merged and overwritten values based on mergeJson.</returns>
        /// <exception cref="ArgumentNullException">Thrown when inputJson or mergeJson is null or empty.</exception>
        /// <exception cref="ArgumentException">Thrown when inputJson or mergeJson is not a JSON object, or contains duplicate property names.</exception>
        /// <exception cref="JsonException">Thrown when inputJson or mergeJson is not valid JSON or exceeds the maximum depth.</exception>
        public static string MergeJson(string inputJson, string mergeJson, JsonMergeOptions options)
        {
            if (string.IsNullOrEmpty(inputJson))
            {
                throw new ArgumentNullException(nameof(inputJson), "Input JSON cannot be null or empty");
            }

            if (string.IsNullOrEmpty(mergeJson))
            {
                throw new ArgumentNullException(nameof(mergeJson), "Merge JSON cannot be null or empty");
            }

            if (options == null) options = _DefaultOptions;

            JsonObject resultObject = ParseObject(inputJson, nameof(inputJson), "Input JSON", options);
            JsonObject mergeObject = ParseObject(mergeJson, nameof(mergeJson), "Merge JSON", options);

            MergeObjects(resultObject, mergeObject, options);

            return Serialize(resultObject, options);
        }

        /// <summary>
        /// Attempts to merge JSON objects together, with merge values overwriting input values for matching keys.
        /// Returns false if inputs are null, empty, not valid JSON objects, or contain duplicate property names.
        /// </summary>
        /// <param name="inputJson">The input JSON object.</param>
        /// <param name="mergeJson">The JSON object to merge into the input.</param>
        /// <param name="result">The merged JSON string, or null if the merge failed.</param>
        /// <returns>True if the merge succeeded, false otherwise.</returns>
        public static bool TryMergeJson(string inputJson, string mergeJson, out string result)
        {
            return TryMergeJson(inputJson, mergeJson, null, out result);
        }

        /// <summary>
        /// Attempts to merge JSON objects together, with merge values overwriting input values for matching keys.
        /// Returns false if inputs are null, empty, not valid JSON objects, or contain duplicate property names.
        /// </summary>
        /// <param name="inputJson">The input JSON object.</param>
        /// <param name="mergeJson">The JSON object to merge into the input.</param>
        /// <param name="options">Merge options.  Null uses the defaults.</param>
        /// <param name="result">The merged JSON string, or null if the merge failed.</param>
        /// <returns>True if the merge succeeded, false otherwise.</returns>
        public static bool TryMergeJson(string inputJson, string mergeJson, JsonMergeOptions options, out string result)
        {
            result = null;

            if (string.IsNullOrEmpty(inputJson))
            {
                return false;
            }

            if (string.IsNullOrEmpty(mergeJson))
            {
                return false;
            }

            try
            {
                result = MergeJson(inputJson, mergeJson, options);
                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }

        /// <summary>
        /// Parses a JSON object and validates that it contains no duplicate property names at any depth.
        /// </summary>
        /// <param name="json">The JSON text.</param>
        /// <param name="paramName">Name of the parameter supplying the JSON.</param>
        /// <param name="description">Description used in exception messages.</param>
        /// <param name="options">Merge options.</param>
        /// <returns>The parsed object.</returns>
        private static JsonObject ParseObject(string json, string paramName, string description, JsonMergeOptions options)
        {
            JsonDocumentOptions documentOptions = new JsonDocumentOptions
            {
                MaxDepth = options.MaxDepth
            };

            JsonNode node = JsonNode.Parse(json, null, documentOptions);

            if (!(node is JsonObject obj))
            {
                throw new ArgumentException(description + " must be of type JSON object", paramName);
            }

            try
            {
                Materialize(obj);
            }
            catch (ArgumentException e)
            {
                throw new ArgumentException(description + " contains duplicate property names: " + e.Message, paramName, e);
            }

            return obj;
        }

        /// <summary>
        /// Walks every node so that lazily-parsed objects are materialized, which surfaces duplicate property names.
        /// </summary>
        /// <param name="node">The node to walk.</param>
        private static void Materialize(JsonNode node)
        {
            if (node is JsonObject obj)
            {
                foreach (KeyValuePair<string, JsonNode> property in obj)
                {
                    Materialize(property.Value);
                }
            }
            else if (node is JsonArray array)
            {
                foreach (JsonNode item in array)
                {
                    Materialize(item);
                }
            }
        }

        /// <summary>
        /// Recursively merges properties from source into target.
        /// </summary>
        /// <param name="target">The target JSON object to merge into.</param>
        /// <param name="source">The source JSON object to merge from.</param>
        /// <param name="options">Merge options.</param>
        private static void MergeObjects(JsonObject target, JsonObject source, JsonMergeOptions options)
        {
            foreach (KeyValuePair<string, JsonNode> property in source)
            {
                if (property.Value == null && options.NullRemovesProperty)
                {
                    // Null removes the property
                    target.Remove(property.Key);
                }
                else if (property.Value is JsonObject sourceNestedObject)
                {
                    if (target.TryGetPropertyValue(property.Key, out JsonNode existingValue) &&
                        existingValue is JsonObject targetNestedObject)
                    {
                        // Both are objects, merge recursively
                        MergeObjects(targetNestedObject, sourceNestedObject, options);
                    }
                    else
                    {
                        // Replace or add the object, applying null handling to its contents
                        JsonObject newObject = new JsonObject();
                        MergeObjects(newObject, sourceNestedObject, options);
                        target[property.Key] = newObject;
                    }
                }
                else
                {
                    // Replace or add the value
                    target[property.Key] = property.Value?.DeepClone();
                }
            }
        }

        /// <summary>
        /// Serializes the result according to the output options.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="options">Merge options.</param>
        /// <returns>JSON string.</returns>
        private static string Serialize(JsonObject obj, JsonMergeOptions options)
        {
            if (options.WriteIndented && options.UseRelaxedEscaping) return obj.ToJsonString(_IndentedRelaxed);
            if (options.WriteIndented) return obj.ToJsonString(_Indented);
            if (options.UseRelaxedEscaping) return obj.ToJsonString(_Relaxed);
            return obj.ToJsonString();
        }
    }
}
