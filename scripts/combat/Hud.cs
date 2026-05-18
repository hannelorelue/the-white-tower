using Godot;
public partial class Hud : CanvasLayer
{
    [Export] private Champion _champion;
    [Export] private Combatant _enemy;

    [ExportGroup("Champion")]
    [Export] private Label _championName;
    [Export] private Label _championHp;

    [ExportGroup("Enemy")]
    [Export] private Label _enemyName;
    [Export] private Label _enemyHp;

    [ExportGroup("Actions")]
    [Export] private Label _actionPips;
    [Export] private Label _reactionPip;

    [ExportGroup("Log")]
    [Export] private RichTextLabel _combatLog;

    public override void _Ready()
    {
        CombatManager.Instance.MessageLogged += line => _combatLog.AppendText(line + "\n");

        _champion.HpChanged += (current, max) => _championHp.Text = $"{current} / {max}";
        _champion.ActionSpent += (remaining) => UpdateActionPips(remaining);
        _champion.TurnStarted += () => { UpdateActionPips(3); UpdateReactionPip(true); };
        _champion.ReactionSpent += () => UpdateReactionPip(false);
        _enemy.HpChanged += (current, max) => _enemyHp.Text = $"{current} / {max}";

        _championName.Text = _champion.Data.Name;
        _championHp.Text = $"{_champion.CurrentHp} / {_champion.Data.MaxHp}";
        _enemyName.Text = _enemy.Data.Name;
        _enemyHp.Text = $"{_enemy.CurrentHp} / {_enemy.Data.MaxHp}";
        UpdateActionPips(0);
        UpdateReactionPip(false);
    }

    private void UpdateActionPips(int actionsRemaining)
    {
        _actionPips.Text = actionsRemaining switch
        {
            3 => "● ● ●",
            2 => "● ● ○",
            1 => "● ○ ○",
            _ => "○ ○ ○"
        };
    }

    private void UpdateReactionPip(bool available)
    {
        _reactionPip.Text = "⚡";
        _reactionPip.Modulate = available ? Colors.White : Colors.DarkGray;
    }
}
