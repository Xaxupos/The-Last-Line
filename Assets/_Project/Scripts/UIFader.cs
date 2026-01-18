using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class UIFader : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float defaultDuration = 0.25f;
    [SerializeField] private bool useUnscaledTime = true;
    [SerializeField] private bool enableInteractionWhenVisible = true;
    [SerializeField] private bool blockRaycastsWhenVisible = true;
    [SerializeField] private bool toggleActiveOnFade = true;

    [Header("Events")]
    [SerializeField] private UnityEvent OnFadeInStart;
    [SerializeField] private UnityEvent OnFadeInEnd;
    [SerializeField] private UnityEvent OnFadeOutStart;
    [SerializeField] private UnityEvent OnFadeOutEnd;

    private Tween _fadeTween;

    public void FadeIn()
    {
        FadeIn(defaultDuration);
    }

    public void FadeOut()
    {
        FadeOut(defaultDuration);
    }

    public void FadeIn(float duration)
    {
        StartFade(1f, Mathf.Max(0f, duration), OnFadeInStart, OnFadeInEnd);
    }

    public void FadeOut(float duration)
    {
        StartFade(0f, Mathf.Max(0f, duration), OnFadeOutStart, OnFadeOutEnd);
    }

    public void SetInstant(float alpha)
    {
        if (canvasGroup == null)
            return;

        _fadeTween?.Kill();
        _fadeTween = null;

        canvasGroup.alpha = Mathf.Clamp01(alpha);
        UpdateInteraction(canvasGroup.alpha >= 0.999f);
        UpdateActiveState(canvasGroup.alpha > 0f);
    }

    private void StartFade(float targetAlpha, float duration, UnityEvent onStart, UnityEvent onEnd)
    {
        if (canvasGroup == null)
            return;

        if (toggleActiveOnFade && targetAlpha > 0f && !gameObject.activeSelf)
            gameObject.SetActive(true);

        if (duration <= 0f)
        {
            canvasGroup.alpha = targetAlpha;
            UpdateInteraction(targetAlpha >= 0.999f);
            onStart?.Invoke();
            onEnd?.Invoke();
            if (targetAlpha <= 0f)
                UpdateActiveState(false);
            return;
        }

        _fadeTween?.Kill();
        _fadeTween = canvasGroup
            .DOFade(targetAlpha, duration)
            .SetUpdate(useUnscaledTime)
            .OnStart(() => onStart?.Invoke())
            .OnComplete(() =>
            {
                UpdateInteraction(targetAlpha >= 0.999f);
                onEnd?.Invoke();
                if (targetAlpha <= 0f)
                    UpdateActiveState(false);
                _fadeTween = null;
            });
    }

    private void UpdateInteraction(bool visible)
    {
        if (canvasGroup == null)
            return;

        canvasGroup.interactable = visible && enableInteractionWhenVisible;
        canvasGroup.blocksRaycasts = visible && blockRaycastsWhenVisible;
    }

    private void UpdateActiveState(bool visible)
    {
        if (!toggleActiveOnFade)
            return;

        if (visible && !gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
        else if (!visible && gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
    }
}
