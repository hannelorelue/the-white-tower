using Godot;

public partial class CombatEndScreen : CanvasLayer
{
    [Export] private Label _resultLabel;
    [Export] private Label _promptLabel;

    private bool _waitingForInput;

    public override void _Ready()
    {
        Visible = false;
    }

    public void Show(bool playerWon)
    {
        _resultLabel.Text = playerWon ? "Victory!" : "Defeated...";
        Visible = true;
        _waitingForInput = true;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (!_waitingForInput)
            return;

        if (@event is InputEventKey key && key.Pressed && !key.Echo)
        {
            _waitingForInput = false;
            GameManager.Instance.EndCombat();
        }
    }
}
