using UnityEngine;

public abstract class PlayerWeapon : MonoBehaviour
{
	private Judge judge;

	private void Fire()
	{
		// coordinate with judge

		OnFire();
	}

	protected abstract void OnFire();

	protected virtual void Update()
	{
		Fire();
	}
}
