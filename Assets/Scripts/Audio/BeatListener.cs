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

	// delay corresponds to the time difference between when this function was called
	// and when the desired beat actually occured
	public abstract void BeatAction(float delay);
}
