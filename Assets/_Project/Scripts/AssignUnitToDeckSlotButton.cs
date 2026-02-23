using UnityEngine;

public class AssignUnitToDeckSlotButton : MonoBehaviour
{
    [SerializeField] private Unit unitPrefab;
    [SerializeField] private int slotIndex;

    public void Assign()
    {
        if (unitPrefab == null)
            return;

        PlayerDeck.Instance.TryAssignUnitToSlot(unitPrefab, slotIndex);
    }
}
