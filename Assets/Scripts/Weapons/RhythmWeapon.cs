using UnityEngine;

public abstract class RhythmWeapon : MonoBehaviour
{
	[SerializeField] public RhythmAction shoot;
	[SerializeField] protected float range = 50f; // this might not be the best way to do this
	[SerializeField] protected LayerMask hitMask;

	public abstract void Fire();
}
