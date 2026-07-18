using FMODUnity;
using System.Collections;
using UnityEngine;

public class LaserWeapon : RhythmWeapon
{
	[SerializeField] private LineRenderer laserVisual;
	[SerializeField] private EventReference shootSound;

	public void Awake()
	{
		laserVisual.enabled = false;
	}

	public override void Fire()
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

		StartCoroutine(HideLaserAfter(0.1f));
	}

	public void PlayOneShot()
	{
		MusicPlayer.Instance.PlayOneShot(shootSound);
	}

	private IEnumerator HideLaserAfter(float time)
	{
		yield return new WaitForSeconds(time);
		laserVisual.enabled = false;
	}
}
