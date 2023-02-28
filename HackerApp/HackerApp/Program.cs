using System.Reflection;

namespace TerminalApp
{
	internal class Program
	{
		private static string _startupFolder;
		private static Random _random = Random.Shared;

		private static void Main(string[] args)
		{
			try
			{
				var assembly = Assembly.GetExecutingAssembly();
				var stream = assembly.GetManifestResourceStream(assembly.GetManifestResourceNames()[0]);
				_startupFolder = Environment.CurrentDirectory;

				var tempPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
				var tempDir = Path.Combine(tempPath, "GGJHackerApp");
				AppDomain.CurrentDomain.ProcessExit += (object? sender, EventArgs e) => DeleteDirectory(tempDir);

				CreateStructure(tempDir, stream);

				new Terminal(tempDir).Run();
			}
			catch (Exception exc)
			{
				Console.WriteLine(exc.ToString());
				Console.ReadKey();
			}
		}

		private static string[] _dirs = new string[]
		{
			"left",
			"right",
			"up",
			"down",
			"straight",
		};

		private static string[] _names = new string[]
		{
			"wall",
			"floor",
			"ceiling",
			"sign",
			"door",
			"random",
			"trash",
			"blue",
			"jesse",
			"kendra",
			"joe",
			"GGJ",
			"waldo",
			"bazinga",
			"brief",
			"familiar",
			"spiffy",
			"elated",
			"best",
			"sweltering",
			"energetic",
			"plant",
			"shaggy",
			"actually",
			"drab",
			"exclusive",
		};

		private static string[] _extensions = new string[]
		{
			".txt",
			".pwd",
			".foo",
			".bar",
			".baz",
			".rofl",
			".wow",
			".code",
			".level",
			".hhh",
		};

		private static void CreateStructure(string path, Stream contents)
		{
			Directory.CreateDirectory(path);
			string currentPath = path;
			var sr = new StreamReader(contents);
			while (!sr.EndOfStream)
			{
				var line = sr.ReadLine();
				if (line == null) break;
				// If Directory
				if (line.EndsWith('/'))
				{
					//var dir = line.Trim('├', '└', '─', '/', ' ');
					var dir = line.Trim('/', ' ');
					currentPath = Path.Combine(currentPath, dir);
					recursiveCreateDirectories(currentPath, _random.Next(2, 6));
				}
				else if (line.Length > 0)
				{
					var file = line.Trim('├', '─', '/', ' ');
					var fileName = new string(file.TakeWhile(x => x != '-').ToArray()).ToLower();
					var fileContents = new string(file.SkipWhile(x => x != '-').Skip(1).ToArray());
					var filePath = Path.Combine(currentPath, fileName + _extensions[_random.Next(0, _extensions.Length)]);
					File.WriteAllText(filePath, fileContents);
				}
			}
		}

		private static void recursiveCreateDirectories(string currentPath, int depth)
		{
			if (depth < 0) return;

			Directory.CreateDirectory(currentPath);
			// Create some fake files
			for (int i = 0; i < _random.Next(1, 4); i++)
			{
				File.WriteAllText(Path.Combine(currentPath,
					$"{_names[_random.Next(0, _names.Length)]}{_random.Next(100, 1000)}{_extensions[_random.Next(0, _extensions.Length)]}"),
					$"{_random.Next(1000, 10000)}");
			}

			int index = _random.Next(0, _dirs.Length);
			var name = _dirs[index];
			recursiveCreateDirectories(Path.Combine(currentPath, name), depth - 1);
		}

		public static void DeleteDirectory(string targetDir)
		{
			Directory.SetCurrentDirectory(_startupFolder);
			if (!Directory.Exists(targetDir)) return;

			string[] files = Directory.GetFiles(targetDir);
			string[] dirs = Directory.GetDirectories(targetDir);

			foreach (string file in files)
			{
				File.SetAttributes(file, FileAttributes.Normal);
				File.Delete(file);
			}

			foreach (string dir in dirs)
			{
				DeleteDirectory(dir);
			}

			Directory.Delete(targetDir, false);
		}
	}
}