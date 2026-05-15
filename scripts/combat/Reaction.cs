public abstract class Reaction
{
    public abstract bool CanTrigger(Combatant owner, ReactionTrigger trigger);
    public abstract void Execute(Combatant owner, ReactionTrigger trigger);
}
