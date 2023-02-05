using UnityEngine;

public class DoorConsole : MonoBehaviour
{
	[Multiline]
	[SerializeField] private string _doorText;
	[SerializeField] private string _doorCode;
	[SerializeField] private Animator _doorAnimator;
	[SerializeField] private bool _doorOpen = false;

	private void OnValidate()
	{
		GetComponentInChildren<TextAnimator>().FullText = _doorText;
		GetComponentInChildren<Keypad>().Code = _doorCode;
	}

	private void Start()
	{
		setDoor();
	}

	public void OpenDoor()
	{
		_doorOpen = true;
		setDoor();
	}
	public void CloseDoor()
	{
		_doorOpen = false;
		setDoor();

	}
	public void ToggleDoor()
	{
		_doorOpen = !_doorOpen;
		setDoor();

	}

	private void setDoor()
	{
		_doorAnimator.SetBool("character_nearby", _doorOpen);
	}
}
