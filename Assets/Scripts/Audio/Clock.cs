using FMOD;
using System.Collections.Generic;
using UnityEngine;

public class Clock : Singleton<Clock>
{
	private Dictionary<int, AudioClock> clocks = new();

	public float GameTime => Time.time;
	public float GameDeltaTime => Time.deltaTime;
	public float MusicTime
	{
		get
		{
			if (clocks.TryGetValue(MusicPlayer.Instance.MusicHandle, out var musicClock))
			{
				return musicClock.PosS;
			}

			return 0;
		}
	}
	public float MusicDeltaTime 
	{
		get
		{
			if (clocks.TryGetValue(MusicPlayer.Instance.MusicHandle, out var musicClock))
			{
				return musicClock.DeltaTime;
			}

			return 0;
		}
	}

	public AudioClock MusicClock => GetAudioClock(MusicPlayer.Instance.MusicHandle);

	private Clock() { }

	public AudioClock RegisterClock(int handle, AudioClock clock)
	{
		clocks[handle] = clock;
		return clocks[handle];
	}

	public void DeregisterClock(int handle)
	{
		clocks.Remove(handle);
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
