using TMPro;
using UnityEngine;

public class KeypadButton : MonoBehaviour
{
	private Keypad _keypad;
	private string _keyText;

	void Start()
	{
		_keypad = GetComponentInParent<Keypad>();
		_keyText = GetComponentInChildren<TextMeshProUGUI>().text;
	}

	public void Click()
	{
		_keypad.KeyPress(_keyText);
	}

}
