using UnityEngine;

public class CameraController : MonoBehaviour {
	[SerializeField]
	private ConstraintBox _constraintBox;

	[SerializeField]
	private float _moveSpeed = 5f;
	[SerializeField]
	private float _lookSpeed = 1.5f;

	private Vector2 _lookVector;
	private Vector2 _moveVector;



	private void Start() {
		EventBus.Instance.Subscribe<Vector2>(EventType.PLAYER_LOOK, OnPlayerLook);
		EventBus.Instance.Subscribe<Vector2>(EventType.PLAYER_MOVE, OnPlayerMove);
	}

	private void Update() {
		if (_lookVector != Vector2.zero) {

			float pitch = transform.localEulerAngles.x;
			float yaw = transform.localEulerAngles.y;
			if (pitch > 180) pitch -= 360; // convert 0-360 to -180 to 180

			pitch -= _lookVector.y * 0.1f * _lookSpeed;
			yaw += _lookVector.x * 0.1f * _lookSpeed;
			pitch = Mathf.Clamp(pitch, -90, 90);
			transform.localEulerAngles = new(pitch, yaw, 0);
		}
	}

	private void FixedUpdate() {


		if (_moveVector != Vector2.zero) {

			Vector3 moveDirection = (transform.forward * _moveVector.y) + (transform.right * _moveVector.x);
			Vector3 newPosition = transform.position + _moveSpeed * Time.deltaTime * moveDirection;
			SetPositionWithinBounds(newPosition);
		}

	}

	private void OnPlayerLook(Vector2 direction) {
		_lookVector = direction;
	}

	private void OnPlayerMove(Vector2 direction) {
		_moveVector = direction;
	}

	private void SetPositionWithinBounds(Vector3 position) {
		if (_constraintBox == null || _constraintBox.Constraints.Contains(position)) {
			transform.position = position;
		}
		else {
			// If the camera is outside the bounds, clamp it to the bounds
			transform.position = _constraintBox.Constraints.ClosestPoint(position);
		}
	}

}