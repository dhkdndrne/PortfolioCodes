using System;
using UnityEngine;

namespace Bam.Singleton
{
    public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance is null)
                {
                    instance = FindObjectOfType<T>() ?? new GameObject(typeof(T).Name).AddComponent<T>();
                }

                return instance;
            }
        }

        protected virtual void Awake()
        {
            var objs = FindObjectsOfType<T>();
            if (objs.Length != 1)
            {
                Destroy(gameObject);
                Debug.LogError("중복 오브젝트 객체가 있어 파괴됩니다.");
            }
        }

        protected void OnDestroy()
        {
            if (instance == this)
                instance = null;
        }
    }
}