using FMOD;
using FMODUnity;
using System.Collections;
using UnityEngine;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;

public class Conductor : MonoSingleton<Conductor>
{
	[SerializeField] EventReference music;
	private Pulse pulse;

	void Start()
	{
		int musicHandle = MusicPlayer.Instance.PlayMusic(music);
		StartCoroutine(AwaitChannelGroup(musicHandle));
	}

	private void OnDestroy()
	{
		Clock.Instance.DeregisterClock(MusicPlayer.Instance.MusicHandle);
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

			Metronome metronome = new Metronome(audio);
			AudioClock clock = new AudioClock(group, sampleRate);

			clock.OnTick += metronome.HandleFlags;
			metronome.OnBeat += (int beat, float tempo) => { clock.NotifyBeat(beat, tempo); };

			pulse = new Pulse(clock, metronome);

			Clock.Instance.RegisterClock(handle, clock);
		}
	}

	// The timing of this is really sensitive, needs to be before anything that cares about audio clock deltatimes
	// Maybe just have this script run earlier?
	void Update()
	{
		Clock.Instance.TickAll();
	}
}
