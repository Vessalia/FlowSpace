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
		private int selectedIndex = 0;

		public void Init()
		{
			weapons.Clear();
			foreach (var prefab in weaponPrefabs)
			{
				var weapon = GameObject.Instantiate(prefab, weaponMountPoint.position, weaponMountPoint.rotation, weaponMountPoint);
				weapons.Add(weapon);
			}

			if (weapons.Count > 0 && selectedWeapon == null)
			{
				selectedWeapon = weapons[selectedIndex];
			}
		}

		public void Tick(float dt, PlayerIntent intent)
		{
			if (weapons.Count == 0) return;

			if (intent.Next && !intent.Previous)
			{
				selectedIndex = (selectedIndex + 1) % weapons.Count;
				selectedWeapon = weapons[selectedIndex];
			}
			else if (intent.Previous)
			{
				selectedIndex = (selectedIndex - 1 + weapons.Count) % weapons.Count;
				selectedWeapon = weapons[selectedIndex];
			}

			var attackIntent = intent.AttackJustPressed;
			if (attackIntent.value)
			{
				selectedWeapon.TryShoot(attackIntent.time);
			}
		}
	}
}
