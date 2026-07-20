using FMODUnity;
using System.Collections;
using UnityEngine;

public class LaserWeapon : RhythmWeapon
{
	[SerializeField] private LineRenderer laserVisual;
	[SerializeField] private EventReference shootSound;
	[SerializeField] private float liveTime = 0.01f;

	private float hideAt;

	public override void Awake()
	{
		base.Awake();
		laserVisual.enabled = false;
	}

	protected override void Fire()
	{
		Vector3 origin = transform.position;
		Vector3 dir = transform.forward;

		Vector3 endPoint = origin + dir * Range;
		if (Physics.Raycast(origin, dir, out RaycastHit hit, Range, HitMask))
		{
			endPoint = hit.point;
			if (hit.transform.gameObject.TryGetComponent(out IDamageable damageable))
			{
				damageable.TakeDamage(Damage);
			}
		}

		laserVisual.SetPosition(0, origin);
		laserVisual.SetPosition(1, endPoint);
		laserVisual.enabled = true;

		PlayOneShot();

		hideAt = Clock.Instance.GameTime + liveTime;
		StartCoroutine(HideLaser());
	}

	public void PlayOneShot()
	{
		MusicPlayer.Instance.PlayOneShot(shootSound, transform.position);
	}

	private IEnumerator HideLaser()
	{
		while (Clock.Instance.GameTime < hideAt) yield return new WaitForSeconds(liveTime);
		laserVisual.enabled = false;
	}
}
