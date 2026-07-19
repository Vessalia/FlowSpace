using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Player
{
	[Serializable]
	public class PlayerAttackMotor
	{
		[SerializeField] private List<RhythmWeapon> weaponPrefabs = new();
		[SerializeField] private Transform weaponMountPoint;

		private List<RhythmWeapon> weapons = new();
		private RhythmWeapon selectedWeapon;

		public void SelectWeapon(int index) => selectedWeapon = weapons[index];

		public void Init()
		{
			weapons.Clear();
			foreach (var prefab in weaponPrefabs)
			{
				var weapon = GameObject.Instantiate(prefab, weaponMountPoint.position, weaponMountPoint.rotation, weaponMountPoint);
				weapons.Add(weapon);
			}

			if (weapons.Count > 0 && selectedWeapon == null) SelectWeapon(0);
		}

		public void Tick(float dt, PlayerIntent intent)
		{
			if (weapons.Count == 0) return;

			var attackIntent = intent.AttackJustPressed;
			if (attackIntent.value)
				selectedWeapon.TryShoot(attackIntent.time);
		}
	}
}
