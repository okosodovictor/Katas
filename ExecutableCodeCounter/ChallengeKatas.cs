using ExecutableCodeCounter.CSharp;
using ExecutableCodeCounter.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExecutableCodeCounter
{
    public static class ChallengeKatas
    {
        public static Result? Solution(string code)
        {
            //split code by line
            var lines = code
                .Replace("\r\n", "\n\0")
                .Split(new[] { '\0' });

            //parse each line separately
            var parsedLines = lines
                .Aggregate(new List<ParseResult?>(), (acc, nextLine) =>
                {
                    var prev = acc.LastOrDefault()?.Symbol as NonTerminalSymbol;
                    var lastSymbol = prev?.Symbols.LastOrDefault() as NonTerminalSymbol;

                    if (lastSymbol?.IsSpecialTermination == true)
                    {
                        if (lastSymbol.NameIs(LanguageSymbols.MultiLineComment))
                            nextLine = $"/*{nextLine}";

                        else if (lastSymbol.NameIs(LanguageSymbols.String))
                        {
                            if (lastSymbol.Symbols.First().NameIs(LanguageSymbols.StringOpening))
                                nextLine = $"\"{nextLine}";

                            else if (lastSymbol.Symbols.First().NameIs(LanguageSymbols.LiteralStringOpening))
                                nextLine = $"@\"{nextLine}";

                            else if (lastSymbol.Symbols.First().NameIs(LanguageSymbols.InterpolationStringOpening))
                                nextLine = $"$\"{nextLine}";
                        }
                    }

                    acc.Add(CSharp.Parser.ParseCSharp(nextLine));
                    return acc;
                });

            //every line should parse
            if (parsedLines.Any(r => r?.IsErrorResult == true))
                return null;

            //executable lines
            var execLines = parsedLines.Aggregate(0, (acc, nextResult) =>
            {
                var symbol = nextResult.Value.Symbol as NonTerminalSymbol;

                var count =
                    symbol.GetSymbols(LanguageSymbols.PseudoExpression.ToString()).Count()
                    + symbol.GetSymbols(LanguageSymbols.String.ToString()).Count();

                return acc + (count > 0 ? 1 : 0);
            });

            //non executable lines
            var nonExecLines = parsedLines.Aggregate(0, (acc, nextResult) =>
            {
                var symbol = nextResult.Value.Symbol as NonTerminalSymbol;

                var count =
                    symbol.GetSymbols(LanguageSymbols.PseudoExpression.ToString()).Count()
                    + symbol.GetSymbols(LanguageSymbols.String.ToString()).Count();

                return acc + (count == 0 ? 1 : 0);
            });

            return new Result(
                Convert.ToUInt32(execLines),
                Convert.ToUInt32(nonExecLines));
        }
    }

    public struct Result
    {
        public uint ExecutableLineCount { get; }

        public uint NonExecutableLineCount { get; }

        public Result(uint execCount, uint nonExecCount)
        {
            ExecutableLineCount = execCount;
            NonExecutableLineCount = nonExecCount;
        }
    }

    public static class Extensions
    {

        public static bool NameIs(this ISymbol symbol, params object[] names)
        {
            if (symbol == null)
                throw new ArgumentNullException(nameof(symbol));

            return names
                .Select(name => name?.ToString())
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Any(name => symbol.Name.Equals(name));
        }
    }
}
