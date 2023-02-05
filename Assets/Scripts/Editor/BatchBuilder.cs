using System.IO;
using System.IO.Compression;
using System.Linq;
using UnityEditor;
using UnityEngine;

class BatchBuilder
{
	[MenuItem("Tools/Batch Build")]
	public static void BuildGame()
	{
		if (EditorUtility.DisplayDialog("Batch Build", "Batch build for\nWebGL\nWindows\nMax\nLinux\nAndroid", "OK", "Cancel"))
		{
			var levels = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray();

			string path = $"Build/{PlayerSettings.productName} WebGL";
			Debug.Log($"<Color=red>Building WebGL</color> {path}");
			BuildPipeline.BuildPlayer(levels, path, BuildTarget.WebGL, BuildOptions.None);
			Zip(path);

			path = $"Build/{PlayerSettings.productName} Windows/{PlayerSettings.productName}.exe";
			Debug.Log($"<Color=red>Building Windows</color> {path}");
			BuildPipeline.BuildPlayer(levels, path, BuildTarget.StandaloneWindows, BuildOptions.None);
			Zip(Path.GetDirectoryName(path));

			path = $"Build/{PlayerSettings.productName} Mac.app";
			Debug.Log($"<Color=red>Building Mac</color> {path}");
			BuildPipeline.BuildPlayer(levels, path, BuildTarget.StandaloneOSX, BuildOptions.None);
			Zip(path);

			path = $"Build/{PlayerSettings.productName} Linux/{PlayerSettings.productName}.x64";
			Debug.Log($"<Color=red>Building Linux</color> {path}");
			BuildPipeline.BuildPlayer(levels, path, BuildTarget.StandaloneLinux64, BuildOptions.None);
			Zip(Path.GetDirectoryName(path));

			path = $"Build/{PlayerSettings.productName} Android.apk";
			Debug.Log($"<Color=red>Building Android</color> {path}");
			BuildPipeline.BuildPlayer(levels, path, BuildTarget.Android, BuildOptions.None);

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
