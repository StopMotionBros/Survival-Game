using UnityEngine;

[CreateAssetMenu(fileName = "Light Settings", menuName = "Lighting/Light Settings")]
public class LightSettings : ScriptableObject
{
    [SerializeField] Gradient _skyColor;
    [SerializeField] AnimationCurve _skyPower;
    
    [SerializeField] Gradient _horrizonColor;

    [SerializeField] AnimationCurve _voidPower;
    [SerializeField] Gradient _voidColor;

    [SerializeField] Gradient _sunColor;
    [SerializeField] AnimationCurve _sunIntensity;
}
