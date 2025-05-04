using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour {
	private UIDocument _builderUI;

	void Start() {
		_builderUI = FindAnyObjectByType<BuilderUI>().GetComponent<UIDocument>();


		EventBus.Instance.Subscribe(EventType.MENU_BUILD, OnToggleBuildMenu);
	}


	private void OnToggleBuildMenu() {
		// _builderUI.gameObject.SetActive(!_builderUI.gameObject.);
	}
}