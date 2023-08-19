using UnityEngine;

public static class MethodExtentions
{
	public static Vector3 ToVector3XZ(this Vector2 v) => new Vector3(v.x, 0, v.y);
	public static void DestroyAllChildren(this Transform transform)
	{
		foreach (Transform child in transform)
		{
			GameObject.Destroy(child.gameObject);
		}
	}
	public static void DestroyAllChildren(this GameObject gameObject) => gameObject.transform.DestroyAllChildren();
}
