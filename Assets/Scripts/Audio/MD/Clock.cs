using FMOD;
using System.Collections.Generic;
using UnityEngine;

public class Clock : Singleton<Clock>
{
	private Dictionary<int, AudioClock> clocks = new();

	public float GameDeltaTime => Time.deltaTime;

	public AudioClock RegisterClock(int handle, AudioClock clock)
	{
		clocks[handle] = clock;
		return clocks[handle];
	}

	public void DeregisterClock(int handle)
	{
		if (clocks.ContainsKey(handle))
		{
			clocks.Remove(handle);
		}
	}
	
	public AudioClock GetAudioClock(int handle) => clocks[handle];

	public void TickAll()
	{
		foreach (var clock in clocks.Values)
		{
			clock.Tick();
		}
	}
}
