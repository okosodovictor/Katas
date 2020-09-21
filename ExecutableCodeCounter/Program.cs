using ExecutableCodeCounter.Parser;
using System;
using System.IO;
using System.Linq;

namespace ExecutableCodeCounter
{
	public static class Program
    {
        static void Main(string[] args)
        {
            string lines = File.ReadAllText(@"..\..\..\SampleCode1.txt");
            var result = ChallengeKatas.Solution(lines);
            if (result != null)
            {
                Console.WriteLine($"Executable Line Count: {result?.ExecutableLineCount}");
                Console.WriteLine($"Non Executable Line Count: {result?.NonExecutableLineCount}");
            }

            Console.ReadLine();
        }		
	}
}
