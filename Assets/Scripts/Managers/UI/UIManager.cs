using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour {
	[SerializeField]
	private UIDocument _builderUI;

	void Start() {
		EventBus.Instance.Subscribe(EventType.MENU_BUILD, OnToggleBuildMenu);
	}


	private void OnToggleBuildMenu() {
		bool buildUIActive = _builderUI.gameObject.activeSelf;
		if (buildUIActive) {
			_builderUI.gameObject.SetActive(false);
			EventBus.Instance.TriggerEvent(EventType.MENU_BUILD, false);
			return;
		}
		_builderUI.gameObject.SetActive(true);
	}
}