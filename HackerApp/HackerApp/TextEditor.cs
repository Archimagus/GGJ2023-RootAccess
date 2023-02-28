using System.Text;

namespace TerminalApp
{
	internal class TextEditor
	{
		private string filePath;
		private List<StringBuilder> lines = new List<StringBuilder>();

		public TextEditor(string filePath)
		{
			this.filePath = filePath;
			if (File.Exists(filePath))
			{
				lines.AddRange(File.ReadAllLines(filePath).Select(s => new StringBuilder(s)));
			}
			else
			{
				lines.Add(new StringBuilder());
			}
			Console.SetCursorPosition(0, 0);
		}

		public void Run(bool root)
		{
			while (true)
			{
				Display();
				ConsoleKeyInfo keyInfo = Console.ReadKey(true);

				if (keyInfo.Key == ConsoleKey.Escape)
				{
					break;
				}

				switch (keyInfo.Key)
				{
					case ConsoleKey.UpArrow:
						MoveCursorUp();
						break;

					case ConsoleKey.DownArrow:
						MoveCursorDown();
						break;

					case ConsoleKey.LeftArrow:
						MoveCursorLeft();
						break;

					case ConsoleKey.RightArrow:
						MoveCursorRight();
						break;

					case ConsoleKey.Enter:
						InsertLine();
						break;

					case ConsoleKey.Backspace:
						BackspaeCharacter();
						break;

					case ConsoleKey.Delete:
						DeleteCharacter();
						break;

					default:
						if (char.IsLetterOrDigit(keyInfo.KeyChar) || char.IsPunctuation(keyInfo.KeyChar) || keyInfo.KeyChar == ' ')
						{
							InsertCharacter(keyInfo.KeyChar);
						}

						break;
				}
			}
			Console.Clear();
			Console.WriteLine("Save? (Y/N)");
			if (Console.ReadLine()?.ToUpper() == "Y")
			{
				if (root)
				{
					File.WriteAllLines(filePath, lines.Select(sb => sb.ToString()));
				}
				else
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("Isufficient Permissions, must be root to write files.  Try <sudo>");
					Console.ResetColor();
				}
			}
		}

		private void Display()
		{
			var left = Console.CursorLeft;
			var top = Console.CursorTop;
			Console.Clear();
			for (int i = 0; i < lines.Count; i++)
			{
				Console.WriteLine(lines[i]);
			}
			Console.CursorTop = Console.WindowHeight - 1;
			Console.CursorLeft = 0;
			Console.WriteLine("<exc> to exit                                                         ");
			Console.CursorLeft = left;
			Console.CursorTop = top;
		}

		private void MoveCursorUp()
		{
			if (Console.CursorTop > 0)
				Console.CursorTop--;
			Console.CursorLeft = Math.Min(Console.CursorLeft, lines[Console.CursorTop].Length);
		}

		private void MoveCursorDown()
		{
			if (lines.Count > Console.CursorTop)
				Console.CursorTop++;
			Console.CursorLeft = Math.Min(Console.CursorLeft, lines[Console.CursorTop].Length);
		}

		private void MoveCursorLeft()
		{
			if (Console.CursorLeft > 0)
				Console.CursorLeft--;
		}

		private void MoveCursorRight()
		{
			if (Console.CursorLeft < lines[Console.CursorTop].Length)
			{
				Console.CursorLeft++;
			}
		}

		private void InsertLine()
		{
			var curLine = lines[Console.CursorTop];
			StringBuilder newLine = new StringBuilder();
			if (Console.CursorLeft < curLine.Length)
			{
				newLine.Append(curLine.Remove(Console.CursorLeft, curLine.Length - Console.CursorLeft));
			}
			if (Console.CursorTop == lines.Count)
			{
				lines.Add(newLine);
			}
			else
			{
				lines.Insert(Console.CursorTop + 1, newLine);
			}
			Console.CursorTop++;
			Console.CursorLeft = 0;
		}

		private void BackspaeCharacter()
		{
			int cursorLeft = Console.CursorLeft - 1;
			if (cursorLeft == 0 && Console.CursorTop > 0)
			{
				lines.RemoveAt(Console.CursorTop);
				Console.CursorTop--;
				Console.CursorLeft = lines[Console.CursorTop].Length;
			}
			else if (cursorLeft >= 0)
			{
				var line = lines[Console.CursorTop];
				line.Remove(cursorLeft, 1);
				Console.CursorLeft--;
			}
		}

		private void DeleteCharacter()
		{
			if (Console.CursorLeft >= lines[Console.CursorTop].Length)
			{
				if (lines.Count > Console.CursorTop + 1)
				{
					lines[Console.CursorTop].Append(lines[Console.CursorTop + 1]);
					lines.RemoveAt(Console.CursorTop + 1);
				}
			}
			else
			{
				lines[Console.CursorTop].Remove(Console.CursorLeft, 1);
			}
		}

		private void InsertCharacter(char character)
		{
			var line = lines[Console.CursorTop];
			int cursorLeft = Console.CursorLeft;
			line.Insert(cursorLeft, character);
			Console.CursorLeft++;
		}
	}
}