using UnityEngine;

[CreateAssetMenu(fileName = "Build", menuName = "Data/Building")]
public class BuildData : ScriptableObject
{
	public GameObject BuildObject => _buildObject;

	[SerializeField] GameObject _buildObject;
}
