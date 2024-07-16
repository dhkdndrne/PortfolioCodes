using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CachedComponent<T> where T : MonoBehaviour
{
    #region Properties

    public T Root { get; private set; }

    #endregion

    #region Public Methods

    public void Initialize(T root)
    {
        Root = root;
        Initizlize();
    }

    #endregion

    #region Virtual Methods

    protected virtual void Initizlize() { }

    #endregion
}
