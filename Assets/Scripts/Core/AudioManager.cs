using FMOD.Studio;
using FMODUnity;
using System;

public class AudioManager : MonoSingleton<AudioManager>
{
	private EventInstance _musicInstance;
	private AudioManager() { }

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

	public void Destroy()
	{
		if (!_musicInstance.isValid()) return;

		_musicInstance.setUserData(IntPtr.Zero);
		_musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		_musicInstance.release();
	}

	public EventInstance GetMusicInstance()
	{
		return _musicInstance;
	}

	public int GetTimelinePosition()
	{
		_musicInstance.getTimelinePosition(out int position);
		return position;
	}

	public void PlayOneShot(EventReference oneShot)
	{
		RuntimeManager.PlayOneShot(oneShot);
	}
}
