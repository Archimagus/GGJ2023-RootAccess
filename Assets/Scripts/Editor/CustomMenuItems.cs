using UnityEditor;
using UnityEngine;

public class CustomMenuItems : MonoBehaviour
{
	[MenuItem("GameObject/RotateLeft %q")]
	private static void RotateLeft()
	{
		Rotate(-90);
	}

	[MenuItem("GameObject/RotateLeft %q", true)]
	private static bool ValidateRotateLeft()
	{
		// Return false if no transform is selected.
		return Selection.activeTransform != null;
	}

	[MenuItem("GameObject/RotateRight %e")]
	private static void RotateRight()
	{
		Rotate(90);
	}

	private static void Rotate(float angle)
	{
		//Selection.activeTransform.Rotate(0, 90 - (Selection.activeTransform.eulerAngles.y % 90), 0);

		//var center = Vector3.zero;
		//foreach (var t in Selection.transforms)
		//{
		//	center += t.position;
		//}
		//center /= Selection.transforms.Length;

		var center = Selection.activeTransform.position;

		foreach (var t in Selection.transforms)
		{
			t.RotateAround(center, Vector3.up, angle);
		}
	}

	[MenuItem("GameObject/RotateRight %e", true)]
	private static bool ValidateRotateRight()
	{
		// Return false if no transform is selected.
		return Selection.activeTransform != null;
	}

	[MenuItem("GameObject/Zero Y %w")]
	private static void ZeroY()
	{
		foreach (var t in Selection.transforms)
		{
			Vector3 pos = t.position;
			pos.y = 0;
			pos.x = (Mathf.Round(pos.x / 4)) * 4;
			pos.z = (Mathf.Round(pos.z / 4)) * 4;
			t.position = pos;
		}
	}

	[MenuItem("GameObject/Zero Y %w", true)]
	private static bool ValidateZeroY()
	{
		// Return false if no transform is selected.
		return Selection.activeTransform != null;
	}
}