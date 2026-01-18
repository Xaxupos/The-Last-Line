public readonly struct StatModifier
{
    public readonly StatModType Type;
    public readonly float Value;

    public StatModifier(StatModType type, float value)
    {
        Type = type;
        Value = value;
    }
}
