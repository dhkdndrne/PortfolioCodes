using UnityEngine;


public abstract class SubProfessionData : ScriptableObject
{
    [SerializeField] private Sprite icon;
    [SerializeField] private string name;
    
    [TextArea,SerializeField] private string description;

    public Sprite Icon => icon;
    public string Name => name;
    public string Description => description;
    
    public abstract void ApplyTrait(OperatorController op);
}
