using UnityEngine;

public class SwapMaterialOnTriggger : MonoBehaviour
{
	[SerializeField] private Material _inTriggerMat;
	[SerializeField] private Material _outOfTriggerMat;
	[SerializeField] private int _materialIndex;
	private Renderer _renderer;

	private void Start()
	{
		_renderer = GetComponent<Renderer>();
		_renderer.materials[_materialIndex] = _outOfTriggerMat;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			SetMaterial(_inTriggerMat);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			SetMaterial(_outOfTriggerMat);
		}
	}

	private void SetMaterial(Material mat)
	{
		var mats = _renderer.materials;
		mats[_materialIndex] = mat;
		_renderer.materials = mats;
	}
}