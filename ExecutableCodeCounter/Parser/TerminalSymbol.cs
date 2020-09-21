using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecutableCodeCounter.Parser
{
    public class TerminalSymbol : ISymbol
    {
        /// <summary>
        /// The character that this symbol represents
        /// </summary>
        public readonly char Terminal;

        /// <summary>
        /// The name of this symbol
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// An array/enumerable of the exact characters from which this symbol was parsed
        /// </summary>
        /// <returns>the list of characters</returns>
        public IEnumerable<char> Value() => Terminal.ToString();

        /// <summary>
        /// Gets a string representation of the symbol's value
        /// </summary>
        /// <returns>the string representation</returns>
        public string StringValue() => Terminal.ToString();

        ///<inheritdoc/>
        public bool NameIs(object name) => Name?.Equals(name?.ToString()) == true;

        /// <summary>
        /// String representation of this Symbol
        /// </summary>
        /// <returns>the string value</returns>
        public override string ToString() => $"{Name}[{new string(Value().ToArray())}]";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">name of the symbol</param>
        /// <param name="element">character of the symbol</param>
        /// <param name="escapeCharacter">if the character was escaped, the escape-character is supplied, otherwise null</param>
        public TerminalSymbol(object name, char element)
        {
            Terminal = element;
            Name = name is string sname
                ? sname
                : name?.ToString()
                ?? throw new ArgumentNullException(nameof(name));
        }
    }
}
