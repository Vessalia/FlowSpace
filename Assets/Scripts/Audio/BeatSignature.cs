using System;
using System.Collections.Generic;
using System.Text;

[Serializable]
public struct BeatSignature
{
	public readonly float period;
	public readonly float offset;

	public BeatSignature(float period = 1, float offset = 0)
	{
		this.period = period;
		this.offset = offset;
	}
};
