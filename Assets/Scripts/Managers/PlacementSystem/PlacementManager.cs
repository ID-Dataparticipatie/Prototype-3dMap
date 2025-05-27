using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlacementManager : MonoBehaviour {
	[SerializeField]
	private Camera _playerView;

	private GameObject _buildablePrefab;
	private GameObject _currentPlacedPrefab;

	private RaycastHit _placementPosition;

	[SerializeField]
	private LayerMask _buildableLayers;

	[SerializeField]
	private LayerMask _tempBuildLayers;

	[SerializeField]
	private float _rotationFactor = 2.5f;

	[SerializeField]
	private Material _validPlacementMaterial;
	[SerializeField]
	private Material _invalidPlacementMaterial;

	private bool _canPlace = false;

	// Values to map holding behaviour
	private float _rotationInput;
	private Vector2 _gamepadInput;
	private (Vector2, bool) _lastInput;


	public static PlacementManager Instance;


	private void Awake() {

		if (Instance == null) {
			Instance = this;
		}
		else {
			Logger.LogWarning("Placementmanager", "Multiple instances of PlacementManager detected. Destroying duplicate.");
			Destroy(gameObject);
		}
	}

	private void Start() {
		if (_playerView == null) {
			_playerView = Camera.main;
		}
		EventBus.Instance.Subscribe<(Vector2, bool)>(EventType.MOVE_STRUCTURE, MoveStructureWrapper); // yes I should probably fix stupid stuff like this in eventbus but I cannot be bothered right now
		EventBus.Instance.Subscribe<float>(EventType.ROTATE_STRUCTURE, OnRotateStructure);
		EventBus.Instance.Subscribe<GameObject>(EventType.CHANGE_STRUCTURE, SwitchPrefab);
		EventBus.Instance.Subscribe(EventType.PLACE_STRUCTURE, OnPlaceStructure);
		EventBus.Instance.Subscribe(EventType.REMOVE_STRUCTURE, OnRemoveStructure);

	}

	void FixedUpdate() {
		if (_currentPlacedPrefab != null && _rotationInput != 0) {
			_currentPlacedPrefab.transform.Rotate(Vector3.up, _rotationInput * _rotationFactor);
		}
		if (_gamepadInput != Vector2.zero) {
			OnMoveStructure(_gamepadInput, true);
		}
		SwitchPlacementHightlight();
	}

	void OnDisable() {
		if (Instance == this) {
			Instance = null;
		}
		EventBus.Instance.Unsubscribe<(Vector2, bool)>(EventType.MOVE_STRUCTURE, MoveStructureWrapper); 
		EventBus.Instance.Unsubscribe<float>(EventType.ROTATE_STRUCTURE, OnRotateStructure);
		EventBus.Instance.Unsubscribe<GameObject>(EventType.CHANGE_STRUCTURE, SwitchPrefab);
		EventBus.Instance.Unsubscribe(EventType.PLACE_STRUCTURE, OnPlaceStructure);
	}


	private void MovePrefab() {
		if (_currentPlacedPrefab != null) {
			_currentPlacedPrefab.transform.position = _placementPosition.point;
		}
	}

	public void SwitchPrefab(GameObject prefab) {
		Quaternion rotation = Quaternion.identity;
		if (_currentPlacedPrefab != null) {
			rotation = _currentPlacedPrefab.transform.rotation;
			Destroy(_currentPlacedPrefab);
		}

		if (prefab == null) {
			Logger.LogWarning("PlacementManager", "Prefab is null. Cannot set placement.");
			return;
		}

		if (rotation == Quaternion.identity) {
			// If the rotation is not set, use the prefab's rotation
			rotation = prefab.transform.rotation;
		}

		_buildablePrefab = prefab;
		_currentPlacedPrefab = Instantiate(prefab, _placementPosition.point, rotation);
		_currentPlacedPrefab.transform.parent = _placementPosition.transform;
	}

	private void SwitchPlacementHightlight() {
		if (_currentPlacedPrefab != null && _placementPosition.transform != null) {
			List<Renderer> renderers = _currentPlacedPrefab.GetComponentsInChildren<Renderer>().ToList();

			// Bitwise, if layermask contains object layer
			if ((_buildableLayers & (1 << _placementPosition.transform.gameObject.layer)) != 0) {
				SetMaterialsInRenderers(renderers, _validPlacementMaterial);
				_canPlace = true;
			}
			else {
				SetMaterialsInRenderers(renderers, _invalidPlacementMaterial);
				_canPlace = false;
			}
		}
	}

	private void SetMaterialsInRenderers(List<Renderer> renderers, Material material) {
		foreach (Renderer renderer in renderers) {
			Material[] materials = renderer.materials;
			for (int i = 0; i < materials.Length; i++) {
				materials[i] = material;
			}
			renderer.materials = materials;
		}
	}

	private void MoveStructureWrapper((Vector2, bool) data) {
		OnMoveStructure(data.Item1, data.Item2);
	}

	private void OnMoveStructure(Vector2 position, bool useCenter) {

		// Use the mouse position
		Ray ray = _playerView.ScreenPointToRay(position);

		// If not using mouse, use the center of the screen
		if (useCenter) {
			// You do not want to know what abomination led to this. Makes controllers release the object when the trigger is released while looking around.
			if (position == Vector2.zero && _gamepadInput == Vector2.zero) {
				return;
			}
			_gamepadInput = position;
			ray = _playerView.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
		}


		if (ShootSelectorRay(ray, out RaycastHit hit)) {
			_placementPosition = hit;
			MovePrefab();
		}
	}

	private void OnRotateStructure(float rotation) {
		_rotationInput = rotation;
	}

	private void OnPlaceStructure() {
		if (_canPlace) {
			GameObject newPlacement = Instantiate(_buildablePrefab, _currentPlacedPrefab.transform.position, _currentPlacedPrefab.transform.rotation);
			newPlacement.transform.parent = _placementPosition.transform;

			newPlacement.AddComponent<PlacedObject>();

			EventBus.Instance.TriggerEvent<GameObject>(EventType.CHANGE_STRUCTURE, _buildablePrefab);
		}
	}

	private void OnRemoveStructure() {

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		//check if raycast collides with an placed object and delete it
		if (ShootSelectorRay(ray, out RaycastHit hit, _tempBuildLayers)) {
			Destroy(FindClosestPlacedRoot(hit.collider.gameObject));
		}
	}


	private bool ShootSelectorRay(Ray ray, out RaycastHit hitInfo, LayerMask layerMask = default, float length = 100.0f) {
		if (layerMask == default) {
			layerMask = ~0; // Cannot set the "Everything" layer directly so use default as a placeholder
		}
		Debug.DrawRay(ray.origin, ray.direction * length, Color.red, 0.2f);
		if (Physics.Raycast(ray, out hitInfo, length, layerMask)) {
			return true;
		}
		return false;
	}

	private GameObject FindClosestPlacedRoot(GameObject start) {
		return start.GetComponentInParent<PlacedObject>().gameObject ?? start;
	}
}