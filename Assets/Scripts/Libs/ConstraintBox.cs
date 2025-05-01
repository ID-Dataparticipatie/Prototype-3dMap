using UnityEngine;

public class ConstraintBox : MonoBehaviour {
	public Bounds Constraints;

	void OnDrawGizmos() {
		Gizmos.DrawWireCube(Constraints.center, Constraints.size);
	}
}