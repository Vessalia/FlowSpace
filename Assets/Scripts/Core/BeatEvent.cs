using System;
using System.Collections.Generic;
using System.Text;

class BeatEvent
{
	public readonly float period;
	public readonly float offset;

	private event Action<float> action;

	public bool HasAction { get { return action != null; } }

	public BeatEvent(float period, float offset)
	{
		this.period = period;
		this.offset = offset;
	}

	public void RegisterBeatListener(Action<float> listener)
	{
		action += listener;
	}

	public void DeregisterBeatListener(Action<float> listener)
	{
		action -= listener;
	}

	public void Invoke(float delay)
	{
		action?.Invoke(delay);
	}
}
