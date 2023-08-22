using UnityEngine;

[CreateAssetMenu(fileName = "Light Settings", menuName = "Settings/Lighting")]
public class LightSettings : ScriptableObject
{
    public Gradient SunColor => _sunColor;
    public Gradient AmbientLight => _ambientLight;

    [SerializeField] Gradient _sunColor;
    [SerializeField] Gradient _ambientLight;
}
