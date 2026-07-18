using UnityEngine;

public class RhythmReticle : BeatListener
{
	[Min(1f), SerializeField] float growthFactor = 1.2f;
	[Min(0f), SerializeField] float decay = 10;

	private RectTransform reticle;
	private Vector3 baseScale;
	private Vector3 grownScale;

	public override void Start()
	{
		base.Start();
		reticle = GetComponent<RectTransform>();
		baseScale = reticle.localScale;
		grownScale = baseScale * growthFactor;
	}

	public override void BeatAction(float delay)
	{
		reticle.localScale = MathUtils.ExpDecay(grownScale, baseScale, decay, delay);
	}

	public void Update()
	{
		reticle.localScale = MathUtils.ExpDecay(reticle.localScale, baseScale, decay, Clock.Instance.MusicDeltaTime);
	}
}
