using UnityEngine;

public class CamSwitch : MonoBehaviour {
    [SerializeField]
    private Camera[] _cameras;
    private int _activeCamera;

    private void Start() {
        EventBus.Instance.Subscribe(EventType.CHANGE_CAMERA, OnChangeCamera);
    }

    private void OnChangeCamera() {
        _activeCamera++;
        if (_cameras.Length == _activeCamera) {
            _activeCamera = 0;
        }
        SetActiveCamera(_cameras[_activeCamera]);
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