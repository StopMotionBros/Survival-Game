using UnityEngine;

public static class DirectionUtil
{
	public static Vector3 GetDirection3D(Direction3D direction)
	{
		switch(direction)
		{
			case Direction3D.North: return Vector3.forward;
			case Direction3D.South: return Vector3.back;
			case Direction3D.West: return Vector3.right;
			case Direction3D.East: return Vector3.left;
			case Direction3D.Up: return Vector3.up;
			case Direction3D.Down: return Vector3.down;
		}

		return Vector3.zero;
	}

	public static Vector2 GetDirection2D(Direction2D direction)
	{
		switch(direction)
		{
			case Direction2D.North: return Vector2.up;
			case Direction2D.South: return Vector2.down;
			case Direction2D.West: return Vector2.right;
			case Direction2D.East: return Vector2.left;
		}

		return Vector2.zero;
	}

	public static float GetDirection1D(Direction1D direction)
	{
		switch(direction)
		{
			case Direction1D.North: return 1;
			case Direction1D.South: return -1;
		}

		return 0;
	}
}
public enum Direction3D
{
	North,
	West,
	South,
	East,
	Up,
	Down,
}
public enum Direction2D
{
	North,
	West,
	South,
	East,
}
public enum Direction1D
{
	North,
	South,
}