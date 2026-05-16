public enum ReactionTriggerType { BeforeDamageTaken, DamageTaken, AllyDamageTaken }

public class ReactionTrigger
{
    public ReactionTriggerType Type;
    public Combatant Source;
    public Combatant Target;
    public int Amount;
}
