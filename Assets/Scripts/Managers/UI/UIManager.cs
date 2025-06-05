using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour {
	[SerializeField]
	private UIDocument _builderUI;

	[SerializeField]
	private UIDocument _controlUI;

	void OnEnable() {
		EventBus.Instance.Subscribe(EventType.MENU_BUILD, OnToggleBuildMenu);
		EventBus.Instance.Subscribe(EventType.LEGEND_CONTROLS, OnToggleControlsLegend);
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

	private void OnToggleControlsLegend() {
		bool currentvalue = _controlUI.rootVisualElement.Q<Foldout>("Controls").value;
		_controlUI.rootVisualElement.Q<Foldout>("Controls").value = !currentvalue;
	}
}