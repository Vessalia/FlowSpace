using FMOD;
using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Metronome;

public class Conductor : MonoSingleton<Conductor>
{
	[SerializeField] EventReference music;

	private Pulse pulse = new();
	private Metronome metronome;

	void Start()
	{
		int musicHandle = MusicPlayer.Instance.PlayMusic(music);
		StartCoroutine(AwaitChannelGroup(musicHandle));
	}

	private void OnDestroy()
	{
		Clock.Instance.DeregisterClock(MusicPlayer.Instance.MusicHandle);
		MusicPlayer.Instance.ClearAudio();
	}

	private IEnumerator AwaitChannelGroup(int handle)
	{
		ChannelGroup group;
		while (MusicPlayer.Instance.getChannelGroup(handle, out group) != RESULT.OK)
		{
			yield return null;
		}

		if (MusicPlayer.Instance.GetAudioInstance(handle, out var audio))
		{
			RuntimeManager.CoreSystem.getSoftwareFormat(out int sampleRate, out _, out _);

			metronome = new Metronome(audio, MusicPlayer.Instance.MusicLength);
			AudioClock clock = new AudioClock(group, sampleRate);

			clock.OnTick += metronome.HandleFlags;
			metronome.OnBeat += (int bar, int beat, int beatsPerBar, float tempo) => 
			{
				clock.NotifyBeat(bar, beat, beatsPerBar, tempo); 
			};

			pulse.SetClock(clock);
			pulse.SetMetronome(metronome);

			Clock.Instance.RegisterClock(handle, clock);
		}
	}

	public bool InWindow(BeatSignature signature, int windowInMS, float requestTime)
	{
		// we need n s.t. offset + n * period is as close to currBeat as possible
		AudioClock musicClock = Clock.Instance.MusicClock;
		float n = Mathf.Round((musicClock.SecondsToBeats(requestTime) - signature.Offset) / signature.Period);
		float nearestBeatTimeMS = (signature.Offset + n * signature.Period) * musicClock.MSPerBeat;
		return Mathf.Abs((requestTime * 1000) - nearestBeatTimeMS) <= windowInMS;
	}

	public void RegisterBeatListener(Action<float> beatAction, BeatSignature signature)
	{
		pulse.RegisterBeatListener(beatAction, signature);
	}

	public void DeregisterBeatListener(Action<float> beatAction, BeatSignature signature)
	{
		pulse.DeregisterBeatListener(beatAction, signature);
	}

	void Update()
	{
		Clock.Instance.TickAll();
	}

#if UNITY_EDITOR
	void OnGUI()
	{
		if (metronome != null)
		{
			var content = "";
			content += $"**Music Manager Debug**\n\n";
			content += $"Length (ms): {metronome.timelineInfo.length}, Tempo: {metronome.timelineInfo.tempo}\n\n";
			content += $"Playback Position (ms): {metronome.timelineInfo.position}\n";
			content += $"Current Bar: {metronome.timelineInfo.bar}, Current Beat: {metronome.timelineInfo.beat}\n\n";
			content += $"Last Marker: {metronome.timelineInfo.lastMarker}";

			GUI.Label(new Rect(10, 160, 300, 150), content, GUI.skin.textArea);
		}
	}
#endif
}
