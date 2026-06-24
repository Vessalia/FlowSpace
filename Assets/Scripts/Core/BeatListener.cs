using UnityEngine;

public abstract class BeatListener : MonoBehaviour
{
	[SerializeField]
	protected float beat = 1;

	[SerializeField]
	protected float offset = 0;

	public void Start()
	{
		BeatManager.Instance.RegisterBeatListener(BeatAction, beat, offset);
	}

	private void OnDestroy()
	{
		BeatManager.Instance.DeregisterBeatListener(BeatAction, beat, offset);
	}

	public abstract void BeatAction();
}
