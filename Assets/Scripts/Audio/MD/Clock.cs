using FMOD;
using System.Collections.Generic;
using UnityEngine;

public class Clock : Singleton<Clock>
{
	private Dictionary<string, AudioClock> tracks = new();

	public float GameDeltaTime => Time.deltaTime;

	// this should probably return a handle instead of taking a string for the id
	public void RegisterTrack(string id, ChannelGroup group, int sampleRate) => tracks[id] = new AudioClock(group, sampleRate);
	public void DeregisterTrack(string id)
	{
		if (tracks.ContainsKey(id))
		{
			tracks.Remove(id);
		}
	}
	
	public AudioClock GetAudioClock(string id) => tracks[id];

	public void TickAll()
	{
		foreach (var clock in tracks.Values)
		{
			clock.Tick();
		}
	}
}
