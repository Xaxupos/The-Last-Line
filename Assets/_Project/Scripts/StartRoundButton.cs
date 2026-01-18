using UnityEngine;
using UnityEngine.SceneManagement;

public class StartRoundButton : MonoBehaviour
{
    [SerializeField] private string sceneName;
    [SerializeField] private RoundDefinition roundDefinition;

    public void StartRound()
    {
        if (roundDefinition == null)
        {
            Debug.LogError("StartRoundButton missing RoundDefinition.", this);
            return;
        }

        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogError("StartRoundButton missing scene name.", this);
            return;
        }

        RunStateManager.Instance.SetPendingRound(roundDefinition);
        SceneManager.LoadScene(sceneName);
    }
}
