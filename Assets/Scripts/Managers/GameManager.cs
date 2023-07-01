using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static void ToggleCursor(bool enabled)
	{
		Cursor.visible = enabled;
		Cursor.lockState = enabled ? CursorLockMode.None : CursorLockMode.Locked;
	}
}