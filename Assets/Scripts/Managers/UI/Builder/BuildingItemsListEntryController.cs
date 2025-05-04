using UnityEngine.UIElements;

public class BuildingItemsListEntryController {
	private Label _itemName;

	public void SetVisualElement(VisualElement visualElement) {
		_itemName = visualElement.Q<Label>("item-name-text");
	}

	public void SetEntryData(BuildingObjectData objectData) {
		_itemName.text = objectData.ObjectName;
	}
}
