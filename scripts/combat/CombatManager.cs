using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class CombatManager : Node
{
    public static CombatManager Instance { get; private set; }

    private List<Combatant> _combatants = new();
    private int _currentIndex;

    public Combatant ActiveCombatant => _combatants.Count > 0 ? _combatants[_currentIndex] : null;

    [Signal] public delegate void CombatStartedEventHandler();
    [Signal] public delegate void TurnChangedEventHandler(Combatant combatant);
    [Signal] public delegate void CombatEndedEventHandler(bool playerWon);

    public override void _Ready()
    {
        Instance = this;
    }

    public void BroadcastTrigger(ReactionTrigger trigger)
    {
        foreach (var combatant in _combatants)
        {
            if (combatant.CurrentHp > 0 && combatant != trigger.Source)
                combatant.TryTriggerReactions(trigger);
        }
    }

    public void StartCombat(List<Combatant> combatants)
    {
        _combatants = combatants
            .OrderByDescending(c => Dice.Roll(20) + c.Data.Perception)
            .ToList();

        _currentIndex = 0;

        foreach (var combatant in _combatants)
            combatant.Died += () => OnCombatantDied();

        EmitSignal(SignalName.CombatStarted);
        BeginTurn();
    }

    public void AdvanceTurn()
    {
        _combatants[_currentIndex].EndTurn();

        int next = FindNextLivingIndex();
        if (next == -1)
            return;

        _currentIndex = next;
        BeginTurn();
    }

    private void BeginTurn()
    {
        _combatants[_currentIndex].ResetForNewTurn();
        EmitSignal(SignalName.TurnChanged, _combatants[_currentIndex]);
    }

    private int FindNextLivingIndex()
    {
        int count = _combatants.Count;
        for (int i = 1; i <= count; i++)
        {
            int index = (_currentIndex + i) % count;
            if (_combatants[index].CurrentHp > 0)
                return index;
        }
        return -1;
    }

    private void OnCombatantDied()
    {
        bool allEnemiesDead = _combatants
            .Where(c => c is not Champion)
            .All(c => c.CurrentHp <= 0);

        bool playerDead = _combatants
            .OfType<Champion>()
            .Any(c => c.CurrentHp <= 0);

        if (allEnemiesDead || playerDead)
            EmitSignal(SignalName.CombatEnded, allEnemiesDead);
    }
}
