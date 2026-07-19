using UnityEngine;

[CreateAssetMenu(fileName = "NewRhythmActionDefinition", menuName = "FlowSpace/Rhythm Action Definition", order = 0)]
public class RhythmActionDefinition : ScriptableObject
{
	[Tooltip("Beat period and offset this action checks against")]
	[SerializeField] private BeatSignature signature = new BeatSignature(1, 0);

	[Tooltip("Timing window around this actions beats, in MS")]
	[SerializeField] private int windowInMS = 40;

	/// <summary>True if the current time falls within one of this action's beat windows</summary>
	public bool InWindow(float time) => Conductor.Instance.InWindow(signature, windowInMS, time);
}
