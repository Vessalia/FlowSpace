using FMOD;
using FMODUnity;
using System.Collections;
using UnityEngine;

public struct BeatSignature
{
	public float Period { get; private set; }
	public float Offset { get; private set; }

	public BeatSignature(float period, float offset) : this()
	{
		this.Period = period;
		this.Offset = offset;
	}
};

public class Referee : MonoBehaviour
{
	[SerializeField] EventReference music;
	private Pulse pulse = null;

	void Start()
	{
		MusicPlayer.Instance.SetAudio(music);
		StartCoroutine(SetChannelGroup());
	}

	private IEnumerator SetChannelGroup()
	{
		ChannelGroup group;
		while (MusicPlayer.Instance.getChannelGroup(out group) != RESULT.OK)
		{
			yield return null;
		}

		if (MusicPlayer.Instance.GetAudioInstance(out var audio))
		{
			RuntimeManager.CoreSystem.getSoftwareFormat(out int sampleRate, out _, out _);
			string flowTrackId = "flow";
			Clock.Instance.RegisterTrack(flowTrackId, group, sampleRate);
			var metronome = new Metronome(audio);
			pulse = new Pulse(flowTrackId, metronome);
		}
	}

	// a part of me feels like these tick methods are bad, we should realistically be doing callbacks for at least one of these
	void Update()
	{
		Clock.Instance.TickAll();
		pulse?.Tick();
	}
}
