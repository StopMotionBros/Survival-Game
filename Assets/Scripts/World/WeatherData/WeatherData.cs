using UnityEngine;

[CreateAssetMenu(fileName = "WeatherData", menuName = "Data/Weather")]
public class WeatherData : ScriptableObject
{
	public int Chance => _chance;

	public float CloudDensity => _cloudDensity;
	public float CloudFalloff => _cloudFalloff;
	public float CloudSpeed => _cloudSpeed;
	public Color CloudColor => _cloudColor;

	public float SkyIntensity => _skyIntensity;

	public float SunIntensity => _sunIntensity;

	[Range(0, 100)]
	[SerializeField] int _chance;

	[Range(0, 1)]
    [SerializeField] float _cloudDensity;
	[Range(0, 1)]
    [SerializeField] float _cloudFalloff;
    [SerializeField] float _cloudSpeed;
	[SerializeField] Color _cloudColor;

	[Range(0, 1)]
	[SerializeField] float _skyIntensity;

	[SerializeField] float _sunIntensity;
}
