using JetBrains.Annotations;
using System;
using UnityEngine;
using static UnityEditor.PlayerSettings;

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

		[SerializeField] private Camera cam;
		[SerializeField] private BoxCollider boundsBox;
		[SerializeField] private Transform player;
		[SerializeField] private RectTransform reticle;
		[SerializeField] private RectTransform canvas;

		private Vector2 planeVelocity;

		public void Tick(float dt, PlayerIntent intent, Animator animator)
		{
			// ship movement
			Vector2 moveInput = intent.Move;

			if (!isDashing && intent.Dash)
			{
				isDashing = true;
				StartDash(moveInput);
			}

			planeVelocity = ResolveMovement(dt, moveInput);
			ResolveRotation(moveInput);

			UpdateAnimator(animator, intent);
			player.localPosition += new Vector3(planeVelocity.x, planeVelocity.y, 0) * dt;

			ConstrainToViewport();

			// reticle movement
			Vector2 pos = reticle.anchoredPosition + intent.Look;

			Vector2 max = canvas.rect.size - reticle.rect.size / 2;
			Vector2 min = reticle.rect.size / 2;
			pos.x = Mathf.Clamp(pos.x, min.x, max.x);
			pos.y = Mathf.Clamp(pos.y, min.y, max.y);

			reticle.anchoredPosition = pos;
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

		private void ResolveRotation(Vector3 moveDir)
		{
			// should handle banking here
		}

		private void UpdateAnimator(Animator animator, PlayerIntent intent)
		{
			animator.SetFloat("Speed", planeVelocity.magnitude);
		}
	}
}
