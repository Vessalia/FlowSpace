using System;
using UnityEngine;

public interface ICharacterInputReader
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
	
	public void Enable();
	public void Disable();
}

public interface IUIInputReader
{
	public event Action<Vector2> NavigateEvent;
	public event Action SubmitEvent;
	public event Action CancelEvent;

	public void Enable();
	public void Disable();
}
