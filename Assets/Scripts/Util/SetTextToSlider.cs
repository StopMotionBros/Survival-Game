using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SetTextToSlider : MonoBehaviour
{
    [SerializeField] Slider _slider;
    [SerializeField] TMP_Text _text;

#if UNITY_EDITOR
	void OnValidate()
	{
		if (!_slider || !_text) return;
		SetText();
	}
#endif

	void Awake()
	{
		if (!_slider || !_text) throw new System.AccessViolationException("Slider or text not assigned.");

		_slider.onValueChanged.AddListener(v => SetText());
	}

	void OnDestroy()
	{
		_slider.onValueChanged.RemoveListener(v => SetText());
	}

	void OnEnable()
	{
		SetText();
	}

	public void SetText()
	{
		_text.SetText(_slider.value + "/" + _slider.maxValue);
	}
}
