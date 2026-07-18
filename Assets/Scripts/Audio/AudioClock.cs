using FMOD;
using System;

public class AudioClock
{
	private struct AudioPosition
	{
		public ulong sample;

		public float PosBeat(float bpm, int sampleRate, int anchorBeat, double anchorDsp)
		{
			if (bpm <= 0) return 0;

			double samplesPerBeat = sampleRate * 60.0 / bpm;
			return (float)(anchorBeat + (sample - anchorDsp) / samplesPerBeat);
		}
		public float PosMS(int sampleRate) => 1000 * PosS(sampleRate);
		public float PosS(int sampleRate) => sample / ((float)sampleRate);
	};

	private ChannelGroup group;
	public int sampleRate = 44100;
	public double anchorDsp = 0;
	public int anchorBeat = 0;
	public float bpm = 0;

	private bool initialized = false;

	private AudioPosition lastPos;
	private AudioPosition currPos;

	public float LastBeatPos => lastPos.PosBeat(bpm, sampleRate, anchorBeat, anchorDsp);
	public float BeatPos => currPos.PosBeat(bpm, sampleRate, anchorBeat, anchorDsp);
	public float PosS => currPos.PosS(sampleRate);
	public float PosMS => currPos.PosMS(sampleRate);
	public float DeltaTime => currPos.PosS(sampleRate) - lastPos.PosS(sampleRate);

	public float SecondsPerBeat => 60.0f / bpm;
	public float MSPerBeat => 1000 * SecondsPerBeat;


	public event Action OnTick;

	public AudioClock(ChannelGroup group, int sampleRate)
	{
		this.group = group;
		this.sampleRate = sampleRate;

		currPos.sample = 0;
	}

	public void NotifyBeat(int beat, float bpm)
	{
		if (bpm > 0)
		{
			double prevSamplesPerBeat = sampleRate * 60.0 / bpm;
			anchorDsp += (beat - anchorBeat) * prevSamplesPerBeat;
		}
		else
		{
			anchorDsp = currPos.sample;
		}

		anchorBeat = beat;
		this.bpm = bpm;
	}

	public void Tick()
	{
		lastPos = currPos;

		group.getDSPClock(out currPos.sample, out _);

		if (!initialized)
		{
			lastPos = currPos;
			initialized = true;
		}

		OnTick?.Invoke();
	}
}
