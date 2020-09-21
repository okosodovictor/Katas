using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankOcr
{
    public static class BankOcrProcessor
    {
		public static string[] Solution(string[] lines)
		{
			var blockLines = ExtractBlockLines(lines);

			var output = blockLines
				.Select(Convert)
				.ToArray();

			return output;
		}

		public static IEnumerable<IEnumerable<Block>> ExtractBlockLines(string[] lines)
		{
			var blockLines = new List<List<Block>>();

			for (int row = 0; row < lines.Length; row++)
			{
				//skip line between blocks
				var blockRowIndex = Math.DivRem(row + 1, 4, out var remainder);
				if (remainder == 0)
					continue;

				var blockRow = blockRowIndex >= blockLines.Count
					? blockLines.AppendAndGet(new List<Block>())
					: blockLines[blockRowIndex];

				for (int col = 0; col < lines[row].Length; col++)
				{
					var blockColIndex = col / 3;
					var block = blockColIndex >= blockRow.Count
						? blockRow.AppendAndGet(new Block())
						: blockRow[blockColIndex];


					//populate block
					var blockCharRow = row % 4;
					var blockCharCol = col % 3;
					block.Matrix[blockCharRow, blockCharCol] = lines[row][col];
				}
			}

			return blockLines;
		}

		public static string Convert(IEnumerable<Block> blocks)
		{
			var sb = new StringBuilder();
			foreach (var block in blocks)
			{
				var matched = Block.Match(block);

				if (matched == null)
					sb.Append("\nError in data\n");
                else
					sb.Append(matched.Value);	
            }

			return sb.ToString();
		}
	}

	public static class Extensions
	{
		public static T AppendAndGet<T>(this List<T> list, T value)
		{
			if (list == null)
				throw new ArgumentNullException(nameof(list));

			list.Add(value);
			return value;
		}
	}
}
