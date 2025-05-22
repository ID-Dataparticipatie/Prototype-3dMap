using System;
using System.Collections.Generic;
using UnityEngine;

public static class Logger {
	private static int _maxLogCount = 50;
	public static List<string> LogList = new();

    public static void Log(string name, object message) {
		string logMessage = $"[{name}]: {message}";
		#if UNITY_EDITOR
			Debug.Log($"<color=silver>[  <color=lime>{name}</color>  ]: " + message + "</color>");
		#else
			Debug.LogError(logMessage);
		#endif

		#if DEVELOPMENT_BUILD
			Console.WriteLine(logMessage);
			System.Diagnostics.Debugger.Log(0, name, message.ToString());
		#endif
		AddLog(name, logMessage);
    }

    public static void LogWarning(string name, object message) {
		string logMessage = $"[{name}]: {message}";
		#if UNITY_EDITOR
			Debug.Log($"<color=silver>[  <color=orange>{name}</color>  ]: " + message + "</color>");
		#else
			Debug.LogError(logMessage);
		#endif

		#if DEVELOPMENT_BUILD
			Console.WriteLine(logMessage);
			System.Diagnostics.Debugger.Log(0, name, message.ToString());
		#endif
		AddLog(name, logMessage);
    }

    public static void LogError(string name, object message) {
		string logMessage = $"[{name}]: {message}";
		#if UNITY_EDITOR
			Debug.LogError($"<color=silver>[  <color=red>{name}</color>  ]: " + message + "</color>");
		#else
			Debug.LogError(logMessage);
		#endif

		#if DEVELOPMENT_BUILD
			Console.WriteLine(logMessage);
			System.Diagnostics.Debugger.Log(0, name, message.ToString());
		#endif
		AddLog(name, logMessage);
    }

	private static void AddLog(string name, object message) {
		if (LogList.Count >= _maxLogCount) {
			LogList.RemoveAt(0);
		}
		LogList.Add($"[{name}]: {message}");
	}
}