using System.Collections;
using TMPro;
using UnityEngine;

public class TextAnimator : MonoBehaviour
{
	[SerializeField][Multiline] private string _fullText;
	[SerializeField] private float _characterDelay = 0.1f;

	[SerializeField] private TextMeshProUGUI _textMesh;
	public string FullText
	{
		get => _fullText;
		set
		{
			_fullText = value;
			_textMesh.text = _fullText;
		}
	}

	private void OnValidate()
	{
		if (_textMesh != null)
			_textMesh.text = _fullText;
	}
	private void OnEnable()
	{
		if (string.IsNullOrWhiteSpace(_fullText))
		{
			_fullText = _textMesh.text;
			_textMesh.text = "";
		}
		StartCoroutine(AnimateText());
	}

	private IEnumerator AnimateText()
	{
		_textMesh.text = "";
		foreach (char c in _fullText)
		{
			_textMesh.text += c;
			yield return new WaitForSeconds(_characterDelay);
		}
	}
}