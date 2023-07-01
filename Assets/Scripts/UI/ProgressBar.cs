using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(CanvasGroup))]
public class ProgressBar : MonoBehaviour
{
#if UNITY_EDITOR
	[MenuItem("GameObject/UI/Progress Bar")]
	public static void CreateProgressBar()
	{
		GameObject obj = Instantiate(Resources.Load<GameObject>("UI/ProgressBar"));
		obj.transform.SetParent(Selection.activeTransform, false);
	}
	[MenuItem("GameObject/UI/Radial Progress Bar")]
	public static void CreateRadialProgressBar()
	{
		GameObject obj = Instantiate(Resources.Load<GameObject>("UI/RadialProgressBar"));
		obj.transform.SetParent(Selection.activeTransform, false);
	}
#endif

	#region Getters

	public float Value { get => _value; set => _value = value; }
	public float Min { get => _min; set => _min = value; }
	public float Max { get => _max; set => _max = value; }

	#endregion

	[SerializeField] Image _fill;
	[SerializeField] Image _fillGraphic;
	[SerializeField] Gradient _fillColor;

	[Space]

	[SerializeField] Image.FillMethod _fillMethod;
	[SerializeField] TMP_Text _text;
	[SerializeField] string _valueFormat = "{0}";

	[Space]

	[SerializeField] float _value;
	float _prevValue;
	[SerializeField] float _min;
	[SerializeField] float _max;

	[Space]

	[SerializeField] UnityEvent<float> _onValidate;

#if UNITY_EDITOR
	public void OnValidate()
	{	
		if (_value != _prevValue)
		{
			if (_fill != null)
			{
				_fill.fillMethod = _fillMethod;

				_value = Mathf.Clamp(_value, _min, _max);

				float normalizedValue = Mathf.InverseLerp(_min, _max, _value);
				_fill.fillAmount = normalizedValue;
				_fillGraphic.color = _fillColor.Evaluate(normalizedValue);

				if (_text)
				{
					_text.SetText(string.Format(_valueFormat, _value, Mathf.InverseLerp(_min, _max, _value) * 100f, _min, _max, _prevValue));
				}

				_prevValue = _value;
			}
		}
	}
#endif

	public void Update()
	{
		if (_value != _prevValue)
		{
			if (_fill != null)
			{
				if (_fill.fillMethod != _fillMethod) _fill.fillMethod = _fillMethod;

				_value = Mathf.Clamp(_value, _min, _max);

				float normalizedValue = Mathf.InverseLerp(_min, _max, _value);
				_fill.fillAmount = normalizedValue;
				_fillGraphic.color = _fillColor.Evaluate(normalizedValue);

				if (_text)
				{
					_text.SetText(string.Format(_valueFormat, _value, Mathf.InverseLerp(_min, _max, _value) * 100f, _min, _max, _prevValue));
				}

				_onValidate?.Invoke(_value);
				_prevValue = _value;
			}
		}
	}
}
