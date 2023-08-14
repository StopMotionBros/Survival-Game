using TMPro;
using UnityEngine;

public class Tooltip : MonoBehaviour
{
	[SerializeField] TMP_Text _title;
	[SerializeField] TMP_Text _description;

	[SerializeField] GameObject _content;
	[SerializeField] CanvasGroup _group;

	public void Show()
	{
		_content.SetActive(true);
	}

	public void Hide()
	{
		_content.SetActive(false);
	}

	void Update()
	{
		if (!_content.activeSelf) return;

		transform.position = Input.mousePosition;
	}

	public void Write(string title, string description)
	{
		_title.SetText(title);
		_description.SetText(description);
	}
}