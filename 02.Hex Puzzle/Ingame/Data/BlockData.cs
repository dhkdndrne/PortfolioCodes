using UnityEngine;

[CreateAssetMenu(menuName = "SO/BlockData",fileName = "New BlockData")]
public class BlockData : ScriptableObject
{
	[field: SerializeField] public Sprite Sprite { get; private set; }
	[field: SerializeField] public ColorLayer ColorLayer; //{ get; protected set; }
	[field: SerializeField] public bool CanDrop { get; private set; }
	[field: SerializeField] public bool CanMatch { get; private set; }
	[field: SerializeField] public bool CanSwap { get; private set; }
	[field: SerializeField] public bool CanAffectOther { get; private set; }
	[field: SerializeField] public bool CanAffected { get; private set; }
	[field: SerializeField] public bool CanClick { get; private set; }
	[field: SerializeField] public bool CanMerge { get; private set; }
	//[field: SerializeField] public bool IsTarget { get; private set; }
	[field:SerializeField] public int HP { get; private set; }
}