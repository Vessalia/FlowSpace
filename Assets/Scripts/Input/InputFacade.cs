using System;
using UnityEngine;

public class CharacterInputFacade : ICharacterInputReader
{
	private ICharacterInputReader _reader;

	public event Action<Vector2> MoveEvent;
	public event Action<Vector2> LookEvent;
	public event Action AttackStartedEvent;
	public event Action AttackCancelledEvent;
	public event Action DashStartedEvent;
	public event Action DashCancelledEvent;
	public event Action NextEvent;
	public event Action PreviousEvent;
	public event Action PauseEvent;

	public CharacterInputFacade(ICharacterInputReader reader)
	{
		_reader = reader;
		SubscribeAll(_reader);
	}

	public void Set(ICharacterInputReader reader)
	{
		UnsubscribeAll(_reader);
		_reader = reader;
		SubscribeAll(_reader);
	}

	private void SubscribeAll(ICharacterInputReader reader)
	{
		reader.MoveEvent += OnMove;
		reader.LookEvent += OnLook;
		reader.AttackStartedEvent += OnAttackStarted;
		reader.AttackCancelledEvent += OnAttackCancelled;
		reader.DashStartedEvent += OnDashStarted;
		reader.DashCancelledEvent += OnDashCancelled;
		reader.NextEvent += OnNext;
		reader.PreviousEvent += OnPrevious;
		reader.PauseEvent += OnPause;
	}

	private void UnsubscribeAll(ICharacterInputReader reader)
	{
		reader.MoveEvent -= OnMove;
		reader.LookEvent -= OnLook;
		reader.AttackStartedEvent -= OnAttackStarted;
		reader.AttackCancelledEvent -= OnAttackCancelled;
		reader.DashStartedEvent -= OnDashStarted;
		reader.DashCancelledEvent -= OnDashCancelled;
		reader.NextEvent -= OnNext;
		reader.PreviousEvent -= OnPrevious;
		reader.PauseEvent -= OnPause;
	}

	private void OnMove(Vector2 v) => MoveEvent?.Invoke(v);
	private void OnLook(Vector2 v) => LookEvent?.Invoke(v);
	private void OnAttackStarted() => AttackStartedEvent?.Invoke();
	private void OnAttackCancelled() => AttackCancelledEvent?.Invoke();
	private void OnDashStarted() => DashStartedEvent?.Invoke();
	private void OnDashCancelled() => DashCancelledEvent?.Invoke();
	private void OnNext() => NextEvent?.Invoke();
	private void OnPrevious() => PreviousEvent?.Invoke();
	private void OnPause() => PauseEvent?.Invoke();

	public void Enable() => _reader.Enable();
	public void Disable() => _reader.Disable();
}

public class UIInputFacade : IUIInputReader
{
	private IUIInputReader _reader;

	public event Action<Vector2> NavigateEvent;
	public event Action SubmitEvent;
	public event Action CancelEvent;

	public UIInputFacade(IUIInputReader reader)
	{
		_reader = reader;
		SubscribeAll(_reader);
	}

	public void Set(IUIInputReader reader)
	{
		UnsubscribeAll(_reader);
		_reader = reader;
		SubscribeAll(_reader);
	}

	private void SubscribeAll(IUIInputReader reader)
	{
		reader.NavigateEvent += OnNavigate;
		reader.SubmitEvent += OnSubmit;
		reader.CancelEvent += OnCancel;
	}

	private void UnsubscribeAll(IUIInputReader reader)
	{
		reader.NavigateEvent -= OnNavigate;
		reader.SubmitEvent -= OnSubmit;
		reader.CancelEvent -= OnCancel;
	}

	private void OnNavigate(Vector2 v) => NavigateEvent?.Invoke(v);
	private void OnSubmit() => SubmitEvent?.Invoke();
	private void OnCancel() => CancelEvent?.Invoke();

	public void Enable() => _reader.Enable();
	public void Disable() => _reader.Disable();
}