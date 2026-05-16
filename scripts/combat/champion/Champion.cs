using Godot;

public partial class Champion : Combatant
{
    public int ShieldCurrentHp { get; private set; }
    public bool IsShieldRaised { get; private set; }

    [Signal] public delegate void ShieldRaisedEventHandler();
    [Signal] public delegate void ShieldLoweredEventHandler();
    [Signal] public delegate void ShieldHpChangedEventHandler(int current, int max);

    public override void _Ready()
    {
        base._Ready();
        ShieldCurrentHp = ((ChampionData)Data).ShieldMaxHp;
        Reactions.Add(new ShieldBlock());
        Reactions.Add(new RetributiveStrike());
    }

    public void RaiseShield()
    {
        if (!SpendAction())
            return;

        IsShieldRaised = true;
        AcBonus = 2;
        EmitSignal(SignalName.ShieldRaised);
    }

    public void LowerShield()
    {
        IsShieldRaised = false;
        AcBonus = 0;
        EmitSignal(SignalName.ShieldLowered);
    }

    // Returns the damage that passes through after hardness and shield absorption.
    public int BlockWithShield(int incomingDamage)
    {
        var data = (ChampionData)Data;
        int absorbed = Mathf.Min(ShieldCurrentHp, Mathf.Max(0, incomingDamage - data.ShieldHardness));
        ShieldCurrentHp -= absorbed;
        EmitSignal(SignalName.ShieldHpChanged, ShieldCurrentHp, data.ShieldMaxHp);

        return Mathf.Max(0, incomingDamage - data.ShieldHardness - absorbed);
    }

    public new void ResetForNewTurn()
    {
        LowerShield();
        base.ResetForNewTurn();
    }
}
