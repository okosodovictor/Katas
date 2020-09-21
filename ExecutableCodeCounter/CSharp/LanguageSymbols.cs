using System;
using System.Collections.Generic;
using System.Text;

namespace ExecutableCodeCounter.CSharp
{
	public enum LanguageSymbols
	{
		Executable,

		LineComment,
		LCOpening,
		LCClosing,

		MultiLineComment,
		MLCOpening,
		MLCClosing,

		PseudoExpression,

		String,
		StringOpening,
		StringClosing,
		StringEscape, // --> \n

		InterpolationStringOpening,

		LiteralStringOpening,
		LiteralStringEscape, // --> ""

		At,
		Dollar,
		Star,
		Quote,
		BSlash,
		FSlash,
		Character,
		Whitespace,
		NewLine
	}
}
