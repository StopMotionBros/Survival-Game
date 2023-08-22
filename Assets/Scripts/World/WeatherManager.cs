using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class WeatherManager : MonoBehaviour
{
	public static WeatherManager Instance;
	public static System.Action<WeatherType> OnWeatherChange;

	public static WeatherType Weather { get; private set; }
	[SerializeField] WeatherType _weather;
	[SerializeField] WeatherInstance[] _weatherInstances;
	WeatherInstance _currentInstance;

	WeatherType _weatherThisDay;
	float _timeOfWeatherChange;

    [SerializeField] Material _skybox;

	[SerializeField] Volume _postProcessing;

	[SerializeField] GameObject _rain;
	[SerializeField] GameObject _lightning;

	void Awake()
	{
		_timeOfWeatherChange = -1;
		Instance = this;
		TimeManager.OnAdvanceDay += CheckWeather;

		SetWeather(_weather);
	}

	void Update()
	{
#if UNITY_EDITOR
		if (Input.GetKeyDown(KeyCode.P))
		{
			SetWeather((WeatherType)(((int)_weather + 1) % 3));
		}
#endif

		switch (_weather)
		{
			case WeatherType.Storm:
				SpawnLightning();
				break;
		}

		if (_timeOfWeatherChange < 0) return;

		if (TimeManager.Instance.CompareTime(_timeOfWeatherChange))
		{
			SetWeather(_weatherThisDay);
			_timeOfWeatherChange = -1;
		}
	}

	void CheckWeather(int day)
	{
		int rng = Random.Range(0, 101);
		for (int i = 0; i < _weatherInstances.Length; i++)
		{
			if (rng < _weatherInstances[i].Chance)
			{
				_weatherThisDay = (WeatherType)i;
				_timeOfWeatherChange = Random.Range(0, 1);
			}
		}
	}

	bool _flashing;
	public void Flash(Color color)
	{
		if (_flashing) return;
		_flashing = true;

		Color ogAOLight = RenderSettings.ambientLight;
		DOVirtual.Color(color, ogAOLight, 1, v => RenderSettings.ambientLight = v).OnComplete(() => _flashing = false);

		_postProcessing.profile.TryGet(out ColorAdjustments colorAdj);
		float ogExposure = colorAdj.postExposure.value;
		DOVirtual.Float(color.a, ogExposure, 1, v => colorAdj.postExposure.value = v);

		Color ogCloudColor = _skybox.GetColor("_CloudColor");
		DOVirtual.Color(color, ogCloudColor, 1, v => _skybox.SetColor("_CloudColor", v));
	}

	public void SetWeather(WeatherType weather)
	{
		_currentInstance.OnDeactivate?.Invoke();

		_weather = weather;
		_currentInstance = _weatherInstances[(int)_weather];
		WeatherData currentData = _currentInstance.Data;

		float duration = 3;
		_skybox.DOFloat(currentData.CloudDensity, "_CloudDensity", duration);
		_skybox.DOFloat(currentData.CloudFalloff, "_CloudFalloff", duration);
		_skybox.DOFloat(currentData.CloudSpeed, "_CloudSpeed", duration);
		_skybox.DOColor(currentData.CloudColor, "_CloudColor", duration);

		_skybox.DOFloat(currentData.SkyIntensity, "_SkyIntensity", duration);

		if (_postProcessing.profile.TryGet(out ColorAdjustments color))
			DOVirtual.Float(color.postExposure.value, currentData.SunIntensity, duration, v => color.postExposure.value = v);

		_currentInstance.OnActivate?.Invoke();

		OnWeatherChange?.Invoke(_weather);
		Weather = _weather;
	}

	public void ToggleRain(bool enabled) => _rain.SetActive(enabled);

	bool _lightninged;
	void SpawnLightning()
	{
		if (!_lightninged) StartCoroutine(C_Lightning(Random.Range(5, 20)));
	}

	IEnumerator C_Lightning(float time)
	{
		_lightninged = true;

		yield return new WaitForSeconds(time);

		Instantiate(_lightning, new Vector3(Random.Range(-200, 200), 0, Random.Range(-200, 200)), Quaternion.identity);
		Flash(Color.white);

		_lightninged = false;
	}

	void OnDestroy()
	{
		_weather = WeatherType.Clear;
		_currentInstance = _weatherInstances[0];
		WeatherData currentData = _currentInstance.Data;

		_skybox.SetFloat("_CloudDensity", currentData.CloudDensity);
		_skybox.SetFloat("_CloudFalloff", currentData.CloudFalloff);
		_skybox.SetFloat("_CloudSpeed", currentData.CloudSpeed);
		_skybox.SetColor("_CloudColor", currentData.CloudColor);

		_skybox.SetFloat("_SkyIntensity", currentData.SkyIntensity);
	}
}
public enum WeatherType
{
	Clear,
	Rain,
	Storm
}
[System.Serializable]
public struct WeatherInstance
{
	public WeatherData Data;
	public int Chance;
	public UnityEvent OnActivate;
	public UnityEvent OnDeactivate;
}
