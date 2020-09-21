using System;

namespace ExecutableCodeCounter.Parser
{
    public struct ParseResult
    {
        private object _innerResult;

        /// <summary>
        /// If this represents an error result, the error message will be found here
        /// </summary>
        public string ErrorMessage { get; }

        /// <summary>
        /// For a successful result, the parsed symbol will be contained here
        /// </summary>
        public ISymbol Symbol { get; }

        /// <summary>
        /// For non-terminals, this will contain the failed result of the inner symbol that couldn't be parsed
        /// </summary>
        public ParseResult? InnerResult() => (ParseResult?) _innerResult;

        /// <summary>
        /// Indicates if this result represents a failed result
        /// </summary>
        public bool IsErrorResult => Symbol == null;

        /// <summary>
        /// Create a new successful Parse result
        /// </summary>
        /// <param name="symbol">the symbol</param>
        public ParseResult(ISymbol symbol)
        {
            Symbol = symbol;
            ErrorMessage = null;
            _innerResult = null;
        }

        /// <summary>
        /// Create a new failed parse result
        /// </summary>
        /// <param name="error">The error message</param>
        /// <param name="innerResult">The inner result if applicable - for non-terminals</param>
        public ParseResult(string error, ParseResult? innerResult = null)
        {
            ErrorMessage = error;
            _innerResult = innerResult;
            Symbol = null;
        }

        /// <summary>
        /// Get the innermost error message
        /// </summary>
        public string TerminalError
        {
            get
            {
                if (IsErrorResult)
                {
                    if (InnerResult() == null)
                        return ErrorMessage;

                    else
                    {
                        var inner = InnerResult();
                        while (inner?.InnerResult() != null)
                            inner = inner?.InnerResult();

                        return inner?.ErrorMessage;
                    }
                }

                else return null;
            }
        }
    }
}
