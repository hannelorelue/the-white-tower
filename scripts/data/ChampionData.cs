using Godot;

[GlobalClass]
public partial class ChampionData : CombatantData
{
    [Export] public int ShieldHardness { get; set; }
    [Export] public int ShieldMaxHp { get; set; }
}
