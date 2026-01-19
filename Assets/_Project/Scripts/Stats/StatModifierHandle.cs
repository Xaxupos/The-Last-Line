public readonly struct StatModifierHandle
{
    public readonly StatId StatId;
    public readonly int Id;

    public bool IsValid => Id != 0;

    internal StatModifierHandle(StatId statId, int id)
    {
        StatId = statId;
        Id = id;
    }
}
