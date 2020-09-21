//using IronOcr;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BankOcr
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] lines = File.ReadAllLines(@"..\..\ocr1.txt");
            string[] results = BankOcrProcessor.Solution(lines);

            foreach (var result in results)
            {
                Console.WriteLine(result);
            }

            Console.ReadLine();
        }
	}
}
