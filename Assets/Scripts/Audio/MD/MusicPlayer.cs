using FMOD;
using FMOD.Studio;
using FMODUnity;
using System;
using UnityEditor;
using UnityEngine;

public class MusicPlayer : Singleton<MusicPlayer>
{
	private EventInstance audioInstance;

	MusicPlayer()
	{
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

	~MusicPlayer()
	{
		ClearAudio();
	}

	public void SetAudio(EventReference audioEvent)
	{
		ClearAudio();

		var audioDescription = RuntimeManager.GetEventDescription(audioEvent);
		audioDescription.createInstance(out audioInstance);

		audioInstance.start();
	}

	public void ClearAudio()
	{
		if (audioInstance.isValid())
		{
			audioInstance.setUserData(IntPtr.Zero);
			audioInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
			audioInstance.release();
		}
	}

	public void Pause()
	{
		audioInstance.setPaused(true);
	}

	public void Resume()
	{
		audioInstance.setPaused(false);
	}

	public bool GetAudioInstance(out EventInstance instance)
	{
		instance = audioInstance;
		return instance.isValid();
	}

	public RESULT getChannelGroup(out ChannelGroup channelGroup)
	{
		return audioInstance.getChannelGroup(out channelGroup);
	}

	public void PlayOneShot(EventReference oneShot)
	{
		RuntimeManager.PlayOneShot(oneShot);
	}
}
