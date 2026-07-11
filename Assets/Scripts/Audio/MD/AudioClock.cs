using FMOD;
using System;

public class AudioClock
{
	private ChannelGroup group;
	private int sampleRate = 44100;
	private ulong lastDsp;
	private ulong dsp;

	private ulong anchorDsp;
	private int anchorBeat = 0;

	private float bpm = 0;
	private bool initialized = false;

	public float LastBeatPos { get; private set; } = 0;
	public float BeatPos { get; private set; } = 0;
	public float DeltaTime { get; private set; } = 0;

	public event Action OnTick;

	public AudioClock(ChannelGroup group, int sampleRate)
	{
		this.group = group;
		this.sampleRate = sampleRate;
	}

	// we need to handle bpm changes ON beats, not mid beat
	public void NotifyBeat(int beat, float bpm)
	{
		this.bpm = bpm;
		anchorDsp = lastDsp;
		anchorBeat = beat;
	}

	public void Tick()
	{
		group.getDSPClock(out dsp, out _);

		if (!initialized)
		{
			lastDsp = dsp;
			initialized = true;
		}

		DeltaTime = (float)(dsp - lastDsp) / sampleRate;
		lastDsp = dsp;

		LastBeatPos = BeatPos;
		
		if (bpm > 0)
		{
			float samplesPerBeat = sampleRate * 60f / bpm;
			BeatPos = anchorBeat + (dsp - anchorDsp) / samplesPerBeat;
		}

		OnTick?.Invoke();
	}
}
