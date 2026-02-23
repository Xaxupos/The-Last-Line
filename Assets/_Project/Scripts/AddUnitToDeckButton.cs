using UnityEngine;

public class AddUnitToDeckButton : MonoBehaviour
{
    [SerializeField] private Unit unitPrefab;

    public void Add()
    {
        if (unitPrefab == null)
            return;

        PlayerDeck.Instance.TryAddUnit(unitPrefab);
    }
}
