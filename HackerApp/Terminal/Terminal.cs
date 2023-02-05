using System.Text;

namespace TerminalApp
{
	internal class Terminal
	{
		private string currentDirectory;
		private string rootDirectory;
		private string user = "user";
		private bool isRoot = false;
		private int windowWidth = 0;
		private string clearLineString = "";
		private string rootPassword = "HU GGJ-2023";
		private bool running = true;
		private string input;
		private StringBuilder tempInput = new StringBuilder();
		private int tabCount = 0;
		private List<string> commandHistory = new List<string>();

		private Dictionary<string, Action<Terminal, string[]>> commands = new Dictionary<string, Action<Terminal, string[]>>
		{
			{"cls", (t,p) => Console.Clear() },
			{"exit", (t,p) => t.running = false },
			{"ls", (t,p) => {
					IOrderedEnumerable<(string type, string path)> paths = t.getPaths();
					foreach (var dir in paths)
					{
						Console.ForegroundColor = dir.type == "d" ? ConsoleColor.Yellow : ConsoleColor.DarkGreen;
						Console.WriteLine(dir.path);
					}
					Console.ResetColor();
				}
			},

			{ "cd", (t,inputParts) => {
				if (inputParts.Length > 1)
				{
					string newDirectory = inputParts[1];
					newDirectory = newDirectory.Replace("~", t.rootDirectory);
					try
					{
						if (!Directory.Exists(newDirectory))
						{
							throw new DirectoryNotFoundException();
						}
						var tempDir = Path.GetFullPath(newDirectory);

						if (!tempDir.StartsWith(t.rootDirectory))
						{
							throw new DirectoryNotFoundException();
						}

						t.currentDirectory = tempDir;
						Directory.SetCurrentDirectory(t.currentDirectory);
					}
					catch
					{
						Console.ForegroundColor = ConsoleColor.Red;
						Console.WriteLine("Error: Directory does not exist");
						Console.ResetColor();
					}
				}
			}
			},
			{"edit",(t,inputParts) => {
					if (inputParts.Length != 2)
					{
						Console.WriteLine("Usage: edit <file>");
						return;
					}
					var te = new TextEditor(inputParts[1]);
					te.Run(t.isRoot);
					Console.Clear();
				}
			},
			{"sudo", (t,p) => {
				StringBuilder tempInput = new StringBuilder();
				Console.WriteLine("Enter Root Password");
				Console.Write(":");
				while (true)
				{
					var key = Console.ReadKey(true);
					if (key.Key == ConsoleKey.Enter)
					{
						Console.WriteLine();
						if(tempInput.ToString() == t.rootPassword)
						{
							t.user = "root";
							t.isRoot = true;
						}
						else
						{
							Console.ForegroundColor = ConsoleColor.Red;
							Console.WriteLine("Invalid Password");
							Console.ResetColor();
						}
						if(p.Length > 1)
						{
							var segment = new ArraySegment<string>( p, 1, p.Length-1 );
							if(t.commands.TryGetValue(segment[0], out var command))
							{
								command(t, p.Skip(1).ToArray());
							}
						}
						return;
					}

					switch (key.Key)
					{
						case ConsoleKey.Backspace:
							if (tempInput.Length > 0)
							{
								tempInput.Remove(tempInput.Length - 1, 1);
							}
							break;

						default:
							if (char.IsLetterOrDigit(key.KeyChar) || char.IsPunctuation(key.KeyChar) || key.KeyChar == ' ')
							{
								tempInput.Append(key.KeyChar);
							}
							break;
					}
					}
				}
			},
			{"help", (t, inputParts) =>
				{
					Console.WriteLine("********** HackerApp++ *************");
					Console.WriteLine("Commands:");
					Console.WriteLine(string.Join(" ", t.commands.Select(c=>c.Key)));
				}
			},
		};

		private int commandHistoryIndex = -1;

		public Terminal(string rootDir)
		{
			currentDirectory = rootDirectory = rootDir;
			Directory.SetCurrentDirectory(currentDirectory);
		}

		public void Run()
		{
			commands["help"](this, new string[0]);

			writeCurrentPath();
			while (running)
			{
				while (running)
				{
					var key = Console.ReadKey(true);
					if (key.Key == ConsoleKey.Enter)
					{
						Console.WriteLine();
						input = tempInput.ToString();
						tempInput.Clear();
						tabCount = -1;
						if (commandHistory.Count == 0)
						{
							commandHistory.Add(input);
							commandHistoryIndex++;
						}
						if (commandHistory[commandHistoryIndex] != input)
						{
							commandHistory.Insert(commandHistoryIndex, input);
							commandHistoryIndex++;
						}
						break;
					}

					switch (key.Key)
					{
						case ConsoleKey.Tab:
							autoComplete();
							break;

						case ConsoleKey.Backspace:
							if (tempInput.Length > 0)
							{
								tempInput.Remove(tempInput.Length - 1, 1);
							}
							input = tempInput.ToString();
							tabCount = -1;
							break;

						case ConsoleKey.UpArrow:
							{
								tempInput.Clear();
								if (commandHistory.Count > 0)
								{
									commandHistoryIndex--;
									if (commandHistoryIndex < 0)
										commandHistoryIndex = commandHistory.Count - 1;
									tempInput.Append(commandHistory[commandHistoryIndex]);
								}
							}
							break;

						case ConsoleKey.DownArrow:
							{
								tempInput.Clear();
								if (commandHistory.Count > 0)
								{
									commandHistoryIndex = (++commandHistoryIndex) % (commandHistory.Count - 1);
									tempInput.Append(commandHistory[commandHistoryIndex]);
								}
							}
							break;

						default:
							if (key.KeyChar == ' ' ||
								key.KeyChar == '~' ||
								char.IsLetterOrDigit(key.KeyChar) ||
								char.IsPunctuation(key.KeyChar) ||
								char.IsPunctuation(key.KeyChar))
							{
								tempInput.Append(key.KeyChar);
								input = tempInput.ToString();
								tabCount = -1;
							}
							break;
					}

					ClearLine();
					writeCurrentPath();
					Console.Write(tempInput.ToString());
				}
				if (string.IsNullOrWhiteSpace(input)) continue;

				string[] inputParts = input.Split(' ');
				string command = inputParts[0];

				if (commands.TryGetValue(command, out var cmd))
				{
					cmd(this, inputParts);
				}
				else
				{
					Console.WriteLine($"{command} is not a valid command.  See <help>.");
				}

				writeCurrentPath();
			}
		}

		private void autoComplete()
		{
			if (input == null)
				return;
			string[] inputParts = input.Split(' ');
			string command = inputParts[0];

			if (inputParts.Length == 1)
			{
				var possibleCommands = commands.Keys.Where(p => p.StartsWith(command)).ToArray();
				if (possibleCommands.Any())
				{
					tabCount = ++tabCount % possibleCommands.Length;
					tempInput.Clear().Append(possibleCommands[tabCount]);
				}
				return;
			}

			if (command == "cd" || command == "edit")
			{
				var paths = getPaths().Where(p => (command == "cd" ? p.type == "d" : p.type == "f") && p.path.StartsWith(inputParts.Last())).ToArray();
				if (paths.Any())
				{
					tabCount = ++tabCount % paths.Length;
					tempInput.Clear().Append($"{command} {paths[tabCount].path}");
				}
			}
		}

		private void ClearLine()
		{
			if (Console.WindowWidth != windowWidth)
			{
				windowWidth = Console.WindowWidth;
				clearLineString = new string(' ', windowWidth);
			}
			int currentLineCursor = Console.CursorTop;
			Console.SetCursorPosition(0, Console.CursorTop);
			Console.Write(clearLineString);
			Console.SetCursorPosition(0, currentLineCursor);
		}

		private void writeCurrentPath()
		{
			Console.ForegroundColor = isRoot ? ConsoleColor.Magenta : ConsoleColor.Green;
			Console.Write(user);
			Console.ForegroundColor = ConsoleColor.White;
			Console.Write($"@{currentDirectory.Substring(rootDirectory.Length).Replace('\\', '/')}/");
			Console.ResetColor();
		}

		private IOrderedEnumerable<(string type, string path)> getPaths()
		{
			var directories = Directory.GetDirectories(currentDirectory).Select(f => (type: "d", path: Path.GetRelativePath(currentDirectory, f)));
			var files = Directory.GetFiles(currentDirectory).Select(f => (type: "f", path: Path.GetFileName(f)));

			var paths = directories.Concat(files).OrderBy(p => p.path);
			return paths;
		}
	}
}