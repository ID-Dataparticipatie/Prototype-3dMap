using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingLights : MonoBehaviour {
    public int windowMaterialIndex;
    public Color lightColor;
    public bool areLightsOn;
    private Color _defaultColor;
    private MeshRenderer _mr;

    private void Start() {
        _mr = GetComponent<MeshRenderer>();
        _defaultColor = _mr.materials[windowMaterialIndex].color;
        SetLights(areLightsOn);
    }

    public void SetLights(bool isOn) {
        _mr.materials[windowMaterialIndex].shader = isOn ? Shader.Find("Universal Render Pipeline/Unlit") : Shader.Find("Universal Render Pipeline/Lit");
        _mr.materials[windowMaterialIndex].color = isOn ? lightColor : _defaultColor;
    }
}
