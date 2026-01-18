using System;
using System.Collections.Generic;

public class UnitsManager : MonoBehaviourSingleton<UnitsManager>
{
    public List<Unit> _players = new();
    public List<Unit> _enemies = new();

    public event Action<Unit> OnUnitRegistered;
    public event Action<Unit> OnUnitDied;

    public IReadOnlyList<Unit> Players => _players;
    public IReadOnlyList<Unit> Enemies => _enemies;

    public int AlivePlayers => CountAlive(_players);
    public int AliveEnemies => CountAlive(_enemies);

    public void Register(Unit unit)
    {
        var list = unit.team == Team.Player ? _players : _enemies;
        if (!list.Contains(unit))
        {
            list.Add(unit);
            OnUnitRegistered?.Invoke(unit);
        }
    }

    public void Unregister(Unit unit)
    {
        _players.Remove(unit);
        _enemies.Remove(unit);
    }

    public void NotifyUnitDied(Unit unit)
    {
        Unregister(unit);
        OnUnitDied?.Invoke(unit);
    }

    private int CountAlive(List<Unit> list)
    {
        int c = 0;
        foreach (var u in list)
            if (u != null) c++;
        return c;
    }
}
