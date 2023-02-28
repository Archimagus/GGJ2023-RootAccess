using System.IO;
using System.IO.Compression;
using System.Linq;
using UnityEditor;
using UnityEngine;

internal class BatchBuilder
{
	[MenuItem("Tools/Batch Build")]
	public static void BuildGame()
	{
		var productName = new string(PlayerSettings.productName.Skip("GGJ2023-".Length).ToArray());

		if (EditorUtility.DisplayDialog($"Batch Build {productName}?", "Batch build for\nWebGL\nWindows\nMax\nLinux", "OK", "Cancel"))
		{
			var levels = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray();

			string path = $"Build/{productName} WebGL";
			Debug.Log($"<Color=red>Building WebGL</color> {path}");
			BuildPipeline.BuildPlayer(levels, path, BuildTarget.WebGL, BuildOptions.None);
			Zip(path);

			path = $"Build/{productName} Windows/{productName}.exe";
			Debug.Log($"<Color=red>Building Windows</color> {path}");
			BuildPipeline.BuildPlayer(levels, path, BuildTarget.StandaloneWindows, BuildOptions.None);
			Zip(Path.GetDirectoryName(path));

			path = $"Build/{productName} Mac.app";
			Debug.Log($"<Color=red>Building Mac</color> {path}");
			BuildPipeline.BuildPlayer(levels, path, BuildTarget.StandaloneOSX, BuildOptions.None);
			Zip(path);

			path = $"Build/{productName} Linux/{productName}.x64";
			Debug.Log($"<Color=red>Building Linux</color> {path}");
			BuildPipeline.BuildPlayer(levels, path, BuildTarget.StandaloneLinux64, BuildOptions.None);
			Zip(Path.GetDirectoryName(path));

			//path = $"Build/{productName} Android.apk";
			//Debug.Log($"<Color=red>Building Android</color> {path}");
			//BuildPipeline.BuildPlayer(levels, path, BuildTarget.Android, BuildOptions.None);

			Debug.Log($"<Color=red>Build Complete</color>");
		}
	}

	private static void Zip(string path)
	{
		var dataPath = Path.GetDirectoryName(Application.dataPath.Replace('/', '\\'));
		var toZip = Path.Combine(dataPath, path.Replace('/', '\\'));
		var zipPath = Path.ChangeExtension(toZip, ".zip");
		Debug.Log($"<Color=red>Zipping to {zipPath}</color>");
		if (File.Exists(zipPath))
			File.Delete(zipPath);
		ZipFile.CreateFromDirectory(toZip, zipPath, System.IO.Compression.CompressionLevel.Optimal, true);
	}
}