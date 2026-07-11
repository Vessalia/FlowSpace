using FMOD;
using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MusicPlayer : Singleton<MusicPlayer>
{
	private Dictionary<int, EventInstance> audioInstances;
	private int nextHandle = 0;

	public int MusicHandle { get; private set; } = -1;

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

	public int PlayMusic(EventReference musicEvent)
	{
		musicHandle = PlayAudio(musicEvent);
		return musicHandle;
	}

	public int PlayAudio(EventReference audioEvent)
	{
		var audioDescription = RuntimeManager.GetEventDescription(audioEvent);
		audioDescription.createInstance(out EventInstance audioInstance);

		audioInstance.start();

		int handle = nextHandle++;
		audioInstances[handle] = audioInstance;
		return handle;
	}

	public void ClearAudio()
	{
		foreach (var audioInstance in audioInstances.Values)
		{
			if (audioInstance.isValid())
			{
				audioInstance.setUserData(IntPtr.Zero);
				audioInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
				audioInstance.release();
			}
		}
	}

	public void Pause(int handle = -1)
	{
		if (handle < 0)
		{
			foreach (var audioInstance in audioInstances.Values)
			{
				audioInstance.setPaused(true);
			}
		}
		else
		{
			audioInstances[handle].setPaused(true);
		}
	}

	public void Resume(int handle = -1)
	{
		if (handle < 0)
		{
			foreach (var audioInstance in audioInstances.Values)
			{
				audioInstance.setPaused(false);
			}
		}
		else
		{
			audioInstances[handle].setPaused(false);
		}
	}

	public bool GetAudioInstance(int handle, out EventInstance audioInstance)
	{
		audioInstance = audioInstances[handle];
		return audioInstance.isValid();
	}

	public RESULT getChannelGroup(int handle, out ChannelGroup channelGroup)
	{
		return audioInstances[handle].getChannelGroup(out channelGroup);
	}

	public void PlayOneShot(EventReference oneShot)
	{
		RuntimeManager.PlayOneShot(oneShot);
	}
}
