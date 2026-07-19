using UnityEngine;

public abstract class RhythmWeapon : MonoBehaviour
{
	[SerializeField] private RhythmActionDefinition shootDef;
	private RhythmAction shoot;

	// this might not be the best way to do this, maybe a weapon profile SO?
	[SerializeField] protected float range = 50f;
	[SerializeField] protected LayerMask hitMask;

	public virtual void Awake() => shoot = new RhythmAction(shootDef);
	protected virtual void OnEnable() => shoot.OnTriggered += Fire;
	protected virtual void OnDestroy() => shoot.OnTriggered -= Fire;

	protected abstract void Fire();

	public void TryShoot(float time) => shoot.TryInvoke(time);
}
