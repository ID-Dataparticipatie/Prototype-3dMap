using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour {
	private static string _logname = "InputManager";
	private string _currentProfile = "Default";

	private PlayerInput _playerInputActions;

	public static InputManager Instance;

	private void Awake() {
		if (Instance == null) {
			Instance = this;
		}
		else {
			Logger.LogWarning(_logname, "Multiple Instances found! Exiting..");
			Destroy(this);
			return;
		}
		DontDestroyOnLoad(Instance);
	}

	void Start() {
		_playerInputActions = GetComponent<PlayerInput>();
		SetControlMap(_playerInputActions.defaultActionMap);

		// Switch control map when entering UI state
		EventBus.Instance.Subscribe<bool>(EventType.MENU_BUILD, state => SetControlMap(state ? "UI" : "Player"));
	}


	public void OnMove(InputValue value) {
		if (CheckTimeScale())
			EventBus.Instance.TriggerEvent(EventType.PLAYER_MOVE, value.Get<Vector2>());
	}

	public void OnLook(InputValue value) {
		// If using mouse, ensure rmb is held before looking around

		if (CheckTimeScale()) {
			if (IsUsingMouseProfile() && !IsRightButtonPressed()) {
				EventBus.Instance.TriggerEvent(EventType.PLAYER_LOOK, Vector2.zero);
				return;
			}
			EventBus.Instance.TriggerEvent(EventType.PLAYER_LOOK, value.Get<Vector2>());
		}
	}

	// Build mode!
	public void OnMoveStructure(InputValue value) {
		if (CheckTimeScale()) {
			if (IsUsingMouseProfile() && IsLeftButtonPressed()) {
				EventBus.Instance.TriggerEvent(EventType.MOVE_STRUCTURE, (value.Get<Vector2>(), false));
			}
			else if (IsUsingGamepadProfile() && IsRightTriggerPressed()) {
				EventBus.Instance.TriggerEvent(EventType.MOVE_STRUCTURE, (value.Get<Vector2>(), true));
			}
			else if (IsUsingGamepadProfile()) { // Ensure the structure is decoupled when the trigger is released
				EventBus.Instance.TriggerEvent(EventType.MOVE_STRUCTURE, (Vector2.zero, true));
			}
		}
	}

	public void OnRotateStructure(InputValue value) {
		if (CheckTimeScale()) {
			EventBus.Instance.TriggerEvent(EventType.ROTATE_STRUCTURE, value.Get<float>());
		}
	}

	public void OnPlace(InputValue value) {
		if (CheckTimeScale())
			EventBus.Instance.TriggerEvent(EventType.PLACE_STRUCTURE);
	}

	// UI Stuff

	public void OnToggleBuildMenu(InputValue value) {
		if (CheckTimeScale())
			EventBus.Instance.TriggerEvent(EventType.MENU_BUILD, value.isPressed);
		EventBus.Instance.TriggerEvent(EventType.MENU_BUILD);
	}




	// Helpers

	public void OnControlsChanged(PlayerInput value) {
		_currentProfile = value.currentControlScheme;
		Logger.Log(_logname, $"Control scheme changed to: {_currentProfile}");
	}

	public void SetControlMap(string controlMap) {
		_playerInputActions = GetComponent<PlayerInput>();
		_playerInputActions.SwitchCurrentActionMap(controlMap);
		Logger.Log(_logname, "Current actionmap: " + _playerInputActions.currentActionMap.name);
	}

	private bool CheckTimeScale() {
		return Time.timeScale != 0;
	}

	private bool IsUsingMouseProfile() {
		return _currentProfile.Contains("Mouse");
	}
	private bool IsUsingGamepadProfile() {
		return _currentProfile.Contains("Gamepad");
	}

	private bool IsRightButtonPressed() {
		return Mouse.current.rightButton.isPressed;
	}
	private bool IsLeftButtonPressed() {
		return Mouse.current.leftButton.isPressed;
	}

	private bool IsRightTriggerPressed() {
		return Gamepad.current.rightTrigger.isPressed;
	}
}