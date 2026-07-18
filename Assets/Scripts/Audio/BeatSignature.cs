using System;
using UnityEngine;

[Serializable]
public struct BeatSignature
{
	[SerializeField] private float period;
	[SerializeField] private float offset;

	public float Period => period;
	public float Offset => offset;

	public BeatSignature(float period = 1, float offset = 0)
	{
		this.period = period;
		this.offset = offset;
	}
};
