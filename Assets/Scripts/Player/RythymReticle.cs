using UnityEngine;

public class RythymReticle : BeatListener
{
	[Min(1f)]
	[SerializeField] float growthFactor = 1.2f;

	private bool grow = true;
	private RectTransform reticle;

	public override void Start()
	{
		base.Start();
		reticle = GetComponent<RectTransform>();
	}

	public override void BeatAction(float delay)
	{
		if (grow)
		{
			reticle.localScale *= growthFactor;
			grow = false;
		}
		else
		{
			reticle.localScale /= growthFactor;
			grow = true;
		}
	}
}
