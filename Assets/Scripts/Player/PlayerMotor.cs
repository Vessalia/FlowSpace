using JetBrains.Annotations;
using System;
using UnityEngine;

namespace Assets.Scripts.Player
{
	[Serializable]
	public class PlayerMotor
	{
		[Header("Dash Parameters")]

		[Range(1f, 100f)]
		public float dashDistance = 10f; // distance traveled until end of dash
		public float dashDuration = 0.2f;

		public float InitialDashVelocity => (2.0f * dashDistance) / dashDuration; // constant decceleration

		private bool isDashing = false;
		private float dashTimer = 0;
		private Vector2 dashDir = Vector2.zero;

		[Header("Movement")]
		public float FlightSpeed = 6f;
		public float ReticleSpeed = 10f;

		[Range(0.01f, 1f)]
		public float timeToMaxSpeed = 0.1f; // how long does it take to reach max speed
		public float Acceleration => FlightSpeed / timeToMaxSpeed;

		private Vector2 planeVelocity;

		public void Tick(float dt, PlayerIntent intent, Transform transform, Animator animator)
		{
			Vector2 moveInput = intent.Move;

			if (!isDashing && intent.Dash)
			{
				isDashing = true;
				StartDash(moveInput);
			}

			planeVelocity = ResolveMovement(dt, moveInput);
			ResolveRotation(moveInput, transform);

			UpdateAnimator(animator, intent);
			transform.localPosition += new Vector3(planeVelocity.x, planeVelocity.y, 0) * dt;
		}

		private void StartDash(Vector2 dir)
		{
			if (MathUtils.ApproxZero(dir.sqrMagnitude)) return;

			dashTimer = dashDuration;
			dashDir = dir.normalized;
			planeVelocity = dashDir * InitialDashVelocity;
		}

		private Vector2 ResolveMovement(float dt, Vector2 moveInput)
		{
			if (isDashing)
			{
				return ResolveDash(dt);
			}
			else
			{
				return ResolveFlight(moveInput);
			}
		}

		private Vector2 ResolveDash(float dt)
		{
			dashTimer -= dt;

			if (dashTimer <= 0)
			{
				isDashing = false;
				return Vector2.zero;
			}
			else
			{
				float t = dashTimer / dashDuration;
				return dashDir * (InitialDashVelocity * t);
			}
		}

		private Vector2 ResolveFlight(Vector2 moveInput)
		{
			Vector2 velocity = Vector2.zero;
			if (moveInput.sqrMagnitude > 0f)
			{
				velocity = Vector2.MoveTowards(planeVelocity, moveInput.normalized * FlightSpeed, Acceleration * Time.deltaTime);
			}
			else
			{
				float accelerationScale = 1f / 4f;
				float magnitude = Mathf.MoveTowards(planeVelocity.magnitude, 0f, Acceleration * accelerationScale * Time.deltaTime);
				velocity = planeVelocity.normalized * magnitude;
			}

			return velocity;
		}

		private void ResolveRotation(Vector3 moveDir, Transform transform)
		{
			// should handle banking here
		}

		private void UpdateAnimator(Animator animator, PlayerIntent intent)
		{
			animator.SetFloat("Speed", planeVelocity.magnitude);
		}
	}
}
