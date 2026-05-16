using Godot;
using System.Collections.Generic;

public partial class BattleArena : Node2D
{
    private const string ChampionScenePath = "res://scenes/combat/champion.tscn";
    private const string CombatantScenePath = "res://scenes/combat/combatant.tscn";

    [Export] private Champion _champion;
    [Export] private Combatant _enemy;

    public override void _Ready()
    {
        _champion ??= CreateCombatant<Champion>(ChampionScenePath, GameManager.Instance?.PlayerData);
        _enemy ??= CreateCombatant<Combatant>(CombatantScenePath, GameManager.Instance?.PendingEnemyData);

        CombatManager.Instance.TurnChanged += OnTurnChanged;
        CombatManager.Instance.CombatEnded += OnCombatEnded;

        CombatManager.Instance.StartCombat(new List<Combatant> { _champion, _enemy });
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (CombatManager.Instance.ActiveCombatant is not Champion)
            return;

        if (@event.IsActionPressed("ui_accept"))
        {
            CombatManager.Instance.AdvanceTurn();
        }
        else if (@event.IsActionPressed("ui_select"))
        {
            var result = StrikeAction.Execute(_champion, _enemy);
            GD.Print($"Strike: {result.Outcome} | Roll: {result.Roll} | Total: {result.TotalToHit} vs AC {_enemy.Data.ArmorClass + _enemy.AcBonus} | Damage: {result.DamageDealt}");
        }
        else if (@event.IsActionPressed("ui_up"))
        {
            _champion.RaiseShield();
            GD.Print($"Shield raised — AC is now {_champion.Data.ArmorClass + _champion.AcBonus}");
        }
    }

    private void OnTurnChanged(Combatant combatant)
    {
        GD.Print($"--- {combatant.Data.Name}'s turn ({combatant.ActionsRemaining} actions) ---");

        if (combatant is Champion)
            return;

        RunEnemyTurn(combatant);
    }

    private void RunEnemyTurn(Combatant enemy)
    {
        while (enemy.ActionsRemaining > 0 && _champion.CurrentHp > 0)
        {
            var result = StrikeAction.Execute(enemy, _champion);
            GD.Print($"{enemy.Data.Name} strikes: {result.Outcome} | Roll: {result.Roll} | Total: {result.TotalToHit} vs AC {_champion.Data.ArmorClass + _champion.AcBonus} | Damage: {result.DamageDealt}");
        }

        CombatManager.Instance.AdvanceTurn();
    }

    private void OnCombatEnded(bool playerWon)
    {
        GD.Print(playerWon ? "Victory!" : "Defeated...");
        SetProcessUnhandledInput(false);
        GameManager.Instance?.EndCombat();
    }

    private static T CreateCombatant<T>(string scenePath, CombatantData data) where T : Combatant
    {
        var scene = GD.Load<PackedScene>(scenePath);
        var combatant = scene.Instantiate<T>();
        combatant.Data = data;
        return combatant;
    }
}
