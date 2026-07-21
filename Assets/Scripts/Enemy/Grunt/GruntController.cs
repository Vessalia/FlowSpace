using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class GruntController : BeatListener
{
	[SerializeField] private GruntLaserWeapon weaponPrefab;
	[SerializeField] private Transform weaponMountPoint;

	[SerializeField] private Transform player;

	private GruntLaserWeapon weapon;

	public void Awake()
	{
		weapon = GameObject.Instantiate(weaponPrefab, weaponMountPoint.position, weaponMountPoint.rotation, weaponMountPoint);
		weapon.SetPlayer(player);
	}

	public override void BeatAction(float delay)
	{
		weapon.Fire();
	}
}
