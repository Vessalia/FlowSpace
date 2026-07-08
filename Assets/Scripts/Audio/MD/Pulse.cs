using System;
using System.Collections.Generic;
using UnityEngine;

class Pulse
{
	private string trackId;
	private Metronome metronome;

	private Dictionary<BeatSignature, BeatEvent> beatHandlers = new();
	private Dictionary<BeatSignature, bool> primed = new();

	private Dictionary<string, Action> markerHandlers = new();

	public Pulse(string trackId, Metronome metronome)
	{
		this.trackId = trackId;
		this.metronome = metronome;
	}

	public void Tick()
	{
		var clock = Clock.Instance.GetAudioClock(trackId);
		var info = metronome.timelineInfo;

		float secondsPerBeat = info.tempo == 0 ? -1 : 60f / info.tempo;
		if (secondsPerBeat < 0) return;

		float prevBeatPos = clock.GetLastBeatPosition(info.tempo);
		float currBeatPos = clock.GetBeatPosition(info.tempo);

		foreach (var (signature, handler) in beatHandlers)
		{
			bool inWindow = IsBeatActive(signature, prevBeatPos, currBeatPos, out float beatDelay);
			if (inWindow && primed[signature])
			{
				handler.Invoke(beatDelay * secondsPerBeat);
				primed[signature] = false;
			}
			else if (!inWindow && !primed[signature])
			{
				primed[signature] = true;
			}
		}

		Action onMarker = () =>
		{
			if (markerHandlers.TryGetValue(info.lastMarker, out var handler))
			{
				handler.Invoke();
			}
		};

		metronome.HandleFlags(null, onMarker);
	}

	private bool IsBeatActive(BeatSignature signature, float prevBeatPos, float currBeatPos, out float beatDelay)
	{
		// get smallest n s.t. offset + n * period > prev → n > (prev - offset) / period
		float n = Mathf.Floor((prevBeatPos - signature.Offset) / signature.Period) + 1;
		beatDelay = currBeatPos - (signature.Offset + n * signature.Period);
		return beatDelay >= 0;
	}

	public void RegisterBeatListener(Action<float> listener, float period, float offset = 0)
	{
		var sig = new BeatSignature(period, offset);
		if (beatHandlers.TryGetValue(sig, out var handler))
		{
			handler.RegisterBeatListener(listener);
		}
		else
		{
			var beatHandler = new BeatEvent(period, offset);
			beatHandler.RegisterBeatListener(listener);
			beatHandlers.Add(sig, beatHandler);
			primed.Add(sig, true);
		}
	}

	public void DeregisterBeatListener(Action<float> listener, float period, float offset = 0)
	{
		var sig = new BeatSignature(period, offset);
		if (!beatHandlers.TryGetValue(sig, out var handler)) return;

		handler.DeregisterBeatListener(listener);
		if (!handler.HasAction)
		{
			beatHandlers.Remove(sig);
			primed.Remove(sig);
		}
	}
}
