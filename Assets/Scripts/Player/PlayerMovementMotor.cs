using System;
using UnityEngine;

namespace Assets.Scripts.Player
{
	[Serializable]
	public class PlayerMovementMotor
	{
		[Header("Dash Parameters")]

		[SerializeField] private RhythmActionDefinition dash;
		[Min(0f)] public float dashDistance = 10f; // distance traveled until end of dash
		[Min(0f)] public float dashDuration = 0.2f;

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

		[SerializeField] private Camera cam;
		[SerializeField] private BoxCollider boundsBox;
		[SerializeField] private Transform player;

		private Vector2 planeVelocity;

		public void Tick(float dt, PlayerIntent intent)
		{
			// ship movement
			Vector2 moveInput = intent.Move;
			var dashIntent = intent.Dash;

			if (!isDashing && dashIntent.value)
			{
				if (dash.InWindow(dashIntent.time))
					StartDash(moveInput);
			}

			planeVelocity = ResolveMovement(dt, moveInput);
			ResolveRotation(moveInput);

			player.localPosition += new Vector3(planeVelocity.x, planeVelocity.y, 0) * dt;

			ConstrainToViewport();
		}

		private void ConstrainToViewport()
		{
			Vector3 backFaceCenter = player.position - cam.transform.forward * boundsBox.size.z / 2;

			Vector3 viewportPos = cam.WorldToViewportPoint(backFaceCenter);

			float bufferX = Mathf.Abs(cam.WorldToViewportPoint(backFaceCenter + cam.transform.right * boundsBox.size.x / 2).x - viewportPos.x);
			float bufferY = Mathf.Abs(cam.WorldToViewportPoint(backFaceCenter + cam.transform.up * boundsBox.size.y / 2).y - viewportPos.y);

			viewportPos.x = Mathf.Clamp(viewportPos.x, bufferX, 1 - bufferX);
			viewportPos.y = Mathf.Clamp(viewportPos.y, bufferY, 1 - bufferY);

			Vector3 clampedBack = cam.ViewportToWorldPoint(viewportPos);
			player.position = clampedBack + cam.transform.forward * boundsBox.size.z / 2;
		}

		public void StartDash(Vector2 dir)
		{
			isDashing = true;
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

		private void ResolveRotation(Vector3 moveDir)
		{
			// should handle banking here
		}
	}
}
