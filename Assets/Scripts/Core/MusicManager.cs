using FMOD.Studio;
using FMODUnity;
using UnityEditor;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
	[SerializeField] private EventReference musicEvent;

	void Awake()
	{
		AudioManager.Instance.SetMusic(musicEvent);

		EditorApplication.pauseStateChanged += (PauseState state) =>
		{
			switch (state)
			{
				case PauseState.Paused:
					AudioManager.Instance.Pause();
					break;
				case PauseState.Unpaused:
					AudioManager.Instance.Resume();
					break;
			}
		};

		if (EditorApplication.isPaused)
		{
			AudioManager.Instance.Pause();
		}
	}

	private void OnDestroy()
	{
		AudioManager.Instance.Destroy();
	}
}
