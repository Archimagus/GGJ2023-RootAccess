using TMPro;
using UnityEngine;

public class Keypad : MonoBehaviour
{
	[SerializeField] string _code;
	[SerializeField] TextMeshProUGUI _inputText;

	public string Code { get => _code; set => _code = value; }

	private void OnValidate()
	{
		if (_inputText != null)
			_inputText.text = _code;
	}

	void OnEnable()
	{
		_inputText.text = string.Empty;
	}
	public void KeyPress(string key)
	{
		if (key == "OK")
		{
			CheckCode();
		}
		else if (key == "←" && _inputText.text.Length > 0)
		{
			_inputText.text = _inputText.text.Substring(0, _inputText.text.Length - 1);
		}
		else if (_inputText.text.Length < 4)
		{
			_inputText.text += key;
		}
	}
	public void CheckCode()
	{
		if (_inputText.text == _code)
		{
			GetComponentInParent<DoorConsole>().OpenDoor();
		}
		_inputText.text = string.Empty;
	}
}
