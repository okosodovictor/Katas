using System.Collections.Generic;

namespace ExecutableCodeCounter.Parser
{
    /// <summary>
    /// Grammar symbol
    /// </summary>
    public interface ISymbol
    {
        /// <summary>
        /// The list of characters that make up this symbol
        /// </summary>
        IEnumerable<char> Value();

        /// <summary>
        /// The name of this symbol
        /// </summary>
        string Name { get; }
    }
}
