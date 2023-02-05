using UnityEngine;

public class OpenDoor : MonoBehaviour
{
	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			GetComponent<Animator>().SetBool("character_nearby", true);
		}
	}
	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			GetComponent<Animator>().SetBool("character_nearby", false);
		}
	}
}
