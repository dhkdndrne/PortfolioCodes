using System.Collections.Generic;
using UnityEngine;
public class Skill : ScriptableObject
{
    [SerializeField] private SkillType skillType;
    [Multiline,SerializeField] private string description;

    public SkillType SkillType => skillType;
    public string Description => description;
}



