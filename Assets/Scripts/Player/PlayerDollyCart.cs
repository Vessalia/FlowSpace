using Unity.Cinemachine;
using UnityEngine;

public class PlayerDollyCart : MonoBehaviour
{
	[SerializeField] private CinemachineSplineCart cart;
	[SerializeField] private float speed = 1;

	void Update()
	{
		cart.SplinePosition += speed * Clock.Instance.MusicDeltaTime;
	}
}
