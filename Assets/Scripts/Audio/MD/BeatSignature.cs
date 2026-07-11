using System;
using System.Collections.Generic;
using System.Text;

public struct BeatSignature
{
	public float Period { get; private set; }
	public float Offset { get; private set; }

	public BeatSignature(float period, float offset) : this()
	{
		this.Period = period;
		this.Offset = offset;
	}
};
