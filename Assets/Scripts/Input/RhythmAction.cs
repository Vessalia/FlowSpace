using System;

public class RhythmAction
{
	private readonly RhythmActionDefinition definition;

	public event Action OnTriggered;

	public RhythmAction(RhythmActionDefinition definition) => this.definition = definition;

	public void TryInvoke(float time)
	{
		if (!InWindow(time)) return;
		OnTriggered?.Invoke();
	}

	public bool InWindow(float time)
	{
		return definition.InWindow(time);
	}
}
