using UnityEngine;

public abstract class RhythmWeapon : MonoBehaviour
{
	[SerializeField] private WeaponProfile profile;
	private RhythmAction shoot;

	protected float Range => profile.range;
	protected float Damage => profile.damage;
	protected LayerMask HitMask => profile.hitMask;

	public virtual void Awake() => shoot = new RhythmAction(profile.def);
	protected virtual void OnEnable() => shoot.OnTriggered += Fire;
	protected virtual void OnDestroy() => shoot.OnTriggered -= Fire;

	protected abstract void Fire();

	public void TryShoot(float time) => shoot.TryInvoke(time);
}
