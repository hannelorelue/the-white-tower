using Godot;

public partial class ActionMenuEntry : Control
{
    [Export] private Label _nameLabel;
    [Export] private Label _costLabel;
    [Export] private Label _damageLabel;
    [Export] private ColorRect _highlight;

    public void Setup(CombatAction action)
    {
        _nameLabel.Text = action.Name;
        _costLabel.Text = action.ActionCost switch
        {
            1 => "●",
            2 => "● ●",
            3 => "● ● ●",
            _ => "—"
        };
        _damageLabel.Text = action.DamagePreview;
    }

    public void SetHighlighted(bool highlighted)
    {
        _highlight.Visible = highlighted;
    }
}
