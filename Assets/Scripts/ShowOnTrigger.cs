using UnityEngine;

public class ShowOnTrigger : MonoBehaviour
{
	[SerializeField] GameObject _object;
	[SerializeField] bool _defaultState;

	private void Start()
	{
		_object.SetActive(_defaultState);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			_object.SetActive(true);
		}

	}
	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			_object.SetActive(false);
		}

	}

}
