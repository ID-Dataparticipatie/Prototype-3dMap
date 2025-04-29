using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoliceSiren : MonoBehaviour {

	public GameObject blueLight, redLight;
	public bool isSirenOn;
	public float colorInterval;
	private float _timer;
	private MeshRenderer _mr;
	private Shader _defShader, _unlitShader;

	private float _randomTimeOffset;
	private bool _initialDelayComplete = false;

	private void Start() {
		_mr = GetComponent<MeshRenderer>();
		_defShader = Shader.Find("Universal Render Pipeline/Lit");
		_unlitShader = Shader.Find("Universal Render Pipeline/Unlit");
		_randomTimeOffset = Random.Range(0, colorInterval * 2);
	}

	private void Update() {
		if (!_initialDelayComplete) {
			_timer += Time.deltaTime;
			if (_timer >= _randomTimeOffset) {
				_initialDelayComplete = true;
			}
			return;
		}

		if (isSirenOn) {
			if (_timer > colorInterval) {
				// index 3 : blue, index 4 : red
				bool isBlueUnlit = _mr.materials[3].shader == _unlitShader;

				blueLight.SetActive(!isBlueUnlit);
				redLight.SetActive(isBlueUnlit);

				_mr.materials[3].shader = isBlueUnlit ? _defShader : _unlitShader;
				_mr.materials[4].shader = isBlueUnlit ? _unlitShader : _defShader;

				_timer = 0;
			}
		}
		_timer += Time.deltaTime;
	}
}
