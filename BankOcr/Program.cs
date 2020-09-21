//using IronOcr;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BankOcr
{
    class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadAllLines(@"..\..\ocr1.txt");
            var matrixArray = lines.Select(l => l.ToArray()).ToArray();

            var blockLines = new List<List<Block>>();
            int rowCount = 0;
            for (int row = 0; row < lines.Length; row++)
            {
                var line = lines[row];
                int colCount = 0;
                for (int col = 0; col < line.Length; col++)
                {

                }
            }

            Console.ReadLine();
        }
        public class Block
        {
            public char[] Top { get; } = new char[3];

            public char[] Middle { get; } = new char[3];

            public char[] Bottom { get; } = new char[3];

            public char Value { get; set; }

            public Block()
            { }

            public Block(char[,] arrayf, char value)
            {

            }

            public static readonly Block Zero = new Block(
              new char[3, 3]
              {
                {' ', '_', ' ' },
                { '|', ' ', '|'},
                { '|', '_', '|'}
              },
              '0');

            public static readonly Block One = new Block(
               new char[3, 3]
               {
                {' ', ' ', ' ' },
                { ' ', '|', ' '},
                { ' ', '|', ' '}
               },
               '1');

            public static readonly Block Two = new Block(
                new char[3, 3]
                {
                {' ', '_', ' ' },
                { ' ', '_', '|'},
                { '|', '_', ' '}
                },
                '2');

            public static readonly Block Three = new Block(
                new char[3, 3]
                {
                {' ', '_', ' ' },
                { ' ', '_', '|'},
                { ' ', '_', '|'}
                },
                '3');

            public static readonly Block Four = new Block(
                new char[3, 3]
                {
                {' ', ' ', ' ' },
                { '|', '_', '|'},
                { ' ', ' ', '|'}
                },
                '4');
            public static readonly Block Five = new Block(
                new char[3, 3]
                {
                {' ', '_', ' ' },
                { '|', '_', ' '},
                { ' ', '_', '|'}
                },
                '5');
            public static readonly Block Six = new Block(
                new char[3, 3]
                {
                {' ', '_', ' ' },
                { '|', '_', ' '},
                { '|', '_', '|'}
                },
                '6');

            public static readonly Block Seven = new Block(
                new char[3, 3]
                {
                {' ', '_', ' ' },
                { ' ', ' ', '|'},
                { ' ', ' ', '|'}
                },
                '7');

            public static readonly Block Eight = new Block(
                new char[3, 3]
                {
                {' ', '_', ' ' },
                { '|', '_', '|'},
                { '|', '_', '|'}
                },
                '8');
            public static readonly Block Nine = new Block(
                new char[3, 3]
                {
                {' ', '_', ' ' },
                { '|', '_', '|'},
                { ' ', '_', '|'}
                },
                '9');
        }
    }
}
