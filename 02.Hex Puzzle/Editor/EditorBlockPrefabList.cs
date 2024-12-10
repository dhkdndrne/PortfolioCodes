using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Editor BlockPrefabList", fileName = "New Editor BlockPrefabList")]
public class EditorBlockPrefabList : ScriptableObject
{
	[SerializeField] private GameObject blockPrefab;
	[SerializeField] private List<EditorSBlockToken> sBlockList;

	public GameObject GetBlockPrefab(SpecialBlockType type)
	{
		return type is SpecialBlockType.None ? blockPrefab :sBlockList.Where(x => x.specialBlockType == type).Select(x => x.blockPrefab).FirstOrDefault();
	}

	[System.Serializable]
	private class EditorSBlockToken
	{
		public SpecialBlockType specialBlockType;
		public GameObject blockPrefab;
	}
}