using UnityEngine;

public class CursorSlot : UIItemSlot
{
	public static CursorSlot Instance;
	[SerializeField] CursorSlot dfa;
	
	protected override void Awake()
	{
		base.Awake();

		Instance = this;
		dfa = Instance;
	}

	protected override void Start()
	{
		UpdateSlot();
	}
}