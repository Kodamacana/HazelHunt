using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RichTap.Internal
{
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        #region Instance field
        private static T instance;
        private static readonly object Lock = new object();
        [SerializeField]
        private bool persistent = true;
        #endregion

        #region Property
        public static bool Quitting { get; private set; }
        #endregion

        public static T Instance
        {
            get
            {
                if (Quitting)
                {
                    Debug.LogWarning($"[Singleton<{typeof(T)}>] Instance will not be returned because of quitting.");
                    return null;
                }
                lock (Lock)
                {
                    if (instance != null)
                    {
                        return instance;
                    }
                    T[] instances = FindObjectsOfType<T>();
                    int count = instances.Length;

                    if (count == 0)
                    {
                        return instance = new GameObject($"(Singleton){typeof(T)}").AddComponent<T>();
                    }
                    if (count > 1)
                    {
                        for (int i = 1; i < instances.Length; i++)
                        {
                            Destroy(instances[i]);
                        }
                    }
                    return instance = instances[0];
                }
            }
        }


        #region Methods
        private void Awake()
        {
            if (persistent)
            {
                T[] instances = FindObjectsOfType<T>();
                if (instances.Length > 1)
                {
                    Destroy(gameObject);
                    return;
                }
                else
                {
                    DontDestroyOnLoad(gameObject);
                }
            }
            OnApplicationAwaken();
        }


        private void OnApplicationQuit()
        {
            Quitting = true;
            OnApplicationQuitting();
        }

        protected virtual void OnApplicationAwaken() { }
        protected virtual void OnApplicationQuitting() { }
        #endregion
    }
}
