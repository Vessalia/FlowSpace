using UnityEngine;

public abstract class BeatListener : MonoBehaviour
{
	[SerializeField] protected float period = 1;

	[Range (0f, 1f)]
	[SerializeField] protected float offset = 0;

	public virtual void Start()
	{
		Pulse.Instance.RegisterBeatListener(BeatAction, period, offset);
	}

	public virtual void OnDestroy()
	{
		Pulse.Instance.DeregisterBeatListener(BeatAction, period, offset);
	}

	public abstract void BeatAction(float delay);
}
