using Mono.Cecil.Cil;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

namespace Assets.Scripts.Player
{
	public class PlayerController : MonoBehaviour
	{
		[SerializeField] private Animator animator;

		private PlayerIntent intent;
		[SerializeField] public PlayerMotor motor;

		void Awake()
		{
			intent = new();
			motor.Init();
		}

		void Update()
		{
			motor.Tick(Time.deltaTime, intent, transform, animator);
		}

		private void LateUpdate()
		{
			intent.LateTick();
			motor.LateTick(transform);
		}

		void OnDestroy()
		{
			intent.Dispose();
		}
	}
}
