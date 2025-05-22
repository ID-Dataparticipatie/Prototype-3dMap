using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugGUI : MonoBehaviour {

	private bool _showDebugWindow = false;
	private Rect _debugWindowRect = new(100, 100, 500, 300);

	private float _fpsCount;
	private string _logFilter = string.Empty;

	private IEnumerator Start() {
		GUI.depth = 2;
		while (true) {
			_fpsCount = 1f / Time.unscaledDeltaTime;
			yield return new WaitForSeconds(0.1f);
		}
	}

	private void OnGUI() {
		GUILayout.Label("FPS: " + Mathf.Round(_fpsCount));
		GUILayout.Label("Debugger attatched: " + System.Diagnostics.Debugger.IsAttached);

		if (GUILayout.Button("Open Debug Tools")) {
			_showDebugWindow = !_showDebugWindow;
		}

		// Show debug window if toggled
		if (_showDebugWindow) {
			_debugWindowRect = GUILayout.Window(0, _debugWindowRect, DebugWindowFunction, "Debug Tools");
		}
	}

	void DebugWindowFunction(int windowID) {

		// Display Logging
		GUILayout.Label("Log List:");

		GUILayout.BeginScrollView(Vector2.zero);

		foreach (string log in Logger.LogList) {
			if (_logFilter != string.Empty) {
				if (!log.ToLower().Contains(_logFilter.ToLower())) {
					continue;
				}
			}
			GUILayout.Label(log);
		}
		GUILayout.EndScrollView();

		if (GUILayout.Button("Clear")) {
			Logger.LogList.Clear();
		}

		_logFilter = GUILayout.TextField(_logFilter);

		// Make the window draggable
		GUI.DragWindow(new Rect(0, 0, 10000, 20));
	}
}