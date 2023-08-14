using System;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

public class Grid2D<T>
{
	#region Getters
	public int Width => _width;
	public int Height => _height;
	public float CellWidth => _cellWidth;
	public float CellHeight => _cellHeight;

	public T[,] Cells => _cells;
	#endregion

	int _width, _height;
	float _cellWidth, _cellHeight;

	Vector2 _position;
	Vector2 _pivot;

	T[,] _cells;

	#region Constructors
	public Grid2D(Vector2 pos, int width, int height, float cellSize, Vector2 pivot, Func<T> createFunc) : this(pos, width, height, cellSize, cellSize, pivot, createFunc)
	{
	}

	public Grid2D(Vector2 pos, int width, int height, float cellWidth, float cellHeight, Vector2 pivot, Func<T> createFunc)
	{
		_position = pos;

		_width = width;
		_height = height;
		_cellWidth = cellWidth;
		_cellHeight = cellHeight;

		_pivot = new Vector2(_width * _cellWidth, _height * _cellHeight) * pivot;

		_cells = new T[width, height];
		if (createFunc == null) return;
		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				_cells[x, y] = createFunc.Invoke();
			}
		}
	}
	#endregion

	public T GetCellLocal(int x, int y)
	{
		x = math.clamp(x, 0, _width - 1);
		y = math.clamp(y, 0, _height - 1);
		
		return _cells[x, y];
	}

	public T GetCellGlobal(float x, float y)
	{
		ToLocal(x, y, out int localX, out int localY);

		return GetCellLocal(localX, localY);
	}

	public bool SetCellLocal(int x, int y, T cell)
	{
		if (!InsideGrid(x, y)) return false;

		_cells[x, y] = cell;
		return true;
	}

	public bool SetCellGlobal(float x, float y, T cell)
	{
		ToLocal(x, y, out int localX, out int localY);

		return SetCellLocal(localX, localY, cell);
	}

	public Vector2 GetCellPosLocal(int x, int y, bool clamp = false)
	{
		if (clamp)
		{
			x = math.clamp(x, 0, _width - 1);
			y = math.clamp(y, 0, _height - 1);
		}

		return _position + new Vector2(x * _cellWidth, y * _cellHeight) - _pivot;
	}

	public Vector2 GetCellPosGlobal(float x, float y, bool clamp = false)
	{
		ToLocal(x, y, out int localX, out int localY, clamp);

		return GetCellPosLocal(localX, localY);
	}
	public Vector2 GetCellPosGlobal(Vector2 position, bool clamp = false) => GetCellPosGlobal(position.x, position.y, clamp);

	public bool InsideGrid(int x, int y) => !(x < 0 || x >= _width || y < 0 || y >= _height);

	public void ToLocal(float x, float y, out int localX, out int localY, bool clamp = false)
	{
		Vector2 pivot = _position - _pivot;
		localX = (int)math.round((x - pivot.x) / _cellWidth);
		localY = (int)math.round((y - pivot.y) / _cellHeight);

		if(clamp)
		{
			localX = math.clamp(localX, 0, _width - 1);
			localY = math.clamp(localY, 0, _height - 1);
		}
	}

	public void ToLocal(Vector3 position, out int localX, out int localY) => 
		ToLocal(position.x, position.y, out localX, out localY);

	public void SetRange(int x, int y, int w, int h, T cell)
	{
		for (int tX = x; tX < x + w; tX++)
		{
			for (int tY = y; tY < y + h; tY++)
			{
				_cells[tX, tY] = cell;
			}
		}
	}

	public T GetRandomCell() => _cells[UnityEngine.Random.Range(0, Width), UnityEngine.Random.Range(0, Height)];
	public Vector2 GetRandomCellPos() => 
		GetCellPosLocal(UnityEngine.Random.Range(0, Width), UnityEngine.Random.Range(0, Height));

	public void DrawGrid(Color color, Color nullColor)
	{
		Gizmos.color = color;

		int i = 0;
		for (int y = 0; y < _height; y++)
		{
			for (int x = 0; x < _width; x++)
			{
				if (_cells[x, y] == null) Gizmos.color = nullColor;

				Vector3 currentPos = GetCellPosLocal(x, y);
				Vector3 left = new Vector3(_cellWidth, 0);
				Vector3 top = new Vector3(_cellWidth, _cellHeight);
				Vector3 right = new Vector3(0, _cellHeight);

				Gizmos.DrawLine(currentPos, currentPos + left);
				Gizmos.DrawLine(currentPos + left, currentPos + top);
				Gizmos.DrawLine(currentPos + top, currentPos + right);
				Gizmos.DrawLine(currentPos + right, currentPos);

				DrawText(currentPos + (0.5f * top), i.ToString());

				Gizmos.color = color;

				i++;
			}
		}
	}

	static void DrawText(Vector3 position, string text)
	{
		GUI.color = Gizmos.color;

		GUIStyle style = new GUIStyle(GUI.skin.label);
		style.alignment = TextAnchor.MiddleCenter;

		Handles.Label(position, text, style);
	}
}