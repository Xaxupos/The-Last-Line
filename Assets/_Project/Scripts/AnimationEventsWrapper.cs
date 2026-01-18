using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationEventsWrapper : MonoBehaviour
{
    public List<UnityEvent> animationEvents;

    public void PlayEventAtIndex(int id)
    {
        animationEvents[id]?.Invoke();
    }
}