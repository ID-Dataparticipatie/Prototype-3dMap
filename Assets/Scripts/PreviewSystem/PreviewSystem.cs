using UnityEngine;

public class PreviewSystem : MonoBehaviour {

	// object refs
	[SerializeField]
	private Camera _previewCamera;

	[SerializeField]
	private Transform _previewRenderPoint;

	[SerializeField]
	private GameObject _previewObjectPrefab; // Used for setting the preview object in the editor

	private GameObject _currentPreviewObject;


	// Rotation settings
	[SerializeField]
	private bool _rotateCamera = true;
	[SerializeField]
	private float _cameraRotationSpeed = 150f;
	private Vector3 _cameraTarget;

	public static PreviewSystem Instance { get; private set; }

	void Awake() {
		if (Instance == null) {
			Instance = this;
		}
		else {
			Logger.LogWarning("PreviewSystem", "Multiple instances of PreviewSystem detected. Destroying duplicate.");
			Destroy(gameObject);
		}
	}

	void Update() {
		if (_currentPreviewObject != null && _rotateCamera) {
			_previewCamera.transform.RotateAround(_currentPreviewObject.transform.position, Vector3.up, _cameraRotationSpeed * Time.deltaTime);
			_previewCamera.transform.LookAt(_cameraTarget);
		}
	}


	public RenderTexture GetCurrentRenderTexture() {
		if (_previewCamera != null) {
			if (!_previewCamera.targetTexture) {
				_previewCamera.targetTexture = new RenderTexture(1024, 1024, 24);
			}
			return _previewCamera.targetTexture;
		}
		else {
			Logger.LogError("PreviewSystem", "Preview camera is not set.");
			return null;
		}
	}


	public void SetPreviewObject(GameObject prefab) {
		// Instantiate the preview object at the camera's position and rotation
		if (_currentPreviewObject != null) {
			DestroyImmediate(_currentPreviewObject);
		}

		if (prefab == null) {
			Logger.LogError("PreviewSystem", "Prefab is null. Cannot set preview object.");
			return;
		}

		_currentPreviewObject = Instantiate(prefab, _previewRenderPoint.position, _previewRenderPoint.rotation);

		_currentPreviewObject.transform.SetParent(_previewRenderPoint);

		// Set the preview object's layer to "PrefabPreview". This ensures the preview camera can render it correctly without interference from other objects.
		int previewLayer = LayerMask.NameToLayer("PrefabPreview");
		if (previewLayer != -1) {
			_currentPreviewObject.layer = previewLayer;
			foreach (Transform child in _currentPreviewObject.transform) {
				child.gameObject.layer = previewLayer;
			}
		}
		FitCameraToObject();
	}


	private void FitCameraToObject() {
		// Get the bounds of the preview object and its children
		Bounds previewBounds = new(_currentPreviewObject.transform.position, Vector3.zero);
		foreach (Renderer renderer in _currentPreviewObject.GetComponentsInChildren<Renderer>()) {
			previewBounds.Encapsulate(renderer.bounds);
		}

		// Calculate the orthographic size based on the bounds
		Vector3 boundsSize = previewBounds.size;
		float diagonal = Mathf.Sqrt(boundsSize.x * boundsSize.x + boundsSize.y * boundsSize.y + boundsSize.z * boundsSize.z);
		_previewCamera.orthographicSize = diagonal / 2f;

		// Adjust camera y position to the center of the object
		Vector3 newPostion = _previewCamera.transform.position;
		newPostion.y = previewBounds.center.y;
		_previewCamera.transform.position = newPostion;
		_cameraTarget = previewBounds.center;
	}
}