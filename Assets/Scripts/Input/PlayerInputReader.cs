using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputReader : ICharacterInputReader, GameInputActions.IPlayerActions
{
	public event Action<Vector2> MoveEvent;
	public event Action<Vector2> LookEvent;

	public event Action AttackStartedEvent;
	public event Action AttackCancelledEvent;
	public event Action DashStartedEvent;
	public event Action DashCancelledEvent;
	public event Action NextEvent;
	public event Action PreviousEvent;
	public event Action PauseEvent;

	private Action OnEnable;
	private Action OnDisable;

	public PlayerInputReader(GameInputActions actions)
	{
		OnEnable = () => actions.Player.Enable();
		OnDisable = () => actions.Player.Disable();

		actions.Player.SetCallbacks(this);
	}

	// ── IPlayerActions callbacks ───────────────────────────────────────────────

	public void OnMove(InputAction.CallbackContext ctx)
	{
		MoveEvent?.Invoke(ctx.ReadValue<Vector2>());
	}

	public void OnLook(InputAction.CallbackContext ctx)
	{
		LookEvent?.Invoke(ctx.ReadValue<Vector2>());
	}

	public void OnNext(InputAction.CallbackContext ctx)
	{
		if (ctx.started) NextEvent?.Invoke();
	}

	public void OnPrevious(InputAction.CallbackContext ctx)
	{
		if (ctx.started) PreviousEvent?.Invoke();
	}

	public void OnMenu(InputAction.CallbackContext ctx)
	{
		if (ctx.started) PauseEvent?.Invoke();
	}

	public void OnAttack(InputAction.CallbackContext ctx)
	{
		if (ctx.started) AttackStartedEvent?.Invoke();
		if (ctx.canceled) AttackCancelledEvent?.Invoke();
	}

	public void OnDash(InputAction.CallbackContext ctx)
	{
		if (ctx.started) { DashStartedEvent?.Invoke(); }
		if (ctx.canceled) { DashCancelledEvent?.Invoke(); }
	}

	public void Enable() => OnEnable?.Invoke();

	public void Disable() => OnDisable?.Invoke();
}

