using UnityEngine;

[CreateAssetMenu(fileName = "Build", menuName = "Building/Build")]
public class BuildData : ScriptableObject
{
	public GameObject BuildObject => _buildObject;

	[SerializeField] GameObject _buildObject;
}
