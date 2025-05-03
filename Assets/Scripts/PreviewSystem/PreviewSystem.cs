using UnityEngine;

public class PreviewSystem : MonoBehaviour {
	[SerializeField]
	private Camera _previewCamera;

	[SerializeField]
	private Transform _previewRenderPoint;

	[SerializeField]
	private GameObject _previewObjectPrefab;

	private GameObject _currentPreviewObject;

	public void SetPreviewObject(GameObject prefab) {
		// Instantiate the preview object at the camera's position and rotation
		DestroyImmediate(_currentPreviewObject);
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