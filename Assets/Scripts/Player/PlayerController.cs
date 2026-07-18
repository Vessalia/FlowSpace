using Mono.Cecil.Cil;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

namespace Assets.Scripts.Player
{
	public class PlayerController : MonoBehaviour
	{
		private PlayerIntent intent;
		[SerializeField] public PlayerMovementMotor moveMotor;
		[SerializeField] public PlayerAimMotor      aimMotor;
		[SerializeField] public PlayerAttackMotor   attackMotor;

		void Awake()
		{
			intent = new();
		}

		void Update()
		{
			moveMotor.Tick(Clock.Instance.GameDeltaTime, intent);
			aimMotor.Tick(Clock.Instance.GameDeltaTime, intent);
			attackMotor.Tick(Clock.Instance.GameDeltaTime, intent);
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
