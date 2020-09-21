using System;
using System.Collections.Generic;
using System.Text;
using ExecutableCodeCounter.Parser;

namespace ExecutableCodeCounter.CSharp
{
    using Buffer = BufferedEnumerator<char>;
    using Symbols = LanguageSymbols;

    public static class Parser
    {
        /// <summary>
		/// Entry method for the parser
		/// </summary>
		/// <param name="input">the input string</param>
		/// <param name="result">the result</param>
		/// <returns>indicating if the parse succeeded or failed</returns>
		public static bool TryParseCSharp(string input, out ParseResult result)
        {
            return TryParseInternal(new Buffer(input.GetEnumerator()), out result);
        }

        public static ParseResult ParseCSharp(string input)
        {
            if (TryParseCSharp(input, out var result))
                return result;

            else
            {
                throw new ParserException(result);
            }
        }


        #region Non terminals
        internal static bool TryParseInternal(Buffer buffer, out ParseResult result)
        {
            var components = new List<ISymbol>();

            while (TryParseString(buffer, out var r)
                || TryParseLineComment(buffer, out r)
                || TryParseMultiLineComment(buffer, out r)
                || TryParseWhitespace(buffer, out r)
                || TryParseNewLine(buffer, out r)
                || TryParsePseudoExpression(buffer, out r))
            {
                components.Add(r.Symbol);
            }

            result = new ParseResult(new NonTerminalSymbol(
                Symbols.Executable,
                components));
            return true;
        }

        #region Statement

        internal static bool TryParsePseudoExpression(Buffer buffer, out ParseResult result)
        {
            var position = buffer.Cursor;
            var components = new List<ISymbol>();
            var error = $"{Symbols.PseudoExpression} expected at position: {position + 1}";

            while (buffer.TryNext(out var @char))
            {
                if ((@char == '/' && TryParseFSlash(buffer, out _))
                    || (@char == '/' && TryParseStar(buffer, out _))
                    || (@char == '$' && TryParseQuote(buffer, out _))
                    || (@char == '@' & TryParseQuote(buffer, out _))
                    || @char == '"' || @char == '\n' || @char == '\r'
                    || @char == ' ' || @char == '\t')
                {
                    if (components.Count == 0)
                        return Fail(error, buffer, position, out result);

                    buffer.Back();
                    result = new ParseResult(new NonTerminalSymbol(
                        Symbols.PseudoExpression,
                        components));
                    return true;
                }

                components.Add(new TerminalSymbol(
                    Symbols.Character,
                    @char));
            }

            if (components.Count > 0)
            {
                //end of input 
                result = new ParseResult(new NonTerminalSymbol(
                    Symbols.PseudoExpression,
                    components));
                return true;
            }

            else return Fail(error, buffer, position, out result);
        }

        #endregion

        #region MultiLineComment
        internal static bool TryParseMultiLineComment(Buffer buffer, out ParseResult result)
        {
            var position = buffer.Cursor;
            var components = new List<ISymbol>();
            var error = $"{Symbols.MultiLineComment} expected at position: {position + 1}";

            //opening
            if (TryParseMLCOpening(buffer, out var opening))
                components.Add(opening.Symbol);

            else return Fail(error, buffer, position, out result);

            while (buffer.TryNext(out var @char))
            {
                if (@char == '*' && TryParseFSlash(buffer, out var fslash))
                {
                    components
                        .Append(new TerminalSymbol(
                            Symbols.Star,
                            @char))
                        .Append(fslash.Symbol);

                    result = new ParseResult(new NonTerminalSymbol(
                        Symbols.MultiLineComment,
                        components));
                    return true;
                }

                components.Add(new TerminalSymbol(
                    Symbols.Character,
                    @char));
            }

            //end of input without closing the comment.
            //add the end of comment for it
            components.Add(new NonTerminalSymbol(
                Symbols.MLCClosing,
                new TerminalSymbol(
                    Symbols.Star,
                    '*'),
                new TerminalSymbol(
                    Symbols.FSlash,
                    '/')));

            result = new ParseResult(new NonTerminalSymbol(
                Symbols.MultiLineComment,
                components,
                true));
            return true;
        }

        internal static bool TryParseMLCOpening(Buffer buffer, out ParseResult result)
        {
            var position = buffer.Cursor;
            var error = $"{Symbols.MLCOpening} expected at position: {position + 1}";

            // /*
            if (TryParseFSlash(buffer, out var fslash)
                && TryParseStar(buffer, out var star))
            {
                result = new ParseResult(new NonTerminalSymbol(
                    Symbols.MLCOpening,
                    fslash.Symbol,
                    star.Symbol));

                return true;
            }
            else return Fail(error, buffer, position, out result);
        }

        internal static bool TryParseMLCClosing(Buffer buffer, out ParseResult result)
        {
            var position = buffer.Cursor;
            var error = $"{Symbols.MLCClosing} expected at position: {position + 1}";

            // */
            if (TryParseFSlash(buffer, out var star)
                && TryParseStar(buffer, out var fslash))
            {
                result = new ParseResult(new NonTerminalSymbol(
                    Symbols.MLCClosing,
                    star.Symbol,
                    fslash.Symbol));

                return true;
            }
            else return Fail(error, buffer, position, out result);
        }
        #endregion

        #region Line Comment
        internal static bool TryParseLineComment(Buffer buffer, out ParseResult result)
        {
            var position = buffer.Cursor;
            var components = new List<ISymbol>();
            var error = $"{Symbols.LineComment} expected at position: {position + 1}";

            //opening
            if (TryParseLCOpening(buffer, out var opening))
                components.Add(opening.Symbol);

            else return Fail(error, buffer, position, out result);

            //characters
            while (buffer.TryNext(out var @char))
            {
                if ((@char == '\n' || @char == '\r'))
                {
                    buffer.Back();
                    TryParseNewLine(buffer, out var end);
                    components.Add(end.Symbol);

                    result = new ParseResult(new NonTerminalSymbol(
                        Symbols.LineComment,
                        components));
                    return true;
                }

                //else
                components.Add(new TerminalSymbol(
                    Symbols.Character,
                    @char));
            }

            //input ended on this line, without encountering the new line, so we add it outselves
            components.Add(new TerminalSymbol(
                Symbols.NewLine,
                '\n'));

            result = new ParseResult(new NonTerminalSymbol(
                Symbols.LineComment,
                components));
            return true;
        }

        internal static bool TryParseLCOpening(Buffer buffer, out ParseResult result)
        {
            var position = buffer.Cursor;
            var components = new List<ISymbol>();
            var error = $"{Symbols.LCOpening} expected at position: {position + 1}";

            if (TryParseFSlash(buffer, out var slash1)
                && TryParseFSlash(buffer, out var slash2))
            {
                components
                    .Append(slash1.Symbol)
                    .Append(slash2.Symbol);

                result = new ParseResult(new NonTerminalSymbol(
                    Symbols.LCOpening,
                    components));
                return true;
            }
            else
            {
                return Fail(error, buffer, position, out result);
            }
        }

        internal static bool TryParseLCClosing(Buffer buffer, out ParseResult result)
        {
            var position = buffer.Cursor;
            var error = $"{Symbols.LCOpening} expected at position: {position + 1}";

            if (TryParseNewLine(buffer, out var newLine))
            {
                result = new ParseResult(new NonTerminalSymbol(
                    Symbols.LCOpening,
                    newLine.Symbol));
                return true;
            }
            else
            {
                return Fail(error, buffer, position, out result);
            }
        }
        #endregion

        #region String
        internal static bool TryParseString(Buffer buffer, out ParseResult result)
        {
            var position = buffer.Cursor;
            var components = new List<ISymbol>();
            var error = $"{Symbols.String} expected at position: {position + 1}";

            //compulsory opening $" | @" | "
            if (TryParseInterpolationStringOpening(buffer, out var opening)
                || TryParseLiteralStringOpening(buffer, out opening)
                || TryParseStringOpening(buffer, out opening))
                components.Add(opening.Symbol);

            else return Fail(error, buffer, position, out result);

            //the string body
            while (buffer.TryNext(out var @char))
            {
                if (@char == '"')
                {
                    if (opening.Symbol.Name == Symbols.LiteralStringOpening.ToString()
                        && TryParseQuote(buffer, out var quote))
                    {
                        components.Add(new NonTerminalSymbol(
                            Symbols.LiteralStringEscape,
                            quote.Symbol,
                            new TerminalSymbol(
                                Symbols.Quote,
                                @char)));
                        continue;
                    }

                    components.Add(new TerminalSymbol(
                        Symbols.Quote,
                        @char));

                    result = new ParseResult(new NonTerminalSymbol(
                        Symbols.String,
                        components));
                    return true;
                }
                else if (@char == '\\')
                {
                    if (opening.Symbol.Name != Symbols.LiteralStringOpening.ToString()
                        && TryParseQuote(buffer, out var quote))
                    {
                        components.Add(new NonTerminalSymbol(
                            Symbols.LiteralStringEscape,
                            new TerminalSymbol(
                                Symbols.BSlash,
                                @char),
                            quote.Symbol));
                        continue;
                    }
                }

                components.Add(new TerminalSymbol(
                    Symbols.Character,
                    @char));
            }

            //end of input with no string termination.
            //terminate the string
            components.Add(new TerminalSymbol(
                Symbols.Quote,
                '"'));

            result = new ParseResult(new NonTerminalSymbol(
                Symbols.String,
                components,
                true));
            return true;
        }

        internal static bool TryParseStringOpening(Buffer buffer, out ParseResult result)
        {
            var position = buffer.Cursor;
            var components = new List<ISymbol>();
            var error = $"{Symbols.StringOpening} expected at position: {position + 1}";

            if (TryParseQuote(buffer, out var quote))
            {
                components.Append(quote.Symbol);

                result = new ParseResult(new NonTerminalSymbol(
                    Symbols.StringOpening,
                    components));

                return true;
            }

            return Fail(error, buffer, position, out result);
        }

        internal static bool TryParseStringClosing(Buffer buffer, out ParseResult result)
        {
            var position = buffer.Cursor;
            var components = new List<ISymbol>();
            var error = $"{Symbols.StringClosing} expected at position: {position + 1}";

            if (TryParseQuote(buffer, out var quote))
            {
                components.Append(quote.Symbol);

                result = new ParseResult(new NonTerminalSymbol(
                    Symbols.StringClosing,
                    components));

                return true;
            }

            return Fail(error, buffer, position, out result);
        }

        internal static bool TryParseStringEscape(Buffer buffer, out ParseResult result)
        {
            var position = buffer.Cursor;
            var components = new List<ISymbol>();
            var error = $"{Symbols.StringEscape} expected at position: {position + 1}";

            if (TryParseBSlash(buffer, out var bslash)
                && TryParseCharacter(buffer, out var charResult)
                && charResult.Symbol is TerminalSymbol ts
                && ts.ToString() == "n")
            {
                components
                    .Append(bslash.Symbol)
                    .Append(charResult.Symbol);

                result = new ParseResult(new NonTerminalSymbol(
                    Symbols.StringEscape,
                    components));

                return true;
            }

            return Fail(error, buffer, position, out result);
        }

        internal static bool TryParseLiteralStringOpening(Buffer buffer, out ParseResult result)
        {
            var position = buffer.Cursor;
            var components = new List<ISymbol>();
            var error = $"{Symbols.LiteralStringOpening} expected at position: {position + 1}";

            if (TryParseAt(buffer, out var at) && TryParseQuote(buffer, out var quote))
            {
                components
                    .Append(at.Symbol)
                    .Append(quote.Symbol);

                result = new ParseResult(new NonTerminalSymbol(
                    Symbols.LiteralStringOpening,
                    components));

                return true;
            }

            return Fail(error, buffer, position, out result);
        }

        internal static bool TryParseLiteralStringEscape(Buffer buffer, out ParseResult result)
        {
            var position = buffer.Cursor;
            var components = new List<ISymbol>();
            var error = $"{Symbols.LiteralStringEscape} expected at position: {position + 1}";

            if (TryParseQuote(buffer, out var quote1) && TryParseQuote(buffer, out var quote2))
            {
                components
                    .Append(quote1.Symbol)
                    .Append(quote2.Symbol);

                result = new ParseResult(new NonTerminalSymbol(
                    Symbols.LiteralStringEscape,
                    components));

                return true;
            }

            return Fail(error, buffer, position, out result);
        }

        internal static bool TryParseInterpolationStringOpening(Buffer buffer, out ParseResult result)
        {
            var position = buffer.Cursor;
            var components = new List<ISymbol>();
            var error = $"{Symbols.InterpolationStringOpening} expected at position: {position + 1}";

            if (TryParseDollar(buffer, out var dollar) && TryParseQuote(buffer, out var quote))
            {
                components
                    .Append(dollar.Symbol)
                    .Append(quote.Symbol);

                result = new ParseResult(new NonTerminalSymbol(
                    Symbols.InterpolationStringOpening,
                    components));

                return true;
            }

            return Fail(error, buffer, position, out result);
        }
        #endregion

        #endregion

        #region Terminals

        internal static bool TryParseQuote(Buffer buffer, out ParseResult result)
            => TryParseSpecialCharacter(buffer, '"', Symbols.Quote, out result);

        internal static bool TryParseDollar(Buffer buffer, out ParseResult result)
            => TryParseSpecialCharacter(buffer, '$', Symbols.Dollar, out result);

        internal static bool TryParseAt(Buffer buffer, out ParseResult result)
            => TryParseSpecialCharacter(buffer, '@', Symbols.At, out result);

        internal static bool TryParseBSlash(Buffer buffer, out ParseResult result)
            => TryParseSpecialCharacter(buffer, '\\', Symbols.BSlash, out result);

        internal static bool TryParseFSlash(Buffer buffer, out ParseResult result)
            => TryParseSpecialCharacter(buffer, '/', Symbols.FSlash, out result);

        internal static bool TryParseStar(Buffer buffer, out ParseResult result)
            => TryParseSpecialCharacter(buffer, '*', Symbols.Star, out result);

        private static bool TryParseSpecialCharacter(
            Buffer buffer,
            char expectedCharacter,
            Symbols expectedSymbol,
            out ParseResult result)
        {
            var position = buffer.Cursor;

            if (buffer.TryNext(out var @char) && @char == expectedCharacter)
            {
                result = new ParseResult(new TerminalSymbol(
                    expectedSymbol,
                    @char));

                return true;
            }

            buffer.Reset(position);
            result = new ParseResult($"'{expectedCharacter}' expected at position: {position + 1}");

            return false;
        }

        /// <summary>
        /// This may not be needed
        /// </summary>
        internal static bool TryParseCharacter(Buffer buffer, out ParseResult result)
        {
            var position = buffer.Cursor;

            if (buffer.TryNext(out var @char))
            {
                result = new ParseResult(new TerminalSymbol(
                    Symbols.Character,
                    @char));

                return true;
            }

            buffer.Reset(position);
            result = new ParseResult($"A character was expected at position: {position + 1}");

            return false;
        }

        internal static bool TryParseWhitespace(Buffer buffer, out ParseResult result)
        {
            var position = buffer.Cursor;

            if (!buffer.TryNext(out var @char)
                || (@char != ' ' && @char != '\t'))
            {
                buffer.Reset(position);
                result = new ParseResult($"' ' expected at position: {position + 1}");
                return false;
            }

            //else
            result = new ParseResult(new TerminalSymbol(
                Symbols.Whitespace,
                @char));

            return true;
        }

        internal static bool TryParseNewLine(Buffer buffer, out ParseResult result)
        {
            var position = buffer.Cursor;

            if (!buffer.TryNext(out var char1)
                || (char1 != '\n' && char1 != '\r'))
            {
                buffer.Reset(position);
                result = new ParseResult($"'\n' expected at position: {position + 1}");
                return false;
            }

            buffer.TryNext(out var char2);

            if (char2 != '\n' && char2 != '\r')
                buffer.Back();

            result = new ParseResult(new TerminalSymbol(
                Symbols.NewLine,
                '\n'));

            return true;
        }

        #endregion

        #region helpers
        private static bool Fail(
            string message,
            Buffer buffer,
            int position,
            ParseResult? innerResult,
            out ParseResult result)
        {
            buffer.Reset(position);
            result = new ParseResult(message, innerResult);

            return false;
        }

        private static bool Fail(
            string message,
            Buffer buffer,
            int position,
            out ParseResult result)
            => Fail(message, buffer, position, null, out result);
        #endregion

        #region Extensions
        private static IList<T> Append<T>(this IList<T> list, T value)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            list.Add(value);
            return list;
        }
        #endregion
    }
}
