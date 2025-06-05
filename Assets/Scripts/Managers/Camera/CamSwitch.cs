using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UIElements;

public class CamSwitch : MonoBehaviour {
    [SerializeField]
    private Camera[] _cameras;
    [SerializeField]
    private UIDocument _cameraUI;
    private int _activeCamera=0;

    private void Start() {
        EventBus.Instance.Subscribe(EventType.CHANGE_CAMERA, OnChangeCamera);
        //zet de current camera in de text in camera ui
        _cameraUI.rootVisualElement.Q<Label>("CurrentCamera").text = _cameras[_activeCamera].name;
    }

    private void OnChangeCamera() {
        _activeCamera++;
        if (_cameras.Length == _activeCamera) {
            _activeCamera = 0;
        }
        SetActiveCamera(_cameras[_activeCamera]);
        //zet de current camera in de text in camera ui
        _cameraUI.rootVisualElement.Q<Label>("CurrentCamera").text = _cameras[_activeCamera].name;
    }
    private void SetActiveCamera(Camera newCamera) {
        foreach (Camera cam in _cameras) {
            if (cam != newCamera) {
                cam.gameObject.SetActive(false);
            }
        }
        newCamera.gameObject.SetActive(true);

    }
}