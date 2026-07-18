using System;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "NewRhythmAction", menuName = "FlowSpace/Rhythm Action", order = 0)]
public class RhythmAction : ScriptableObject
{
	[Tooltip("Beat period and offset this action checks against")]
	[SerializeField] private BeatSignature signature = new BeatSignature(1, 0);

	[Tooltip("Timing window around this actions beats, in MS")]
	[SerializeField] private int windowInMS = 40;

	[Tooltip("Configurable side-effects")]
	[SerializeField] private UnityEvent unityEvent;

	/// <summary>Code-level subscriptions for methods to call on certain Invocation methods. Make sure you unsubscribe when destroying!</summary>
	public event Action OnTriggered;

	/// <summary>True if the current time falls within one of this action's beat windows</summary>
	public bool InWindow => Conductor.Instance.InWindow(signature, windowInMS);

	/// <summary>Fires the Unity Event, and then the Action</summary>
	public void Invoke()
	{
		if (!InWindow) return;
		unityEvent?.Invoke();
		OnTriggered?.Invoke();
	}

	/// <summary>Fires the Action, and then the Unity Event</summary>
	public void Evoke()
	{
		if (!InWindow) return;
		OnTriggered?.Invoke();
		unityEvent?.Invoke();
	}

	/// <summary>Fires only the Unity Event</summary>
	public void Event()
	{
		if (!InWindow) return;
		unityEvent?.Invoke();
	}

	/// <summary>Fires only the Action</summary>
	public void Trigger()
	{
		if (!InWindow) return;
		OnTriggered?.Invoke();
	}
}
