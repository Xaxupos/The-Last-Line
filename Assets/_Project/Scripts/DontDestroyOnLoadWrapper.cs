using UnityEngine;

public class DontDestroyOnLoadWrapper : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
