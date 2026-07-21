using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponProfile", menuName = "FlowSpace/Weapon Profile")]
public class WeaponProfile : ScriptableObject
{
	public float range = 50;
	public float damage = 1;
	public LayerMask hitMask;
}
