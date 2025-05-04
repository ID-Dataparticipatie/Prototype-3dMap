using UnityEngine;

public class PreviewSystem : MonoBehaviour {
	[SerializeField]
	private Camera _previewCamera;

	[SerializeField]
	private Transform _previewRenderPoint;

	[SerializeField]
	private GameObject _previewObjectPrefab;

	private GameObject _currentPreviewObject;

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

	public RenderTexture GetCurrentRenderTexture() {
		if (_previewCamera != null) {
			return _previewCamera.targetTexture;
		}
		else {
			Logger.LogError("PreviewSystem", "Preview camera is not set.");
			return null;
		}
	}

	public void SetPreviewObject(GameObject prefab) {
		// Instantiate the preview object at the camera's position and rotation
		if (_currentPreviewObject != null ) {
			DestroyImmediate(_currentPreviewObject);
		}

		if (prefab == null) {
			Logger.LogError("PreviewSystem", "Prefab is null. Cannot set preview object.");
			return;
		}

		_currentPreviewObject = Instantiate(prefab, _previewRenderPoint.position, _previewRenderPoint.rotation);

		// Set the preview object as a child of the camera
		_currentPreviewObject.transform.SetParent(_previewRenderPoint);

		// Set the preview object's layer to "Preview"
		int previewLayer = LayerMask.NameToLayer("PrefabPreview");
		if (previewLayer != -1) {
			_currentPreviewObject.layer = previewLayer;
			foreach (Transform child in _currentPreviewObject.transform) {
				child.gameObject.layer = previewLayer;
			}
		}
	}

	void OnValidate() {
		if (_previewObjectPrefab != null) {
			UnityEditor.EditorApplication.delayCall += () => {
				SetPreviewObject(_previewObjectPrefab);
			};
		}
		else {
			UnityEditor.EditorApplication.delayCall += () => {
				DestroyImmediate(_currentPreviewObject);
			};
		}
	}
}