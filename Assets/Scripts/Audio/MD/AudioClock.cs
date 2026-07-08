using FMOD;

public class AudioClock
{
	private ChannelGroup group;
	private int sampleRate = 44100;
	private ulong lastDsp;
	private ulong dsp;

	public float DeltaTime { get; private set; }

	public AudioClock(ChannelGroup group, int sampleRate)
	{
		this.group = group;
		this.sampleRate = sampleRate;
	}

	public void Tick()
	{
		group.getDSPClock(out dsp, out _);
		DeltaTime = dsp >= lastDsp ? (float)(dsp - lastDsp) / sampleRate : (float)(lastDsp - dsp) / sampleRate;
		lastDsp = dsp;
	}

	public float GetBeatPosition(float bpm) => dsp * bpm / (60 * sampleRate);
	public float GetLastBeatPosition(float bpm) => lastDsp * bpm / (60 * sampleRate);
}
