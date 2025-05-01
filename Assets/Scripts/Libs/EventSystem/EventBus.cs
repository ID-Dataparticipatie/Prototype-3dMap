using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class EventBus : MonoBehaviour {
	[Header("Debug settings")]
	[SerializeField] private bool _showLogging = false;

	[SerializeField, Tooltip("Enables logging of subscription calls")] private bool _showSubscriptionLogs = false;
	[SerializeField, Tooltip("Enables logging of event triggers")] private bool _showTriggerLogs = false;

	private static string _logname = "EventBus";


	private Hashtable _eventHash = new();
	private static EventBus _eventBus;
	public static EventBus Instance {
		get {
			// Check if an instance exists. if not grab the one (which should be) present in the scene.
			if (!_eventBus) {
				_eventBus = FindAnyObjectByType<EventBus>();

				if (_eventBus) {
					_eventBus.Init();
				}
				else {
					Logger.LogError(_logname, "No EventBus found in the scene!");
				}
			}
			return _eventBus;
		}
	}

	private void Init() {
		_eventBus._eventHash ??= new Hashtable();
	}

	private void Awake() {
		if (_eventBus == null || _eventBus == this) {
			_eventBus = this;
		}
		else {
			Logger.LogWarning(_logname, "Multiple Instances found! Exiting...");
			Destroy(gameObject);
			return;
		}
		DontDestroyOnLoad(Instance);
	}

	public void Subscribe<T>(EventType eventName, UnityAction<T> listener) {
		UnityEvent<T> newEvent;

		string key = GetKey<T>(eventName);

		if (Instance._eventHash.ContainsKey(key)) {
			newEvent = (UnityEvent<T>)Instance._eventHash[key];
			newEvent.AddListener(listener);
			Instance._eventHash[key] = newEvent;
		}
		else {
			newEvent = new UnityEvent<T>();
			newEvent.AddListener(listener);
			Instance._eventHash.Add(key, newEvent);
		}

		if (_showSubscriptionLogs) {
			sendToLogger($"{listener.Target} subscribed to event {eventName}<{typeof(T).Name}>");
		}
	}


	public void Subscribe(EventType eventName, UnityAction listener) {
		UnityEvent newEvent;

		if (Instance._eventHash.ContainsKey(eventName)) {
			newEvent = (UnityEvent)Instance._eventHash[eventName];
			newEvent.AddListener(listener);
			Instance._eventHash[eventName] = newEvent;
		}
		else {
			newEvent = new UnityEvent();
			newEvent.AddListener(listener);
			Instance._eventHash.Add(eventName, newEvent);
		}

		if (_showSubscriptionLogs) {
			sendToLogger($"{listener} subscribed to event {eventName}");
		}
	}


	public void Unsubscribe<T>(EventType eventName, UnityAction<T> listener) {
		UnityEvent<T> newEvent;
		string key = GetKey<T>(eventName);

		if (Instance._eventHash.ContainsKey(key)) {
			newEvent = (UnityEvent<T>)Instance._eventHash[key];
			newEvent.RemoveListener(listener);
			Instance._eventHash[key] = newEvent;


			if (_showSubscriptionLogs) {
				sendToLogger($"{listener.Target} unsubscribed from event {eventName}<{typeof(T).Name}>");
			}
		}
	}

	public void Unsubscribe(EventType eventName, UnityAction listener) {
		UnityEvent newEvent;

		if (Instance._eventHash.ContainsKey(eventName)) {
			newEvent = (UnityEvent)Instance._eventHash[eventName];
			newEvent.RemoveListener(listener);
			Instance._eventHash[eventName] = newEvent;

			if (_showSubscriptionLogs) {
				sendToLogger($"{listener} unsubscribed from event {eventName}");
			}
		}
	}

	public void TriggerEvent<T>(EventType eventName, T val) {
		UnityEvent<T> newEvent;
		string key = GetKey<T>(eventName);

		if (Instance._eventHash.ContainsKey(key)) {
			newEvent = (UnityEvent<T>)Instance._eventHash[key];
			newEvent.Invoke(val);

			if (_showTriggerLogs) {
				sendToLogger($"Event {eventName} was triggerd with value {typeof(T).Name}({val})");
			}
		}
	}

	public void TriggerEvent(EventType eventName) {
		UnityEvent newEvent;

		if (Instance._eventHash.ContainsKey(eventName)) {
			newEvent = (UnityEvent)Instance._eventHash[eventName];
			newEvent.Invoke();

			if (_showTriggerLogs) {
				sendToLogger($"Event {eventName} was triggerd");
			}
		}
	}

	private string GetKey<T>(EventType eventtype) {
		Type type = typeof(T);
		return $"{type}_{eventtype}";
	}

	private void sendToLogger(string text) {
		if (_showLogging) {
			Logger.Log(_logname, text);
		}
	}
}
