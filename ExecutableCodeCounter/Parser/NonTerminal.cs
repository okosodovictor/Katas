using System;
using System.Collections.Generic;
using System.Linq;

namespace ExecutableCodeCounter.Parser
{
    public class NonTerminalSymbol : ISymbol
    {
        private readonly List<ISymbol> _symbols;

        /// <summary>
        /// Indicates that this non-terminal did not terminate at the end of the input, and had it's termination symbol(s) added
        /// </summary>
        public bool IsSpecialTermination { get; }

        /// <summary>
        /// The name of the symbol
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Indicating if this symbol has any child symbols
        /// </summary>
        public bool IsEmpty => _symbols.Count == 0;

        /// <summary>
        /// Returns the internal symbol list
        /// </summary>
        public IEnumerable<ISymbol> Symbols => _symbols;

        /// <summary>
        /// Returns the string representation from which this symbol was parsed
        /// </summary>
        /// <returns>the list of characters that makes up this symbol</returns>
        public IEnumerable<char> Value() => _symbols.SelectMany(symbol => symbol.Value());

        /// <summary>
        /// Gets the first child symbol with the given name: null otherwise.
        /// </summary>
        /// <param name="name">the name</param>
        /// <returns>the found symbol or null</returns>
        public ISymbol GetSymbol(string name) => _symbols.FirstOrDefault(s => s.Name == name);

        /// <summary>
        /// Gets all the child symbols with the given name: an empty enumerable otherwise.
        /// </summary>
        /// <param name="name">the name</param>
        /// <returns>the matching symbols or an empty list</returns>
        public IEnumerable<ISymbol> GetSymbols(string name) => _symbols.Where(s => s.Name == name);
        
        #region Possibly unneessary
        /// <summary>
        /// Find all symbols with the given name
        /// </summary>
        /// <param name="name">the name</param>
        /// <returns>the matching symbols</returns>
        public IEnumerable<ISymbol> FindAll(string name)
        {
            return _symbols.Aggregate(new List<ISymbol>(), (list, nextSymbol) =>
            {
                if (nextSymbol.Name == name)
                    list.Add(nextSymbol);

                if (nextSymbol is NonTerminalSymbol nts)
                    list.AddRange(nts.FindAll(name));

                return list;
            });
        }

        /// <summary>
        /// Search through the hierarchy of symbols for the first occurence of the successive name given and separated by '/'
        /// </summary>
        /// <param name="path">the path</param>
        /// <returns>the matching symbols</returns>
        public ISymbol Find(string path)
        {
            var parts = path.Split(
                new[] { '/' },
                StringSplitOptions.RemoveEmptyEntries);

            ISymbol symbol = null;
            IEnumerable<ISymbol> symbols = _symbols;
            foreach (var name in parts)
            {
                if (symbols == null)
                    return null;

                symbol = symbols.FirstOrDefault(_s => _s.Name == name);

                if (symbol is NonTerminalSymbol nts)
                    symbols = nts._symbols;

                else
                    symbols = null;
            }

            return symbol;
        }

        #endregion

        /// <summary>
        /// Returns a string representation of this symbol
        /// </summary>
        /// <returns>the string value</returns>
        public override string ToString() => $"{Name}[{_symbols.Count}]";

        #region Constructors
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Name of the symbol</param>
        /// <param name="symbols">Child symbols</param>
        public NonTerminalSymbol(object name, params ISymbol[] symbols)
        : this(name, symbols.AsEnumerable())
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Name of the symbol</param>
        /// <param name="specialTermination">Indicates that this symbol was specially terminated</param>
        /// <param name="symbols">Child symbols</param>
        public NonTerminalSymbol(
            object name,
            bool specialTermination,
            params ISymbol[] symbols)
        : this(name, symbols.AsEnumerable(), specialTermination)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">name of the symbol</param>
        /// <param name="symbols">Child symbols</param>
        /// <param name="specialTermination">Indicates that this symbol was specially terminated</param>
        public NonTerminalSymbol(
            object name,
            IEnumerable<ISymbol> symbols,
            bool specialTermination = false)
        {
            if (symbols == null || symbols.Any(symbol => symbol == null))
                throw new Exception("Invalid elements");

            IsSpecialTermination = specialTermination;

            _symbols = symbols.ToList();

            Name = name is string sname
                ? sname
                : name?.ToString()
                ?? throw new ArgumentNullException(nameof(name));
        }
        #endregion
    }
}
