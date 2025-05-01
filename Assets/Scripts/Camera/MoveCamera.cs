using UnityEngine;

public class MoveCamera : MonoBehaviour
{
	public Transform Target; // The target to follow
	public float SmoothSpeed = 0.125f; // Speed of the camera movement
	public Vector3 Offset; // Offset from the target

	void LateUpdate()
	{
		if (Target != null)
		{
			Vector3 desiredPosition = Target.position + Offset;
			Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, SmoothSpeed);
			transform.position = smoothedPosition;
		}
	}
}