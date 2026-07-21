using System;
using UnityEngine;

namespace Assets.Scripts.Player
{
	public class PlayerIntent : IDisposable
	{
		public class TimedAction<T>
		{
			public T value;
			public float time;

			public void Set(T value)
			{
				this.value = value;
				time = Clock.Instance.LiveMusicTime;
			}
		};

		public Vector2 Move { get; private set; }
		public Vector2 Look { get; private set; }
		public bool Next { get; private set;  }
		public bool Previous { get; private set;  }

		public TimedAction<bool> Dash { get; private set; } = new();
		public TimedAction<bool> AttackHeld { get; private set; } = new();
		public TimedAction<bool> AttackJustPressed { get; private set; } = new();
		public TimedAction<bool> AttackJustReleased { get; private set; } = new();

		public PlayerIntent()
		{
			var _input = InputManager.Instance.GetPlayerInput();

			_input.MoveEvent += OnMove;
			_input.LookEvent += OnLook;
			_input.NextEvent += OnNext;
			_input.PreviousEvent += OnPrevious;

			_input.DashEvent += OnDash;
			_input.AttackStartedEvent += OnAttackStarted;
			_input.AttackCancelledEvent += OnAttackCancelled;
		}

		~PlayerIntent() => Dispose();

		public void Dispose()
		{
			var _input = InputManager.Instance.GetPlayerInput();

			_input.MoveEvent -= OnMove;
			_input.LookEvent -= OnLook;
			_input.NextEvent -= OnNext;
			_input.PreviousEvent -= OnPrevious;

			_input.DashEvent -= OnDash;
			_input.AttackStartedEvent -= OnAttackStarted;
			_input.AttackCancelledEvent -= OnAttackCancelled;
		}

		public void LateTick()
		{
			Next = false;
			Previous = false;

			Dash.Set(false);
			AttackJustPressed.Set(false);
			AttackJustReleased.Set(false);
		}

		private void OnMove(Vector2 move) => Move = move;
		private void OnLook(Vector2 look) => Look = look;
		private void OnNext() => Next = true;
		private void OnPrevious() => Previous = true;

		private void OnDash() => Dash.Set(true);

		private void OnAttackStarted()
		{
			AttackHeld.Set(true);
			AttackJustPressed.Set(true);
		}
		private void OnAttackCancelled()
		{
			AttackHeld.Set(false);
			AttackJustReleased.Set(true);
		}
	}
}
