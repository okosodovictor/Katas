using System;

namespace ExecutableCodeCounter.Parser
{
	public class ParserException : Exception
	{
		/// <summary>
		/// The result
		/// </summary>
		public ParseResult Result { get; }

		/// <summary>
		/// Create a new instance of the exception
		/// </summary>
		/// <param name="result">the result</param>
		public ParserException(
			ParseResult result)
			: base(result.TerminalError)
		{
			Result = result;
		}
	}
}
