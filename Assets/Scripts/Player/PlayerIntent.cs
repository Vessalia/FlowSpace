using System;
using UnityEngine;

namespace Assets.Scripts.Player
{
	public class PlayerIntent : IDisposable
	{
		public Vector2 Move { get; private set; }
		public Vector2 Look { get; private set; }

		public bool Dash { get; private set; }

		public PlayerIntent()
		{
			var _input = InputManager.Instance.GetPlayerInput();
			_input.MoveEvent += OnMove;
			_input.LookEvent += OnLook;
			_input.DashEvent += OnDash;
		}

		~PlayerIntent() => Dispose();

		public void Dispose()
		{
			var _input = InputManager.Instance.GetPlayerInput();
			_input.MoveEvent -= OnMove;
			_input.LookEvent -= OnLook;
			_input.DashEvent -= OnDash;
		}

		public void LateTick()
		{
			Dash = false;
		}

		private void OnMove(Vector2 move) => Move = move;
		private void OnLook(Vector2 look) => Look = look;

		private void OnDash() => Dash = true;
	}
}
