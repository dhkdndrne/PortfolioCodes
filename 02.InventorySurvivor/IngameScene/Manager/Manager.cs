using System.Collections;
using System.Collections.Generic;
using Bam.Singleton;
using UnityEngine;

public abstract class Manager<T> : Singleton<T>,IInitializer where T : Manager<T>
{
	public virtual void OnStartManager(){}
}

public interface IInitializer
{
	public void OnStartManager();
}


