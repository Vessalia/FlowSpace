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
		}

		void Update()
		{
			motor.Tick(Time.deltaTime, intent, animator);
		}

		private void LateUpdate()
		{
			intent.LateTick();
		}

		void OnDestroy()
		{
			intent.Dispose();
		}
	}
}
