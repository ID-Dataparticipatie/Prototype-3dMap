using UnityEngine;

[CreateAssetMenu(fileName = "new PlacableObjectData", menuName = "PrototypeMaps/PlacebleObjectData", order = 0)]
public class BuildingObjectData : ScriptableObject {
	public string ObjectName;
	public GameObject Prefab;

	private void OnValidate() {
		if (Prefab != null && ObjectName == "") {
			ObjectName = Prefab.name;
		}
	}
}