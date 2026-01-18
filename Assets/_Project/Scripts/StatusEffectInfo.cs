[System.Serializable]
public readonly struct StatusEffectInfo
{
    public readonly StatusEffectDefinition Definition;
    public readonly float RemainingDuration;
    public readonly float TotalDuration;
    public readonly int Stacks;
    public readonly float Strength;
    public readonly Unit Source;

    public bool IsPermanent => float.IsPositiveInfinity(RemainingDuration);

    public StatusEffectInfo(
        StatusEffectDefinition definition,
        float remainingDuration,
        float totalDuration,
        int stacks,
        float strength,
        Unit source)
    {
        Definition = definition;
        RemainingDuration = remainingDuration;
        TotalDuration = totalDuration;
        Stacks = stacks;
        Strength = strength;
        Source = source;
    }
}
