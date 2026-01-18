using System.Collections.Generic;
using UnityEngine;
using VInspector;

public class StatusEffectUI : MonoBehaviour
{
    [SerializeField] private StatusEffectController controller;
    [SerializeField] private RectTransform container;
    [SerializeField] private StatusEffectIcon iconPrefab;

    public SerializedDictionary<StatusEffectDefinition, StatusEffectIcon> Icons = new();
    private readonly List<StatusEffectInfo> _buffer = new();

    private void OnEnable()
    {
        if (controller == null)
            return;

        controller.OnEffectAdded += HandleEffectAdded;
        controller.OnEffectRemoved += HandleEffectRemoved;
    }

    private void OnDisable()
    {
        if (controller == null)
            return;

        controller.OnEffectAdded -= HandleEffectAdded;
        controller.OnEffectRemoved -= HandleEffectRemoved;
    }

    private void Update()
    {
        if (controller == null || iconPrefab == null || container == null)
            return;

        controller.GetActiveEffects(_buffer);
        for (int i = 0; i < _buffer.Count; i++)
        {
            var info = _buffer[i];
            if (!Icons.TryGetValue(info.Definition, out var icon))
            {
                icon = CreateIcon(info.Definition);
                if (icon == null)
                    continue;
            }

            icon.SetStacks(info.Stacks);
            icon.SetDuration(info.RemainingDuration, info.TotalDuration);
        }
    }

    private void HandleEffectAdded(StatusEffectInfo info)
    {
        if (iconPrefab == null || container == null)
            return;

        if (Icons.ContainsKey(info.Definition))
            return;

        CreateIcon(info.Definition);
    }

    private void HandleEffectRemoved(StatusEffectInfo info)
    {
        if (!Icons.TryGetValue(info.Definition, out var icon))
            return;

        Icons.Remove(info.Definition);
        if (icon != null)
            Destroy(icon.gameObject);
    }

    private StatusEffectIcon CreateIcon(StatusEffectDefinition definition)
    {
        var icon = Instantiate(iconPrefab, container);
        icon.Bind(definition);
        Icons[definition] = icon;
        return icon;
    }
}
