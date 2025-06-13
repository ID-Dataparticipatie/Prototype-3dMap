using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PreviewObject : MonoBehaviour {

	public List<Collider> Colliders = new();

	private void Start() {
		int layer = LayerMask.NameToLayer("Ignore Raycast");
		gameObject.layer = layer;

		if (TryGetComponent(out MeshRenderer renderer)) {
			if (!TryGetComponent(out Collider collider)) {
				MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
				meshCollider.convex = true;
				meshCollider.isTrigger = true;
				if (!TryGetComponent(out Rigidbody rigidbody)) {
					rigidbody = gameObject.AddComponent<Rigidbody>();
					rigidbody.isKinematic = true;
					rigidbody.useGravity = false;
				}
			}
		}

		MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
		foreach (MeshRenderer childRenderer in renderers) {
			childRenderer.gameObject.layer = layer;
			if (childRenderer.gameObject.TryGetComponent<Collider>(out Collider collider)) {
				continue;
			}

			MeshCollider meshCollider = childRenderer.gameObject.AddComponent<MeshCollider>();
			meshCollider.convex = true;
			meshCollider.isTrigger = true;
			print("Mesh done");

		}

	}

	void Update() {
		Colliders = Colliders.Where(c => c != null).ToList();
	}


	private void OnTriggerEnter(Collider collider) {
		if (!(collider.gameObject.layer == LayerMask.NameToLayer("Ground") || collider.gameObject.layer == LayerMask.NameToLayer("Buildable"))) {
			Colliders.Add(collider);
		}
	}

	private void OnTriggerExit(Collider collider) {
		if (!(collider.gameObject.layer == LayerMask.NameToLayer("Ground") || collider.gameObject.layer == LayerMask.NameToLayer("Buildable"))) {
			Colliders.Remove(collider);
		}

	}
}