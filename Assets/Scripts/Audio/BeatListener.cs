using UnityEngine;

public abstract class BeatListener : MonoBehaviour
{
	[SerializeField] protected BeatSignature signature = new();

	public virtual void Start()
	{
		Conductor.Instance.RegisterBeatListener(BeatAction, signature);
	}

	public virtual void OnDestroy()
	{
		Conductor.Instance.DeregisterBeatListener(BeatAction, signature);
	}

	public abstract void BeatAction(float delay);
}
