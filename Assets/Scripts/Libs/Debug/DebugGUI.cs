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

		// Display Logging
		GUI.Label(new Rect(5, 100, 500, 25), "Log List:");
		GUI.BeginScrollView(new Rect(5, 120, 500, 300), Vector2.zero, new Rect(0, 0, 500, Logger.LogList.Count * 25));
		for (int i = 0; i < Logger.LogList.Count; i++) {
			GUI.Label(new Rect(0, i * 25, 500, 25), Logger.LogList[i]);
		}
		GUI.EndScrollView();
		if (GUI.Button(new Rect(5, 430, 100, 25), "Clear")) {
			Logger.LogList.Clear();
		}
	}
}