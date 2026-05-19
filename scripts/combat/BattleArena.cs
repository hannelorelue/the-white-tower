using Godot;
using System.Collections.Generic;

public partial class BattleArena : Node2D
{
    private const string ChampionScenePath = "res://scenes/combat/champion.tscn";
    private const string CombatantScenePath = "res://scenes/combat/combatant.tscn";

    [Export] private Champion _champion;
    [Export] private Combatant _enemy;
    [Export] private CombatEndScreen _endScreen;
    [Export] private ActionMenu _actionMenu;

    public override void _Ready()
    {
        _champion ??= CreateCombatant<Champion>(ChampionScenePath, GameManager.Instance?.PlayerData);
        _enemy ??= CreateCombatant<Combatant>(CombatantScenePath, GameManager.Instance?.PendingEnemyData);

        CombatManager.Instance.TurnChanged += OnTurnChanged;
        CombatManager.Instance.CombatEnded += OnCombatEnded;

        CombatManager.Instance.StartCombat(new List<Combatant> { _champion, _enemy });
    }

    private void OnTurnChanged(Combatant combatant)
    {
        CombatManager.Instance.Log($"— {combatant.Data.Name}'s turn —");

        if (combatant is Champion)
        {
            _actionMenu.Populate(BuildChampionActions());
            return;
        }

        _actionMenu.Hide();
        RunEnemyTurn(combatant);
    }

    private List<CombatAction> BuildChampionActions()
    {
        return new List<CombatAction>
        {
            new()
            {
                Name = "Strike",
                ActionCost = 1,
                DamagePreview = FormatDamagePreview(_champion.Data),
                Execute = () =>
                {
                    var result = StrikeAction.Execute(_champion, _enemy);
                    CombatManager.Instance.Log(FormatStrike("You", _enemy.Data.Name, result));
                }
            },
            new()
            {
                Name = "Raise Shield",
                ActionCost = 1,
                DamagePreview = "+2 AC",
                Execute = () =>
                {
                    _champion.RaiseShield();
                    CombatManager.Instance.Log($"Shield raised. (AC {_champion.Data.ArmorClass + _champion.AcBonus})");
                }
            },
            new()
            {
                Name = "End Turn",
                ActionCost = 0,
                Execute = () => CombatManager.Instance.AdvanceTurn()
            }
        };
    }

    private void RunEnemyTurn(Combatant enemy)
    {
        while (enemy.ActionsRemaining > 0 && _champion.CurrentHp > 0)
        {
            var result = StrikeAction.Execute(enemy, _champion);
            CombatManager.Instance.Log(FormatStrike(enemy.Data.Name, "you", result));
        }

        CombatManager.Instance.AdvanceTurn();
    }

    private void OnCombatEnded(bool playerWon)
    {
        _actionMenu.Hide();
        _endScreen.Show(playerWon);
    }

    private static string FormatDamagePreview(CombatantData data)
    {
        string dice = $"{data.DamageDiceCount}d{data.DamageDie}";
        return data.DamageBonus != 0 ? $"{dice}+{data.DamageBonus}" : dice;
    }

    private static string FormatStrike(string attackerName, string targetName, StrikeResult result)
    {
        string outcomeText = result.Outcome switch
        {
            StrikeOutcome.CriticalHit  => $"Critical Hit — {result.DamageDealt} dmg",
            StrikeOutcome.Hit          => $"Hit — {result.DamageDealt} dmg",
            StrikeOutcome.Miss         => "Miss",
            StrikeOutcome.CriticalMiss => "Critical Miss!",
            _                          => result.Outcome.ToString()
        };
        return $"{attackerName} → {targetName}: {outcomeText} (roll {result.Roll})";
    }

    private static T CreateCombatant<T>(string scenePath, CombatantData data) where T : Combatant
    {
        var scene = GD.Load<PackedScene>(scenePath);
        var combatant = scene.Instantiate<T>();
        combatant.Data = data;
        return combatant;
    }
}
