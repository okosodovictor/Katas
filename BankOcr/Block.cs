using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankOcr
{
    public class Block
    {
        public char[,] Matrix { get; } = new char[3, 3];
        public char? Value { get; }

        public override bool Equals(object obj)
            => obj is Block other && MatrixEquals(other.Matrix);

        public override int GetHashCode()
        {
            return Matrix
                .Cast<char>()
                .Aggregate(19, (hash, next) => hash * 181 + next.GetHashCode());
        }


        public bool MatrixEquals(char[,] matrix)
        {
            if (matrix.Rank != 2 || matrix.Length != 9)
                return false;

            //compare
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    if (Matrix[row, col] != matrix[row, col])
                        return false;
                }
            }
            return true;
        }

        public Block()
        { }

        private Block(char value, char[,] matrix)
        {
            Matrix = matrix;
            Value = value;
        }

        #region Static values
        public static Block Match(Block block)
            => Digits.FirstOrDefault(digit => digit.Equals(block));

        private static readonly Block[] Digits = new[]
        {
            //0
			new Block(
                value: '0',
                matrix: new char[3, 3]
                {
                    {' ', '_', ' ' },
                    { '|', ' ', '|'},
                    { '|', '_', '|'}
                }),

			//1
			new Block(
                value: '1',
                matrix: new char[3, 3]
                {
                    {' ', ' ', ' ' },
                    { ' ', ' ', '|'},
                    { ' ', ' ', '|'}
                }),

			//2
			new Block(
                value: '2',
                matrix: new char[3, 3]
                {
                    {' ', '_', ' ' },
                    { ' ', '_', '|'},
                    { '|', '_', ' '}
                }),

			//3
			new Block(
                value: '3',
                matrix: new char[3, 3]
                {
                    {' ', '_', ' ' },
                    { ' ', '_', '|'},
                    { ' ', '_', '|'}
                }),

			//4
			new Block(
                value: '4',
                matrix: new char[3, 3]
                {
                    {' ', ' ', ' ' },
                    { '|', '_', '|'},
                    { ' ', ' ', '|'}
                }),
			//5
			new Block(
                value: '5',
                matrix: new char[3, 3]
                {
                    {' ', '_', ' ' },
                    { '|', '_', ' '},
                    { ' ', '_', '|'}
                }),
			
			//6
			new Block(
                value: '6',
                matrix: new char[3, 3]
                {
                    {' ', '_', ' ' },
                    { '|', '_', ' '},
                    { '|', '_', '|'}
                }),

			//7
			new Block(
                value: '7',
                matrix: new char[3, 3]
                {
                    {' ', '_', ' ' },
                    { ' ', ' ', '|'},
                    { ' ', ' ', '|'}
                }),
			
			//8
			new Block(
                value: '8',
                matrix: new char[3, 3]
                {
                    {' ', '_', ' ' },
                    { '|', '_', '|'},
                    { '|', '_', '|'}
                }),
			//9
			new Block(
                value: '9',
                matrix: new char[3, 3]
                {
                    {' ', '_', ' ' },
                    { '|', '_', '|'},
                    { ' ', '_', '|'}
                }),
        };
        #endregion
    }
}
