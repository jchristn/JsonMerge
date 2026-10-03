namespace JsonMerge
{
    using System;

    /// <summary>
    /// Options controlling how JSON objects are merged and how the result is written.
    /// </summary>
    public class JsonMergeOptions
    {
        #region Public-Members

        /// <summary>
        /// Default maximum nesting depth.
        /// </summary>
        public const int DefaultMaxDepth = 64;

        /// <summary>
        /// Largest permitted value for <see cref="MaxDepth"/>.
        /// </summary>
        public const int MaximumMaxDepth = 1000;

        /// <summary>
        /// Maximum nesting depth permitted when parsing either JSON document.
        /// Default is 64.  Minimum is 1, maximum is 1000.
        /// Documents nested more deeply cause a JsonException.
        /// </summary>
        public int MaxDepth
        {
            get
            {
                return _MaxDepth;
            }
            set
            {
                if (value < 1 || value > MaximumMaxDepth)
                    throw new ArgumentOutOfRangeException(nameof(MaxDepth), "MaxDepth must be between 1 and " + MaximumMaxDepth + ".");
                _MaxDepth = value;
            }
        }

        /// <summary>
        /// When true, a null value in the merge JSON removes the corresponding property from the result, and null values
        /// inside newly added objects are omitted (JSON Merge Patch, RFC 7396, semantics).
        /// When false, a null value in the merge JSON is written to the result as null.
        /// Default is false.
        /// </summary>
        public bool NullRemovesProperty { get; set; } = false;

        /// <summary>
        /// When true, the result is written with indentation and line breaks.
        /// Default is false (compact output).
        /// </summary>
        public bool WriteIndented { get; set; } = false;

        /// <summary>
        /// When true, non-ASCII characters and HTML-sensitive characters (such as &lt;, &gt;, &amp;, and ') are written as-is
        /// rather than as \uXXXX escape sequences, using JavaScriptEncoder.UnsafeRelaxedJsonEscaping.
        /// Characters outside the Basic Multilingual Plane (such as emoji) are still written as surrogate-pair escapes.
        /// Do not enable this if the result will be embedded directly in HTML or script.
        /// Default is false.
        /// </summary>
        public bool UseRelaxedEscaping { get; set; } = false;

        #endregion

        #region Private-Members

        private int _MaxDepth = DefaultMaxDepth;

        #endregion

        #region Constructors-and-Factories

        /// <summary>
        /// Instantiate the object with default values.
        /// </summary>
        public JsonMergeOptions()
        {
        }

        #endregion
    }
}
