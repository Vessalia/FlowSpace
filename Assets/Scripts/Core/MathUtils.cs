using UnityEngine;

public static class MathUtils
{
	public static Vector2 Lerp(Vector2 a, Vector2 b, float t)
	{
		float x = Mathf.Lerp(a.x, b.x, t);
		float y = Mathf.Lerp(a.y, b.y, t);

		return new Vector2(x, y);
	}

	public static Vector3 Lerp(Vector3 a, Vector3 b, float t)
	{
		float x = Mathf.Lerp(a.x, b.x, t);
		float y = Mathf.Lerp(a.y, b.y, t);
		float z = Mathf.Lerp(a.z, b.z, t);

		return new Vector3(x, y, z);
	}

	// Freya Holmer: https://www.youtube.com/watch?v=LSNQuFEDOyQ at the 50min mark
	public static float ExpDecay(float a, float b, float decay, float dt)
	{
		return b + (a - b) * Mathf.Exp(-decay * dt);
	}

	public static Vector2 ExpDecay(Vector2 a, Vector2 b, float decay, float dt)
	{
		float x = ExpDecay(a.x, b.x, decay, dt);
		float y = ExpDecay(a.y, b.y, decay, dt);

		return new Vector2(x, y);
	}

	public static Vector3 ExpDecay(Vector3 a, Vector3 b, float decay, float dt)
	{
		float x = ExpDecay(a.x, b.x, decay, dt);
		float y = ExpDecay(a.y, b.y, decay, dt);
		float z = ExpDecay(a.z, b.z, decay, dt);

		return new Vector3(x, y, z);
	}

	public static float ExpDecayAngle(float a, float b, float decay, float dt)
	{
		float delta = Mathf.DeltaAngle(a, b);
		float factor = 1f - Mathf.Exp(-decay * dt); // Compute decay factor correctly
		return a + delta * factor;
	}

	public static float Dist2(Vector2 a, Vector2 b)
	{
		float x = (a.x - b.x);
		float y = (a.y - b.y);
		return x * x + y * y;
	}

	public static float Dist2(Vector3 a, Vector3 b)
	{
		float x = (a.x - b.x);
		float y = (a.y - b.y);
		float z = (a.z - b.z);
		return x * x + y * y + z * z;
	}

	public static bool ApproxZero(float value, float epsilon = 0.00001f)
	{
		return Mathf.Abs(value) < epsilon;
	}
}

// swizzles
public static class VectorExtensions
{
	public static Vector2 xx(this Vector3 v) => new Vector2(v.x, v.x);
	public static Vector2 xy(this Vector3 v) => new Vector2(v.x, v.y);
	public static Vector2 xz(this Vector3 v) => new Vector2(v.x, v.z);

	public static Vector2 yx(this Vector3 v) => new Vector2(v.y, v.x);
	public static Vector2 yy(this Vector3 v) => new Vector2(v.y, v.y);
	public static Vector2 yz(this Vector3 v) => new Vector2(v.y, v.z);

	public static Vector2 zx(this Vector3 v) => new Vector2(v.z, v.x);
	public static Vector2 zy(this Vector3 v) => new Vector2(v.z, v.y);
	public static Vector2 zz(this Vector3 v) => new Vector2(v.z, v.z);

	public static Vector3 xxx(this Vector3 v) => new Vector3(v.x, v.x, v.x);
	public static Vector3 xxy(this Vector3 v) => new Vector3(v.x, v.x, v.y);
	public static Vector3 xxz(this Vector3 v) => new Vector3(v.x, v.x, v.z);
	public static Vector3 xyx(this Vector3 v) => new Vector3(v.x, v.y, v.x);
	public static Vector3 xyy(this Vector3 v) => new Vector3(v.x, v.y, v.y);
	public static Vector3 xyz(this Vector3 v) => new Vector3(v.x, v.y, v.z);
	public static Vector3 xzx(this Vector3 v) => new Vector3(v.x, v.z, v.x);
	public static Vector3 xzy(this Vector3 v) => new Vector3(v.x, v.z, v.y);
	public static Vector3 xzz(this Vector3 v) => new Vector3(v.x, v.z, v.z);

	public static Vector3 yxx(this Vector3 v) => new Vector3(v.y, v.x, v.x);
	public static Vector3 yxy(this Vector3 v) => new Vector3(v.y, v.x, v.y);
	public static Vector3 yxz(this Vector3 v) => new Vector3(v.y, v.x, v.z);
	public static Vector3 yyx(this Vector3 v) => new Vector3(v.y, v.y, v.x);
	public static Vector3 yyy(this Vector3 v) => new Vector3(v.y, v.y, v.y);
	public static Vector3 yyz(this Vector3 v) => new Vector3(v.y, v.y, v.z);
	public static Vector3 yzx(this Vector3 v) => new Vector3(v.y, v.z, v.x);
	public static Vector3 yzy(this Vector3 v) => new Vector3(v.y, v.z, v.y);
	public static Vector3 yzz(this Vector3 v) => new Vector3(v.y, v.z, v.z);

	public static Vector3 zxx(this Vector3 v) => new Vector3(v.z, v.x, v.x);
	public static Vector3 zxy(this Vector3 v) => new Vector3(v.z, v.x, v.y);
	public static Vector3 zxz(this Vector3 v) => new Vector3(v.z, v.x, v.z);
	public static Vector3 zyx(this Vector3 v) => new Vector3(v.z, v.y, v.x);
	public static Vector3 zyy(this Vector3 v) => new Vector3(v.z, v.y, v.y);
	public static Vector3 zyz(this Vector3 v) => new Vector3(v.z, v.y, v.z);
	public static Vector3 zzx(this Vector3 v) => new Vector3(v.z, v.z, v.x);
	public static Vector3 zzy(this Vector3 v) => new Vector3(v.z, v.z, v.y);
	public static Vector3 zzz(this Vector3 v) => new Vector3(v.z, v.z, v.z);
}
