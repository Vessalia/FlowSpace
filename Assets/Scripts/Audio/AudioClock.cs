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
	private int sampleRate = 44100;
	private ulong anchorDsp = 0;
	private double dspRemainder = 0;
	private int anchorBeat = 0;
	private float bpm = 0;

	private int loop = 0;
	private int lastAbsoluteBeat = 0;

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

	public float SecondsToBeats(float time) => time * bpm / 60f;


	public event Action OnTick;

	public AudioClock(ChannelGroup group, int sampleRate)
	{
		this.group = group;
		this.sampleRate = sampleRate;

		currPos.sample = 0;
	}

	public void NotifyBeat(int bar, int beat, int beatsPerBar, float bpm)
	{
		int absoluteBeat = beat - 1 + beatsPerBar * (bar - 1); // FMOD is 1 indexed
		if (lastAbsoluteBeat > 0 && absoluteBeat < lastAbsoluteBeat) loop += lastAbsoluteBeat;
		lastAbsoluteBeat = absoluteBeat;

		int trueBeat = absoluteBeat + loop;

		if (this.bpm == 0)
		{
			this.bpm = bpm;
		}
		else if (bpm != this.bpm)
		{
			double samplesPerBeat = sampleRate * 60.0 / this.bpm;

			double exactSample = anchorDsp + dspRemainder + (trueBeat - anchorBeat) * samplesPerBeat;
			ulong roundedSample = (ulong)Math.Round(exactSample); // subsamples don't make sense in this context

			dspRemainder = exactSample - roundedSample; // carries forward what rounding dropped
			anchorDsp = roundedSample;
			anchorBeat = trueBeat;
			this.bpm = bpm;
		}
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
