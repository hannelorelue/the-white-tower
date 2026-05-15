using Godot;
using System.Collections.Generic;
public partial class Combatant : Node2D
{
    [Export] public CombatantData Data { get; set; }

    public List<Reaction> Reactions { get; } = [];

    [ExportGroup("Nodes")]
    [Export] private Sprite2D _sprite;
    [Export] private AnimationPlayer _animationPlayer;

    public int CurrentHp { get; private set; }
    public int ActionsRemaining { get; private set; }
    public bool ReactionAvailable { get; private set; }
    public int MultipleAttackPenalty { get; private set; }

    [Signal] public delegate void HpChangedEventHandler(int current, int max);
    [Signal] public delegate void DiedEventHandler();
    [Signal] public delegate void TurnStartedEventHandler();
    [Signal] public delegate void TurnEndedEventHandler();
    [Signal] public delegate void ActionSpentEventHandler(int actionsRemaining);
    [Signal] public delegate void ReactionSpentEventHandler();

    public override void _Ready()
    {
        CurrentHp = Data.MaxHp;
    }

    public void TakeDamage(int amount, Combatant source = null)
    {
        CurrentHp = Mathf.Max(0, CurrentHp - amount);
        EmitSignal(SignalName.HpChanged, CurrentHp, Data.MaxHp);

        if (source != null)
            CombatManager.Instance.BroadcastTrigger(new ReactionTrigger
            {
                Type = ReactionTriggerType.DamageTaken,
                Source = source,
                Target = this,
                Amount = amount
            });

        if (CurrentHp == 0)
            EmitSignal(SignalName.Died);
    }

    public void Heal(int amount)
    {
        CurrentHp = Mathf.Min(Data.MaxHp, CurrentHp + amount);
        EmitSignal(SignalName.HpChanged, CurrentHp, Data.MaxHp);
    }

    public bool SpendAction()
    {
        if (ActionsRemaining <= 0)
            return false;

        ActionsRemaining--;
        MultipleAttackPenalty = ActionsRemaining == 2 ? -5 : -10;
        EmitSignal(SignalName.ActionSpent, ActionsRemaining);
        return true;
    }

    public bool SpendReaction()
    {
        if (!ReactionAvailable)
            return false;

        ReactionAvailable = false;
        EmitSignal(SignalName.ReactionSpent);
        return true;
    }

    public void ResetForNewTurn()
    {
        ActionsRemaining = 3;
        ReactionAvailable = true;
        MultipleAttackPenalty = 0;
        EmitSignal(SignalName.TurnStarted);
    }

    public void TryTriggerReactions(ReactionTrigger trigger)
    {
        if (!ReactionAvailable)
            return;

        foreach (var reaction in Reactions)
        {
            if (reaction.CanTrigger(this, trigger))
            {
                reaction.Execute(this, trigger);
                return;
            }
        }
    }

    public void EndTurn()
    {
        EmitSignal(SignalName.TurnEnded);
    }
}
