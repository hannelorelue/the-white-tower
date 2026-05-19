public class CombatAction
{
    public string Name { get; init; }
    public int ActionCost { get; init; }
    public string DamagePreview { get; init; } = "—";
    public System.Action Execute { get; init; }
}
