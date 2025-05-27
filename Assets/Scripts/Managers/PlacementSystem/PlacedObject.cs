using UnityEngine;

public class PlacedObject : MonoBehaviour {
	private void Start() {
		int layer = LayerMask.NameToLayer("TempBuild");
		gameObject.layer = layer;

		if (TryGetComponent(out MeshRenderer renderer)) {
			if (!TryGetComponent(out Collider collider)) {
				gameObject.AddComponent<MeshCollider>();
			}
		}

		MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
		foreach (MeshRenderer childRenderer in renderers) {
			childRenderer.gameObject.layer = layer;
			if (childRenderer.gameObject.TryGetComponent<Collider>(out Collider collider)) {
				continue;
			}
			childRenderer.gameObject.AddComponent<MeshCollider>();
		}
	}
}