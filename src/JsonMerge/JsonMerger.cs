namespace JsonMerge
{
    using System;
    using System.Text.Json;
    using System.Text.Json.Nodes;

    public static class JsonMerger
    {
        /// <summary>
        /// Merges JSON objects together, with merge values overwriting input values for matching keys.
        /// </summary>
        /// <param name="inputJson">The input JSON object.</param>
        /// <param name="mergeJson">The JSON object to merge into the input.  If the input already contains a key specified in mergeJson, it will be overwritten by the value in mergeJson.</param>
        /// <returns>A JSON string based on inputJson with merged and overwritten values based on mergeJson.</returns>
        public static string MergeJson(string inputJson, string mergeJson)
        {
            if (string.IsNullOrEmpty(inputJson))
            {
                throw new ArgumentNullException(nameof(inputJson), "Input JSON cannot be null or empty");
            }

            if (string.IsNullOrEmpty(mergeJson))
            {
                throw new ArgumentNullException(nameof(mergeJson), "Merge JSON cannot be null or empty");
            }

            JsonNode inputNode = JsonNode.Parse(inputJson);
            JsonNode mergeNode = JsonNode.Parse(mergeJson);

            if (!(inputNode is JsonObject inputObject))
            {
                throw new ArgumentException("Input JSON must be of type JSON object", nameof(inputJson));
            }

            if (!(mergeNode is JsonObject mergeObject))
            {
                throw new ArgumentException("Merge JSON must be of type JSON object", nameof(mergeJson));
            }

            JsonObject resultObject = JsonNode.Parse(inputJson).AsObject();

            MergeObjects(resultObject, mergeObject);

            return resultObject.ToJsonString();
        }

        /// <summary>
        /// Recursively merges properties from source into target.
        /// </summary>
        /// <param name="target">The target JSON object to merge into.</param>
        /// <param name="source">The source JSON object to merge from.</param>
        private static void MergeObjects(JsonObject target, JsonObject source)
        {
            foreach (System.Collections.Generic.KeyValuePair<string, JsonNode> property in source)
            {
                if (property.Value is JsonObject sourceNestedObject &&
                    target.TryGetPropertyValue(property.Key, out JsonNode existingValue) &&
                    existingValue is JsonObject targetNestedObject)
                {
                    // Both are objects, merge recursively
                    MergeObjects(targetNestedObject, sourceNestedObject);
                }
                else
                {
                    // Replace or add the value
                    target[property.Key] = property.Value?.DeepClone();
                }
            }
        }

        /// <summary>
        /// Attempts to merge JSON objects together, with merge values overwriting input values for matching keys.
        /// Returns false if inputs are null, empty, or not valid JSON objects.
        /// </summary>
        /// <param name="inputJson">The input JSON object.</param>
        /// <param name="mergeJson">The JSON object to merge into the input.</param>
        /// <param name="result">The merged JSON string, or null if the merge failed.</param>
        /// <returns>True if the merge succeeded, false otherwise.</returns>
        public static bool TryMergeJson(string inputJson, string mergeJson, out string result)
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
                JsonNode inputNode = JsonNode.Parse(inputJson);
                JsonNode mergeNode = JsonNode.Parse(mergeJson);

                if (!(inputNode is JsonObject inputObject))
                {
                    return false;
                }

                if (!(mergeNode is JsonObject mergeObject))
                {
                    return false;
                }

                JsonObject resultObject = JsonNode.Parse(inputJson).AsObject();

                MergeObjects(resultObject, mergeObject);

                result = resultObject.ToJsonString();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
