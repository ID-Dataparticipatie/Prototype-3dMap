using System.Collections;
using UnityEngine;

public class DebugGUI : MonoBehaviour {

	private float _count;

	private IEnumerator Start() {
		GUI.depth = 2;
		while (true) {
			_count = 1f / Time.unscaledDeltaTime;
			yield return new WaitForSeconds(0.1f);
		}
	}

	private void OnGUI() {
		GUI.Label(new Rect(5, 40, 100, 25), "FPS: " + Mathf.Round(_count));
		GUI.Label(new Rect(5, 80, 500, 25), "Debugger attatched: " + System.Diagnostics.Debugger.IsAttached);

	}
}