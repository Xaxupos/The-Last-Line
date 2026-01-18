using UnityEngine;
using UnityEngine.UI;

public class StatusEffectIcon : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Image durationFill;
    [SerializeField] private Text stackText;

    private StatusEffectDefinition _definition;

    public StatusEffectDefinition Definition => _definition;

    public void Bind(StatusEffectDefinition definition)
    {
        _definition = definition;
        if (iconImage != null)
            iconImage.sprite = definition != null ? definition.icon : null;
    }

    public void SetStacks(int stacks)
    {
        if (stackText == null)
            return;

        bool show = stacks > 1;
        stackText.gameObject.SetActive(show);
        if (show)
            stackText.text = stacks.ToString();
    }

    public void SetDuration(float remaining, float total)
    {
        if (durationFill == null)
            return;

        if (float.IsPositiveInfinity(total) || total <= 0f)
        {
            durationFill.fillAmount = 1f;
            return;
        }

        durationFill.fillAmount = Mathf.Clamp01(remaining / total);
    }
}
