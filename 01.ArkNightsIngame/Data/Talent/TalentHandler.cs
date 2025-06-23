using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class TalentHandler : MonoBehaviour
{
    [SerializeField] protected TalentData data;
    
    public TalentData Data => data;
    
    public abstract void Initialize(Operator op);
}
public interface ISquadDeployHandler
{
    public void HandleSquadDeploy();
}
