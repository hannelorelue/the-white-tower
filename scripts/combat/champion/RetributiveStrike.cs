public class RetributiveStrike : Reaction
{
    public override bool CanTrigger(Combatant owner, ReactionTrigger trigger)
        => trigger.Type == ReactionTriggerType.DamageTaken
        && trigger.Target == owner
        && trigger.Source != null;

    public override void Execute(Combatant owner, ReactionTrigger trigger)
    {
        if (!owner.SpendReaction())
            return;

        var result = StrikeAction.Execute(owner, trigger.Source, situationalBonus: -2);
        Godot.GD.Print($"Retributive Strike: {result.Outcome} | Roll: {result.Roll} | Total: {result.TotalToHit} vs AC {trigger.Source.Data.ArmorClass} | Damage: {result.DamageDealt}");
    }
}
