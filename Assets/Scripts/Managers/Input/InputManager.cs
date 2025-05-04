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
		
		EventBus.Instance.Subscribe<bool>(EventType.MENU_BUILD, state => SetControlMap(state ? "UI" : "Player"));
	}

	public void OnControlsChanged(PlayerInput value) {
		_currentProfile = value.currentControlScheme;
		Logger.Log(_logname, $"Control scheme changed to: {_currentProfile}");
	}

	public void OnMove(InputValue value) {
		if (CheckTimeScale())
			EventBus.Instance.TriggerEvent(EventType.PLAYER_MOVE, value.Get<Vector2>());
	}

	public void OnLook(InputValue value) {
		// If using mouse, ensure rmb is held before looking around
		if (IsUsingMouseProfile() && !IsRightButtonPressed()) {
			EventBus.Instance.TriggerEvent(EventType.PLAYER_LOOK, Vector2.zero);
			return;
		}
		if (CheckTimeScale())
			EventBus.Instance.TriggerEvent(EventType.PLAYER_LOOK, value.Get<Vector2>());
	}

	public void OnPlace(InputValue value) {
		if (CheckTimeScale())
			EventBus.Instance.TriggerEvent(EventType.PLAYCER_PLACE, value.isPressed);
	}

	public void OnToggleBuildMenu(InputValue value) {
		if (CheckTimeScale())
			EventBus.Instance.TriggerEvent(EventType.MENU_BUILD, value.isPressed);
		EventBus.Instance.TriggerEvent(EventType.MENU_BUILD);
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

	private bool IsRightButtonPressed() {
		return Mouse.current.rightButton.isPressed;
	}
}