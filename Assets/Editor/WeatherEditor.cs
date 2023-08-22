using UnityEditor;
using UnityEngine;

public class WeatherEditor : EditorWindow
{
    static int _timesClear;
    static int _timesRain;
    static int _timesStorm;
    static int _day;

    static WeatherManager _weather;

    [MenuItem("Tools/Weather Data")]
    public static void Open()
    {
        CreateWindow<WeatherEditor>("Weather Data");
    }

    [InitializeOnEnterPlayMode]
	static void Start()
	{
		WeatherManager.OnWeatherChange += CountWeather;
        TimeManager.OnAdvanceDay += d => _day = d;
	}

	void OnGUI()
	{
        if (!Application.isPlaying)
        {
            EditorGUILayout.LabelField("Enter play mode");
            return;
        }

        EditorGUILayout.LabelField("Day: " + _day);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Clear: " + _timesClear);
        EditorGUILayout.LabelField("Rain: " + _timesRain);
        EditorGUILayout.LabelField("Storm: " + _timesStorm);
	}

    static void CountWeather(WeatherType weather)
    {
        switch (weather)
        {
            case WeatherType.Clear: _timesClear++; break;
            case WeatherType.Rain: _timesRain++; break;
            case WeatherType.Storm: _timesStorm++; break;
        }
    }
}
