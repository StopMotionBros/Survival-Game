using UnityEngine;

public class World : MonoBehaviour
{
	#region Getters
	public AnimationCurve TerrainFalloff => _terrainFalloff;

	public bool SmoothTerrain => _smoothTerrain;
	public bool ShadeSmooth => _shadeSmooth;
	#endregion

	[SerializeField] Material _terrainMaterial;

	[Space]

	[SerializeField] bool _smoothTerrain;
	[SerializeField] bool _shadeSmooth;

	[Space]

	[SerializeField] AnimationCurve _terrainFalloff;

	void Awake()
	{
		
	}
}
