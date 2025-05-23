using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LightColor { Red, Yellow, Green, None }
public class TrafficLights : MonoBehaviour {

    public LightColor activeLight;
    private MeshRenderer _mr;
    private Shader _defShader, _unlitShader;

    private void Start() {
        _mr = GetComponent<MeshRenderer>();
        _defShader = Shader.Find("Universal Render Pipeline/Lit");
        _unlitShader = Shader.Find("Universal Render Pipeline/Unlit");
        SetLight(activeLight);
    }

    public void SetLight(LightColor color) {
        // mat 1 : green, mat 2 : yellow, mat 3 : red
        int activeIndex = 0;
        switch (color) {
            case LightColor.Green:
                activeIndex = 1;
                break;

            case LightColor.Yellow:
                activeIndex = 2;
                break;

            case LightColor.Red:
                activeIndex = 3;
                break;
        }

        for(int i = 1; i < 4; i++) {
            _mr.materials[i].shader = activeIndex == i ? _unlitShader : _defShader;
        }
    }

}
