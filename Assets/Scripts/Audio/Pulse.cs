using System;
using System.Collections.Generic;
using UnityEngine;

class Pulse
{
	private AudioClock clock = null;
	private Metronome metronome = null;

	private Dictionary<BeatSignature, Action<float>> beatHandlers = new();
	private Dictionary<BeatSignature, bool> primed = new();

	private Dictionary<string, Action> markerHandlers = new();

	public void SetClock(AudioClock clock)
	{
		clock.OnTick += Tick;
		this.clock = clock;
	}

	public void SetMetronome(Metronome metronome)
	{
		metronome.OnMarker += HandleMarker;
		this.metronome = metronome;
	}

	~Pulse()
	{
		if (clock != null) clock.OnTick -= Tick;
		if (metronome != null) metronome.OnMarker -= HandleMarker;
	}

	public void RegisterBeatListener(Action<float> listener, BeatSignature signature)
	{
		if (beatHandlers.TryGetValue(signature, out var handler))
		{
			handler += listener;
		}
		else
		{
			beatHandlers.Add(signature, listener);
			primed.Add(signature, true);
		}
	}

	public void DeregisterBeatListener(Action<float> listener, BeatSignature signature)
	{
		if (!beatHandlers.TryGetValue(signature, out var handler)) return;

		handler -= listener;
		if (handler == null)
		{
			beatHandlers.Remove(signature);
			primed.Remove(signature);
		}
	}

	// as more beatlisteners build up, this will eventually desync since we calc BeatPos once per frame
	private bool IsBeatActive(BeatSignature signature, out float beatDelay)
	{
		// get smallest n s.t. offset + n * period > prev → n > (prev - offset) / period
		float n = Mathf.Floor((clock.LastBeatPos - signature.Offset) / signature.Period) + 1;
		beatDelay = clock.BeatPos - (signature.Offset + n * signature.Period);
		return beatDelay >= 0;
	}

	private void Tick()
	{
		var info = metronome.timelineInfo;
		if (info.tempo <= 0) return;

		float secondsPerBeat = 60f / info.tempo;

		foreach (var (signature, handler) in beatHandlers)
		{
			bool inWindow = IsBeatActive(signature, out float beatDelay);
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

	private void HandleMarker(string marker)
	{
		if (markerHandlers.TryGetValue(marker, out var handler))
		{
			handler.Invoke();
		}
	}
}
