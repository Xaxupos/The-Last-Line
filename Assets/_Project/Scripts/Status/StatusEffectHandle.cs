public readonly struct StatusEffectHandle
{
    public readonly int Id;

    public bool IsValid => Id != 0;

    internal StatusEffectHandle(int id)
    {
        Id = id;
    }
}
