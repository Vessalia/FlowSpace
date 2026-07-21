using FMODUnity;
using System.Collections;
using UnityEngine;

public class GruntLaserWeapon : MonoBehaviour
{
	[SerializeField] WeaponProfile profile;
	Transform player;

	[SerializeField] private LineRenderer laserVisual;
	[SerializeField] private EventReference shootSound;
	[SerializeField] private float liveTime = 0.01f;

	private float hideAt;


	public void Awake()
	{
		laserVisual.enabled = false;
	}

	public void SetPlayer(Transform player) => this.player = player;

	public void Fire()
	{
		Vector3 origin = transform.position;
		Vector3 dir = player.position - origin;

		Vector3 endPoint = origin + dir * profile.range;
		if (Physics.Raycast(origin, dir, out RaycastHit hit, profile.range, profile.hitMask))
		{
			endPoint = hit.point;
			if (hit.transform.gameObject.TryGetComponent(out IDamageable damageable))
			{
				damageable.TakeDamage(profile.damage);
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
