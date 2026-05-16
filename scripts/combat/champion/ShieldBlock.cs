public class ShieldBlock : Reaction
{
    public override bool CanTrigger(Combatant owner, ReactionTrigger trigger)
        => trigger.Type == ReactionTriggerType.BeforeDamageTaken
        && trigger.Target == owner
        && owner is Champion champion
        && champion.IsShieldRaised;

    public override void Execute(Combatant owner, ReactionTrigger trigger)
    {
        if (!owner.SpendReaction())
            return;

        var champion = (Champion)owner;
        int reduced = champion.BlockWithShield(trigger.Amount);
        Godot.GD.Print($"Shield Block! Damage reduced from {trigger.Amount} to {reduced}");
        trigger.Amount = reduced;
    }
}
