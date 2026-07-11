using System;
using System.Collections.Generic;
using UnityEngine;

class Pulse
{
	private AudioClock clock;
	private Metronome metronome;

	private Dictionary<BeatSignature, BeatEvent> beatHandlers = new();
	private Dictionary<BeatSignature, bool> primed = new();

	private Dictionary<string, Action> markerHandlers = new();

	public Pulse(AudioClock clock, Metronome metronome)
	{
		this.clock = clock;
		this.metronome = metronome;

		clock.OnTick += Tick;
		metronome.OnMarker += HandleMarker;
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

	private bool IsBeatActive(BeatSignature signature, float prevBeatPos, float currBeatPos, out float beatDelay)
	{
		// get smallest n s.t. offset + n * period > prev → n > (prev - offset) / period
		float n = Mathf.Floor((prevBeatPos - signature.Offset) / signature.Period) + 1;
		beatDelay = currBeatPos - (signature.Offset + n * signature.Period);
		return beatDelay >= 0;
	}

	private void Tick()
	{
		var info = metronome.timelineInfo;
		if (info.tempo <= 0) return;

		float secondsPerBeat = 60f / info.tempo;
		float prevBeatPos = clock.LastBeatPos;
		float currBeatPos = clock.BeatPos;

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
	}

	private void HandleMarker()
	{
		if (markerHandlers.TryGetValue(metronome.timelineInfo.lastMarker, out var handler))
		{
			handler.Invoke();
		}
	}
}
