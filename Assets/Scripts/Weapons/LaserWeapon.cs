using FMODUnity;
using System.Collections;
using UnityEngine;

public class LaserWeapon : RhythmWeapon
{
	[SerializeField] private LineRenderer laserVisual;
	[SerializeField] private EventReference shootSound;
	[SerializeField] private float liveTime = 0.1f;

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

		Vector3 endPoint = origin + dir * range;
		if (Physics.Raycast(origin, dir, out RaycastHit hit, range, hitMask))
		{
			endPoint = hit.point;
			// probably need to put some IDamageable here from the hit.collider
		}

		laserVisual.SetPosition(0, origin);
		laserVisual.SetPosition(1, endPoint);
		laserVisual.enabled = true;

		hideAt = Clock.Instance.GameTime + liveTime;
		StartCoroutine(HideLaser());
	}

	public void PlayOneShot()
	{
		MusicPlayer.Instance.PlayOneShot(shootSound);
	}

	private IEnumerator HideLaser()
	{
		while (Clock.Instance.GameTime < hideAt) yield return new WaitForSeconds(liveTime);
		laserVisual.enabled = false;
	}
}
