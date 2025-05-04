using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BuildingItemsListController {

	private VisualTreeAsset _buildListEntryTemplate;
	private ListView _buildingItemsList;
	private VisualElement _buildingItemPreview;

	private List<BuildingObjectData> _allBuildingItems;

	public void InitializeList(VisualElement root, VisualTreeAsset listElementTemplate) {
		EnumerateAllEntries();

		_buildListEntryTemplate = listElementTemplate;
		_buildingItemsList = root.Q<ListView>("build-item-selector");
		_buildingItemPreview = root.Q<VisualElement>("build-item-preview");

		FillList();

		_buildingItemsList.selectionChanged += OnEntrySelected;
	}

	private void EnumerateAllEntries() {
		_allBuildingItems = new();
		_allBuildingItems.AddRange(Resources.LoadAll<BuildingObjectData>("BuildingSelector"));
	}

	private void FillList() {
		_buildingItemsList.makeItem = () => {
			TemplateContainer newListEntry = _buildListEntryTemplate.Instantiate();
			BuildingItemsListEntryController newListEntryLogic = new();
			newListEntry.userData = newListEntryLogic;

			newListEntryLogic.SetVisualElement(newListEntry);

			return newListEntry;
		};

		_buildingItemsList.bindItem = (item, index) => {
			(item.userData as BuildingItemsListEntryController)?.SetEntryData(_allBuildingItems[index]);
		};

		_buildingItemsList.itemsSource = _allBuildingItems;
	}


	void OnEntrySelected(IEnumerable<object> selectedItems) {
		BuildingObjectData selectedObject = _buildingItemsList.selectedItem as BuildingObjectData;

		if (selectedObject == null) {
			PreviewSystem.Instance.SetPreviewObject(null);
			return;
		}
		PreviewSystem.Instance.SetPreviewObject(selectedObject.Prefab);
		_buildingItemPreview.style.backgroundImage = new StyleBackground(Background.FromRenderTexture(PreviewSystem.Instance.GetCurrentRenderTexture()));
	}
}