using FMOD;
using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

public class AudioManager : MonoSingleton<AudioManager>
{
	[SerializeField] private EventReference musicEvent;
	private EventInstance _musicInstance;

	protected override void Awake()
	{
		base.Awake();

		SetMusic(musicEvent);

		EditorApplication.pauseStateChanged += (PauseState state) =>
		{
			switch (state)
			{
				case PauseState.Paused:
					Pause();
					break;
				case PauseState.Unpaused:
					Resume();
					break;
			}
		};

		if (EditorApplication.isPaused)
		{
			Pause();
		}
	}

	public void Start()
	{
		StartCoroutine(TrackAudioOnceLoaded());
	}

	public void OnDestroy()
	{
		if (!_musicInstance.isValid()) return;

		_musicInstance.setUserData(IntPtr.Zero);
		_musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		_musicInstance.release();
	}

	private void Update()
	{
		BeatManager.Instance.Update();
	}

	private IEnumerator TrackAudioOnceLoaded()
	{
		ChannelGroup channelGroup;
		while (_musicInstance.isValid() && _musicInstance.getChannelGroup(out channelGroup) != FMOD.RESULT.OK)
		{
			yield return null;
		}

		if (!_musicInstance.isValid())
		{
			UnityEngine.Debug.LogWarning("TrackAudioOnceLoaded: instance became invalid before channel group was ready.");
			yield break;
		}

		BeatManager.Instance.TrackAudio(_musicInstance);
	}

	public void SetMusic(EventReference musicReference)
	{
		if (_musicInstance.isValid())
		{
			_musicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
			_musicInstance.release();
		}

		var musicDescription = RuntimeManager.GetEventDescription(musicReference);
		musicDescription.createInstance(out _musicInstance);

		_musicInstance.start();
	}

	public void Pause()
	{
		_musicInstance.setPaused(true);
	}

	public void Resume()
	{
		_musicInstance.setPaused(false);
	}

	public void PlayOneShot(EventReference oneShot)
	{
		RuntimeManager.PlayOneShot(oneShot);
	}
}
