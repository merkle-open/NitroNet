using System;
using Veil.Parser;

namespace Veil
{
    /// <summary>
    /// Represents general errors during parsing templates.
    /// </summary>
    public class VeilParserException : Exception
    {
	    public SourceLocation Location { get; private set; }

	    /// <summary>
        /// Create an exception with the supplied message
        /// </summary>
        public VeilParserException(string message, SourceLocation location)
            : base(message)
        {
	        Location = location;
        }
    }
}