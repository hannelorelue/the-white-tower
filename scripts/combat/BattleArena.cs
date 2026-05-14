using Godot;
using System.Collections.Generic;

public partial class BattleArena : Node2D
{
    [Export] private Champion _champion;
    [Export] private Combatant _enemy;

    public override void _Ready()
    {
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
            GD.Print($"Strike: {result.Outcome} | Roll: {result.Roll} | Total: {result.TotalToHit} vs AC {_enemy.Data.ArmorClass} | Damage: {result.DamageDealt}");
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
            GD.Print($"{enemy.Data.Name} strikes: {result.Outcome} | Roll: {result.Roll} | Total: {result.TotalToHit} vs AC {_champion.Data.ArmorClass} | Damage: {result.DamageDealt}");
        }

        CombatManager.Instance.AdvanceTurn();
    }

    private void OnCombatEnded(bool playerWon)
    {
        GD.Print(playerWon ? "Victory!" : "Defeated...");
        SetProcessUnhandledInput(false);
    }
}
