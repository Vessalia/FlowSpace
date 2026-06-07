using Mono.Cecil.Cil;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

namespace Assets.Scripts.Player
{
	public class PlayerController : MonoBehaviour
	{
		[SerializeField] private Animator animator;
		[SerializeField] private CharacterController cc;

		private PlayerIntent intent;
		[SerializeField] public PlayerMotor motor;
		[SerializeField] public Transform trackPivot;

		void Awake()
		{
			intent = new();
		}

		void Update()
		{
			motor.Tick(Time.deltaTime, intent, cc, animator, trackPivot.position, trackPivot.forward, trackPivot.up);
		}

		void OnDestroy()
		{
			intent.Dispose();
		}
	}
}
