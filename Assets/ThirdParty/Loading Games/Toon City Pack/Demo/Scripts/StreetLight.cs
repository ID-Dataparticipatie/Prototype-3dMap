using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StreetLight : MonoBehaviour {

    public Light[] lights;
    public bool isOn;

    private void Start() {
        SetLight(isOn);
    }

    private void Update() {
        SetLight(isOn);
    }

	private void OnValidate() {
		SetLight(isOn);
	}

    public void SetLight(bool isOn) {
        this.isOn = isOn;

        MeshRenderer mr = GetComponent<MeshRenderer>();
        Shader shader = Shader.Find(isOn ? "Universal Render Pipeline/Unlit" : "Universal Render Pipeline/Lit");
        mr.sharedMaterials[1].color = lights[0].color;
        mr.sharedMaterials[1].shader = shader;

        foreach(Light l in lights) {
            l.gameObject.SetActive(isOn);
        }
    }
}
