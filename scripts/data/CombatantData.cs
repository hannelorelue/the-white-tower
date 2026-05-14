using Godot;

[GlobalClass]
public partial class CombatantData : Resource
{
    [Export] public string Name { get; set; } = "";
    [Export] public int MaxHp { get; set; }
    [Export] public int ArmorClass { get; set; }
    [Export] public int AttackBonus { get; set; }
    [Export] public int DamageDiceCount { get; set; } = 1;
    [Export] public int DamageDie { get; set; } = 6;
    [Export] public int DamageBonus { get; set; }
    [Export] public int Perception { get; set; }
    [Export] public int SpeedFeet { get; set; } = 25;
}
