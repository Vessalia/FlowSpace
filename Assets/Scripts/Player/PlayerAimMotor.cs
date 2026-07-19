using System;
using UnityEngine;

namespace Assets.Scripts.Player
{
	[Serializable]
	public class PlayerAimMotor
	{
		public float ReticleSpeed = 10f;

		[SerializeField] private RectTransform reticle;
		[SerializeField] private RectTransform reticleBounds;
		[SerializeField] private RectTransform canvas;

		public void Tick(float dt, PlayerIntent intent)
		{
			Vector2 pos = reticle.anchoredPosition + intent.Look.value;

			Vector2 max = canvas.rect.size - reticleBounds.rect.size / 2;
			Vector2 min = reticleBounds.rect.size / 2;
			pos.x = Mathf.Clamp(pos.x, min.x, max.x);
			pos.y = Mathf.Clamp(pos.y, min.y, max.y);

			reticle.anchoredPosition = pos;
		}
	}
}
