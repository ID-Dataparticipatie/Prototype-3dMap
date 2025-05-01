using UnityEngine;

public class ConstraintBox : MonoBehaviour {
	public Bounds Constraints;

	[SerializeField]
	private bool _drawGizmo = true;

	void OnDrawGizmos() {
		if (_drawGizmo) {
			Gizmos.color = Color.green;
			Gizmos.DrawWireCube(Constraints.center, Constraints.size);
		}
	}
}