using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
	World _world;

	GameObject _chunkObject;
	MeshFilter _meshFilter;
	MeshCollider _meshCollider;

	Vector3Int _position;

	List<Vector3> _vertices = new();
	List<int> _triangles = new();

	List<Vector3> _colliderVertices = new();
	List<int> _colliderTriangles = new();

	float[,,] _terrainMap;

	public Chunk(World world, Vector3Int position, Material material)
	{
		_world = world;

		_chunkObject = new GameObject();
		_position = position;
		
		_chunkObject.transform.position = _position;
		_meshFilter = _chunkObject.AddComponent<MeshFilter>();
		_meshCollider = _chunkObject.AddComponent<MeshCollider>();
		_chunkObject.AddComponent<MeshRenderer>().material = material;

		_terrainMap = new float[WorldData.CHUNK_SIZE + 1, WorldData.CHUNK_SIZE + 1, WorldData.CHUNK_SIZE + 1];

		PopulateMap();
		GenerateMeshData();
	}

	void PopulateMap()
	{
		for (int x = 0; x <= WorldData.CHUNK_SIZE; x++)
		{
			for (int y = 0; y <= WorldData.CHUNK_SIZE; y++)
			{
				for (int z = 0; z <= WorldData.CHUNK_SIZE; z++)
				{
					float height = y;
					if (y >= WorldData.SURFACE_HEIGHT)
						height = WorldData.SURFACE_HEIGHT + (Mathf.PerlinNoise((_position.x + x) * 0.1f, (_position.z + z) * 0.1f) * WorldData.SURFACE_SCALE);

					float density = WorldData.PerlinNoise3D(
						(_position.x + x) * 0.1f, 
						(_position.y + y) * 0.1f, 
						(_position.z + z) * 0.1f) * 
						_world.TerrainFalloff.Evaluate(((float)_position.y + y) / (WorldData.SURFACE_HEIGHT * 7));

					_terrainMap[x, y, z] = Mathf.Max(density, _position.y + y - height);
				}
			}
		}
	}

	void GenerateMeshData()
	{
		ClearMesh();

		for (int x = 0; x < WorldData.CHUNK_SIZE; x++)
			for (int y = 0; y < WorldData.CHUNK_SIZE; y++)
				for (int z = 0; z < WorldData.CHUNK_SIZE; z++)
					MarchCube(new Vector3Int(x, y, z));

		UpdateMesh();
	}

	void MarchCube(Vector3Int position)
	{
		float[] cube = new float[8];
		for (int i = 0; i < 8; i++)
		{
			cube[i] = SamplePoint(position + WorldData.VertexTable[i]);
		}

		int configIndex = GetCubeConfig(cube);

		if (configIndex <= 0 || configIndex >= 255) return;

		int edgeIndex = 0;
		for (int i = 0; i < 5; i++)
		{
			for (int k = 0; k < 3; k++)
			{
				int index = WorldData.TriangleTable[configIndex, edgeIndex];

				if (index == -1) return;

				Vector3 vertex1 = position + WorldData.VertexTable[WorldData.EdgeIndexes[index, 0]];
				Vector3 vertex2 = position + WorldData.VertexTable[WorldData.EdgeIndexes[index, 1]];

				Vector3 vertexPosition;

				if (_world.SmoothTerrain)
				{
					float vertex1Sample = cube[WorldData.EdgeIndexes[index, 0]];
					float vertex2Sample = cube[WorldData.EdgeIndexes[index, 1]];

					float difference = vertex2Sample - vertex1Sample;

					if (difference == 0) difference = WorldData.TERRAIN_SURFACE;
					else difference = (WorldData.TERRAIN_SURFACE - vertex1Sample) / difference;

					vertexPosition = vertex1 + (difference * (vertex2 - vertex1));
				}
				else
				{
					vertexPosition = 0.5f * (vertex1 + vertex2);
				}

				if (_world.ShadeSmooth)
				{
					_triangles.Add(VertexForIndex(vertexPosition));
				}
				else
				{
					_vertices.Add(vertexPosition);
					_triangles.Add(_vertices.Count - 1);
				}

				_colliderVertices.Add(0.5f * (vertex1 + vertex2));
				_colliderTriangles.Add(_colliderVertices.Count - 1);

				edgeIndex++;
			}
		}
	}

	int GetCubeConfig(float[] cube)
	{
		int configIndex = 0;

		for (int i = 0; i < 8; i++)
		{
			if (cube[i] > WorldData.TERRAIN_SURFACE) configIndex |= 1 << i;
		}

		return configIndex;
	}

	void UpdateMesh()
	{
		Mesh mesh = new Mesh();
		mesh.vertices = _vertices.ToArray();
		mesh.triangles = _triangles.ToArray();
		mesh.RecalculateNormals();

		_meshFilter.mesh = mesh;

		mesh = new Mesh();
		mesh.vertices = _colliderVertices.ToArray();
		mesh.triangles = _colliderTriangles.ToArray();
		mesh.RecalculateNormals();

		_meshCollider.sharedMesh = mesh;
	}

	void ClearMesh()
	{
		_vertices.Clear();
		_triangles.Clear();

		_colliderVertices.Clear();
		_colliderTriangles.Clear();

		_meshFilter.mesh = null;
		_meshCollider.sharedMesh = null;
	}

	float SamplePoint(Vector3Int point) => _terrainMap[point.x, point.y, point.z];
	int VertexForIndex(Vector3 vertex)
	{
		for (int i = 0; i < _vertices.Count; i++)
		{
			if (_vertices[i] == vertex) return i;
		}

		_vertices.Add(vertex);
		return _vertices.Count - 1;
	}
}