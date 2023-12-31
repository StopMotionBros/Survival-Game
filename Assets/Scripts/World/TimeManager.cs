using Unity.Mathematics;
using DG.Tweening;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
	public static TimeManager Instance;
	public static event System.Action<int> OnAdvanceDay;

	[SerializeField] bool _update;

	[SerializeField] LightSettings _lightSettings;

    [SerializeField] Light _sun;
	[SerializeField] Transform _moon;
	[SerializeField] float _minutesPerDay;

	[SerializeField] float _startTime;

	int _currentDay;
	float _time;
	[SerializeField] float _sunAngle;
	float _moonAngle;

	float _normalizedMinutesPerDay;

	public bool IsDay => _time <= 0.5f;
	public bool IsNight => !IsDay;

	[SerializeField] Material _skybox;

	void Awake()
	{
		Instance = this;

		_time = _startTime;

		_moonAngle = 30;
		_normalizedMinutesPerDay = 1 / (_minutesPerDay * 60);
		_skybox.SetFloat("_Latitude", - _sunAngle);
	}

	void Update()
	{
		_skybox.SetFloat("_NormalizedTime", _time);
		_skybox.SetVector("_MoonDirection", -_moon.forward);
		_skybox.SetMatrix("_MoonSpaceMatrix", new Matrix4x4(-_moon.forward, -_moon.up, -_moon.right, Vector4.zero).transpose);

		_time += Time.deltaTime * _normalizedMinutesPerDay;

		float sunProgress = (-_sun.transform.forward.y + 1) * 0.5f;
		_sun.color = _lightSettings.SunColor.Evaluate(sunProgress);
		RenderSettings.ambientLight = _lightSettings.AmbientLight.Evaluate(sunProgress);

		_sun.transform.rotation = Quaternion.identity;
		_sun.transform.Rotate(Vector3.up, -90);
		_sun.transform.Rotate(Vector3.forward, _sunAngle);
		_sun.transform.Rotate(Vector3.right, _time * 360);

		_moon.rotation = Quaternion.identity;
		_moon.Rotate(Vector3.up, 70);
		_moon.Rotate(Vector3.forward, -_sunAngle + _moonAngle);
		_moon.Rotate(Vector3.right, (0.5f - _time) * 360);
		_moonAngle += Time.deltaTime * (_normalizedMinutesPerDay * 5);

		if (_time >= 1)
		{
			_time = 0;
			_currentDay++;
			OnAdvanceDay?.Invoke(_currentDay);
		}
		if (math.abs(_moonAngle) >= 360)
		{
			_moonAngle = 0;
		}
	}

	public bool CompareTime(float time, float threshold = 0.05f) => math.abs(time - _time) < threshold;

	public void SetSunIntensity(float intensity, float duration) => _sun.DOIntensity(intensity, duration);
	public void SetSunIntensity(float intensity) => _sun.intensity = intensity;

#if UNITY_EDITOR
	void OnValidate()
	{
		if (!Application.isPlaying && !_update) return;

		_skybox.SetVector("_MoonDirection", -_moon.forward);
		_skybox.SetMatrix("_MoonSpaceMatrix", new Matrix4x4(-_moon.forward, -_moon.up, -_moon.right, Vector4.zero).transpose);
	}
#endif
}
