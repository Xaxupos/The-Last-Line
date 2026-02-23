using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDeckSlotUI : MonoBehaviour
{
    [SerializeField] private int slotIndex;
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text label;
    [SerializeField] private Sprite emptyIcon;
    [SerializeField] private string emptyLabel = "Empty";
    [SerializeField] private Selectable interactableTarget;

    private PlayerDeckPanel _owner;

    public void Initialize(PlayerDeckPanel owner, int index)
    {
        _owner = owner;
        slotIndex = index;
    }

    public void SetUnit(Unit unit)
    {
        if (icon != null)
        {
            if (unit == null)
            {
                icon.sprite = null;
                icon.enabled = false;
            }
            else
            {
                Sprite sprite = null;
                if (unit.definition != null)
                    sprite = unit.definition.icon;

                icon.sprite = sprite;
                icon.enabled = sprite != null;
            }
        }

        if (label != null)
        {
            if (unit == null)
            {
                label.text = emptyLabel;
            }
            else if (unit.definition != null && !string.IsNullOrWhiteSpace(unit.definition.displayName))
            {
                label.text = unit.definition.displayName;
            }
            else
            {
                label.text = unit.name;
            }
        }
    }

    public void SetInteractable(bool value)
    {
        if (interactableTarget != null)
            interactableTarget.interactable = value;
    }

    public void OnClickSlot()
    {
        _owner?.HandleSlotClicked(slotIndex);
    }
}
