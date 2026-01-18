using UnityEngine;

public abstract class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviourSingleton<T>
{
    private static T _instance;
    public static bool HasInstance => _instance != null;
    public static T InstanceOrNull => _instance;
    public static T Instance
    {
        get
        {
            if (_instance != null) return _instance;

            _instance = FindFirstObjectByType<T>();
            if (_instance != null) return _instance;

            var go = new GameObject(typeof(T).Name);
            _instance = go.AddComponent<T>();
            return _instance;
        }
    }

    [SerializeField] private bool dontDestroyOnLoad = true;

    protected virtual void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = (T)this;

        if (dontDestroyOnLoad)
            DontDestroyOnLoad(gameObject);
    }

    protected virtual void OnDestroy()
    {
        if (_instance == this) _instance = null;
    }
}
