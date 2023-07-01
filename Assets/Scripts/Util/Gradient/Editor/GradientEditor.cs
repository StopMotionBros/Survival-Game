using UnityEngine;
using UnityEditor;

public class GradientEditor : EditorWindow {

    Grad gradient;
    const int BORDER_SIZE = 10;
    const float KEY_WIDTH = 10;
    const float KEY_HEIGHT = 20;

	static bool _randomizeColor;
	static bool _addKeys = true;
	static bool _editKeys = true;

	Rect gradientPreviewRect;
    Rect[] keyRects;
    bool mouseIsDownOverKey;
    int selectedKeyIndex;
    bool needsRepaint;

	Texture2D transparentTexture;

    private void OnGUI()
    {
        Draw();
        HandleInput();

        if (needsRepaint)
        {
            needsRepaint = false;
            Repaint();
        }
    }

    void Draw()
    {
		GUILayout.BeginHorizontal();

		_addKeys = EditorGUILayout.Toggle("Add Keys", _addKeys);
		_editKeys = EditorGUILayout.Toggle("Edit Keys", _editKeys);

		GUILayout.EndHorizontal();

		GUILayout.Label("Key Count: " + gradient.KeyCount);

		gradientPreviewRect = new Rect(BORDER_SIZE, BORDER_SIZE * 5f, position.width - BORDER_SIZE * 2, 25);

		transparentTexture = Resources.Load<Texture2D>("Textures/TransparentTexture");

		GUI.DrawTexture(gradientPreviewRect, transparentTexture, ScaleMode.ScaleAndCrop);
		GUI.DrawTexture(gradientPreviewRect, gradient.GetTexture((int)gradientPreviewRect.width));

		keyRects = new Rect[gradient.KeyCount];
		for (int i = 0; i < gradient.KeyCount; i++)
		{
			Grad.ColorKey key = gradient.GetKey(i);
			Rect keyRect = new Rect(gradientPreviewRect.x + gradientPreviewRect.width * key.Time - KEY_WIDTH / 2f, gradientPreviewRect.yMax + BORDER_SIZE, KEY_WIDTH, KEY_HEIGHT);
			if (i == selectedKeyIndex)
			{
				EditorGUI.DrawRect(new Rect(keyRect.x - 2, keyRect.y - 2, keyRect.width + 4, keyRect.height + 4), Color.black);
			}
			Rect top = new Rect(gradientPreviewRect.x + gradientPreviewRect.width * key.Time - KEY_WIDTH / 2f, gradientPreviewRect.yMax + BORDER_SIZE, KEY_WIDTH, KEY_HEIGHT / 2);
			Rect bottom = new Rect(gradientPreviewRect.x + gradientPreviewRect.width * key.Time - KEY_WIDTH / 2f, gradientPreviewRect.yMax + BORDER_SIZE + (KEY_HEIGHT / 2), KEY_WIDTH, KEY_HEIGHT / 2);
			GUI.DrawTexture(bottom, transparentTexture, ScaleMode.ScaleAndCrop);
			EditorGUI.DrawRect(bottom, key.Color);
			EditorGUI.DrawRect(top, new Color(key.Color.r, key.Color.g, key.Color.b));
			keyRects[i] = keyRect;
		}

        Rect settingsRect = new Rect(BORDER_SIZE, keyRects[0].yMax + BORDER_SIZE, position.width - BORDER_SIZE * 2, position.height);
        GUILayout.BeginArea(settingsRect);
        EditorGUI.BeginChangeCheck();
        Color newColor = EditorGUILayout.ColorField(gradient.GetKey(selectedKeyIndex).Color);
        if (EditorGUI.EndChangeCheck())
        {
            gradient.UpdateKeyColor(selectedKeyIndex, newColor);
        }
        gradient.BlendMode = (BlendMode)EditorGUILayout.EnumPopup("Blend mode", gradient.BlendMode);
        _randomizeColor = EditorGUILayout.Toggle("Randomize color", _randomizeColor);
		selectedKeyIndex = gradient.UpdateKeyTime(selectedKeyIndex, EditorGUILayout.Slider("Time", gradient.GetKey(selectedKeyIndex).Time, 0f, 1f));
        GUILayout.EndArea();
    }

    void HandleInput()
    {
		Event guiEvent = Event.current;
		if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0)
		{
			if (_editKeys)
			{
				for (int i = 0; i < keyRects.Length; i++)
				{
					if (keyRects[i].Contains(guiEvent.mousePosition))
					{
						mouseIsDownOverKey = true;
						selectedKeyIndex = i;
						needsRepaint = true;
						break;
					}
				}
			}

			if (_addKeys && !mouseIsDownOverKey)
			{
				
				float keyTime = Mathf.InverseLerp(gradientPreviewRect.x, gradientPreviewRect.xMax, guiEvent.mousePosition.x);
                Color interpolatedColor = gradient.Evaluate(keyTime);
                Color randomColor = new Color(Random.value, Random.value, Random.value);

                selectedKeyIndex = gradient.AddKey((_randomizeColor)?randomColor:interpolatedColor, keyTime);
				mouseIsDownOverKey = true;
				needsRepaint = true;
			}
		}

		if (guiEvent.type == EventType.MouseUp && guiEvent.button == 0)
		{
			mouseIsDownOverKey = false;
		}

		if (mouseIsDownOverKey && guiEvent.type == EventType.MouseDrag && guiEvent.button == 0)
		{
			float keyTime = Mathf.InverseLerp(gradientPreviewRect.x, gradientPreviewRect.xMax, guiEvent.mousePosition.x);
			selectedKeyIndex = gradient.UpdateKeyTime(selectedKeyIndex, keyTime);
			needsRepaint = true;
		}

		if (guiEvent.keyCode == KeyCode.Backspace && guiEvent.type == EventType.KeyDown)
		{
			gradient.RemoveKey(selectedKeyIndex);
			if (selectedKeyIndex >= gradient.KeyCount)
			{
				selectedKeyIndex--;
			}
			needsRepaint = true;
		}
    }

    public void SetGradient(Grad gradient)
    {
        this.gradient = gradient;
    }

    private void OnEnable()
    {
        titleContent.text = "Gradient Editor";
        position.Set(position.x, position.y, 400, 150);
        minSize = new Vector2(200, 200);
        maxSize = new Vector2(1920, 1080);
    }

    private void OnDisable()
    {
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
    }
}
