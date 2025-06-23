public interface IRangeModifyingSkill
{
    public int RangeID { get;}
    public GridType[,] GetModifiedAttackRange();
}
