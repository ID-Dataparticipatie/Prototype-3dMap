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
			// Instantiate the UXML template for the entry
			TemplateContainer newListEntry = _buildListEntryTemplate.Instantiate();

			// Instantiate a controller for the data
			BuildingItemsListEntryController newListEntryLogic = new();

			// Assign the controller script to the visual element
			newListEntry.userData = newListEntryLogic;

			// Initialize the controller script
			newListEntryLogic.SetVisualElement(newListEntry);

			// Return the root of the instantiated visual tree
			return newListEntry;
		};

		// Set up bind function for a specific list entry
		_buildingItemsList.bindItem = (item, index) => {
			(item.userData as BuildingItemsListEntryController)?.SetEntryData(_allBuildingItems[index]);
		};

		// Set a fixed item height matching the height of the item provided in makeItem.
		// For dynamic height, see the virtualizationMethod property.
		// _buildingItemsList.fixedItemHeight = 45;

		// Set the actual item's source list/array
		_buildingItemsList.itemsSource = _allBuildingItems;
	}


	void OnEntrySelected(IEnumerable<object> selectedItems) {
		// Get the currently selected item directly from the ListView
		BuildingObjectData selectedObject = _buildingItemsList.selectedItem as BuildingObjectData;

		// Handle none-selection (Escape to deselect everything)
		if (selectedObject == null) {
			// Clear
			_buildingItemPreview.style.backgroundColor = Color.blue;
			return;
		}
		Debug.Log("Selected: " + _buildingItemPreview.style.backgroundColor);
		_buildingItemPreview.style.backgroundColor = new StyleColor(Color.white);
	}


}