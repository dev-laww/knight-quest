using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; protected set; }

    [Tooltip("Prevent this object from being destroyed on scene load")]
    public bool dontDestroyOnLoad = true;

    private void Awake()
    {
        if (Instance is null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this as T;

        if (!dontDestroyOnLoad) return;

        DontDestroyOnLoad(gameObject);
        OnAwake();
    }

    protected virtual void OnAwake() { }
}