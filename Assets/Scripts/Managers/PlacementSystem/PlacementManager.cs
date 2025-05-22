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
			Logger.LogWarning("PreviewSystem", "Multiple instances of PreviewSystem detected. Destroying duplicate.");
			Destroy(gameObject);
		}
	}

	private void Start() {
		if (_playerView == null) {
			_playerView = Camera.main;
		}
		EventBus.Instance.Subscribe<(Vector2, bool)>(EventType.MOVE_STRUCTURE, (data) => { _lastInput = data; OnMoveStructure(data.Item1, data.Item2); }); // yes I should probably fix stupid stuff like this in eventbus but I cannot be bothered right now
		EventBus.Instance.Subscribe<float>(EventType.ROTATE_STRUCTURE, OnRotateStructure);
		EventBus.Instance.Subscribe<GameObject>(EventType.CHANGE_STRUCTURE, SwitchPrefab);
		EventBus.Instance.Subscribe(EventType.PLACE_STRUCTURE, OnPlaceStructure);
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


	private void MovePrefab() {
		if (_currentPlacedPrefab != null) {
			_currentPlacedPrefab.transform.position = _placementPosition.point;
		}
	}

	public void SwitchPrefab(GameObject prefab) {
		if (_currentPlacedPrefab != null) {
			Destroy(_currentPlacedPrefab);
		}

		if (prefab == null) {
			Logger.LogWarning("PlacementManager", "Prefab is null. Cannot set placement.");
			return;
		}

		_buildablePrefab = prefab;
		_currentPlacedPrefab = Instantiate(prefab, _placementPosition.point, prefab.transform.rotation);
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

		Debug.DrawRay(ray.origin, ray.direction * 100.0f, Color.red);

		if (Physics.Raycast(ray, out RaycastHit hit, 100.0f)) {
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

			EventBus.Instance.TriggerEvent<GameObject>(EventType.CHANGE_STRUCTURE, _buildablePrefab);
		}
	}
}