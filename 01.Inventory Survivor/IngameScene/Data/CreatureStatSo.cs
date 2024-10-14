using UnityEngine;

[CreateAssetMenu(menuName = "SO/Stat/CreatureStat",fileName = "New Creature stat")]
public class CreatureStatSo : ScriptableObject
{
    [SerializeField] private int id;
    [SerializeField] private int hp;
    [SerializeField] private float moveSpeed;
    
    [Space(15)]
    [SerializeField] private DamageType damageType;
    [SerializeField] private float atkPower;
    [SerializeField] private float atkRange;
    [SerializeField] private float atkCoolTime;
    
    [Space(15)]
    [SerializeField] private float projectile_Guard;
    [SerializeField] private float melee_Guard;
    [SerializeField] private float magic_Guard;
    [SerializeField] private float dodgeChance;

    
    public int Hp => hp;
    public float MoveSpeed => moveSpeed;
    public DamageType DamageType => damageType;
    public float AtkPower => atkPower;
    public float AtkRange => atkRange;
    public float Projectile_Gurad => projectile_Guard;
    public float Melee_Guard => melee_Guard;
    public float Magic_Guard => magic_Guard;
    public float DodgeChance => dodgeChance;
    public float AtkCoolTime => atkCoolTime;
}
