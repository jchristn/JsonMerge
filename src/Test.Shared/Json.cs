namespace Test.Shared
{
    using System.Text;

    /// <summary>
    /// Builders for generated JSON inputs.
    /// </summary>
    public static class Json
    {
        /// <summary>
        /// Build a chain of nested objects, each with a single property named "x", ending in the given leaf value.
        /// The result contains exactly <paramref name="depth"/> objects.
        /// </summary>
        /// <param name="depth">Number of nested objects (at least 1).</param>
        /// <param name="leaf">Raw JSON for the innermost value.</param>
        /// <returns>JSON string.</returns>
        public static string Nested(int depth, string leaf = "1")
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 1; i < depth; i++) sb.Append("{\"x\":");
            sb.Append("{\"x\":").Append(leaf).Append('}');
            for (int i = 1; i < depth; i++) sb.Append('}');
            return sb.ToString();
        }

        /// <summary>
        /// Build a flat object with <paramref name="count"/> properties named prefix0..prefixN, each with its index as the value.
        /// </summary>
        /// <param name="prefix">Property name prefix.</param>
        /// <param name="count">Number of properties.</param>
        /// <param name="offset">Value added to each index.</param>
        /// <returns>JSON string.</returns>
        public static string Flat(string prefix, int count, int offset = 0)
        {
            StringBuilder sb = new StringBuilder("{");
            for (int i = 0; i < count; i++)
            {
                if (i > 0) sb.Append(',');
                sb.Append('"').Append(prefix).Append(i).Append("\":").Append(i + offset);
            }
            sb.Append('}');
            return sb.ToString();
        }
    }
}
