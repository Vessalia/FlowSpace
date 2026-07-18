using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "NewRhythmAction", menuName = "FlowSpace/Rhythm Action", order = 0)]
abstract class RhythmAction : ScriptableObject
{
	[SerializeField] private BeatSignature signature;
	[SerializeField] private int windowInMS;
	[SerializeField] private List<UnityEvent> actions;

	public bool Invoke()
	{
		if (!InWindow()) return false;

		foreach (var action in actions)
		{
			action.Invoke();
		}

		return true;
	}

	private bool InWindow()
	{
		return Conductor.Instance.InWindow(signature, windowInMS);
	}
}
