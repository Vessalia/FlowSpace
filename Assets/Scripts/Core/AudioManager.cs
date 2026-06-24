using FMOD.Studio;
using FMODUnity;
using System;
using UnityEditor;
using UnityEngine;

public class AudioManager : MonoSingleton<AudioManager>
{
	[SerializeField] private EventReference musicEvent;
	private EventInstance _musicInstance;

	protected override void Awake()
	{
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

		DontDestroyOnLoad(gameObject);
	}

	public void OnDestroy()
	{
		if (!_musicInstance.isValid()) return;

		_musicInstance.setUserData(IntPtr.Zero);
		_musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		_musicInstance.release();
	}

	public void SetMusic(EventReference musicReference)
	{
		if (_musicInstance.isValid())
		{
			_musicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
			_musicInstance.release();
		}

		var musicDescription = RuntimeManager.GetEventDescription(musicReference);
		musicDescription.loadSampleData();
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

	public EventInstance GetMusicInstance()
	{
		return _musicInstance;
	}

	public void PlayOneShot(EventReference oneShot)
	{
		RuntimeManager.PlayOneShot(oneShot);
	}
}
