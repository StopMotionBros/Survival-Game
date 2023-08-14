using UnityEngine;

public static class MethodExtentions
{
	public static Vector3 ToVector3XZ(this Vector2 v) => new Vector3(v.x, 0, v.y);
}
