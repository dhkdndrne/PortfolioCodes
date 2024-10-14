using UnityEngine;
public static class Define
{
	public static readonly int MOVE_ANIM_HASH = Animator.StringToHash("IsMove");
	public static readonly int Attack_ANIM_HASH = Animator.StringToHash("Attack");
	public static readonly int Skill_ANIM_HASH = Animator.StringToHash("Skill");
	public static readonly int Dead_ANIM_HASH = Animator.StringToHash("Dead");

	public static readonly int ITEM_GRID_MAX_COL = 5;
	public static readonly int ITEM_GRID_MAX_ROW = 5;

	public static readonly LayerMask INVENSLOT_LAYERMASK = LayerMask.GetMask("InvenSlot");

	public static readonly float ITEM_INFO_ONMOUSE_TICK = 0.3f;
	public static readonly string GROUND_TAG = "Ground";
	
	public static readonly int[] DIR_X = { -1, 1, 0, 0 };
	public static readonly int[] DIR_Y = { 0, 0, -1, 1 };
}