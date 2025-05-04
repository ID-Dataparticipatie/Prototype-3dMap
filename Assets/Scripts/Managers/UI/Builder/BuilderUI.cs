using UnityEngine;
using UnityEngine.UIElements;

public class BuilderUI : MonoBehaviour {

	[SerializeField]
	private VisualTreeAsset _buildListEntryTemplate;


	private void OnEnable() {
		UIDocument uiDocument = GetComponent<UIDocument>();

		BuildingItemsListController characterListController = new();
		characterListController.InitializeList(uiDocument.rootVisualElement, _buildListEntryTemplate);
	}
}