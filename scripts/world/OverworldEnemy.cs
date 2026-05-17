using Godot;

public partial class OverworldEnemy : WorldCharacter
{
    [Export] public CombatantData Data { get; set; }
    [Export] private Area2D _sightArea;
    [Export] private string _facingDirection = "down";

    private bool _combatTriggered;

    public override void _Ready()
    {
        _sightArea.BodyEntered += OnBodyEnteredSight;
        _lastDirection = _facingDirection;
        UpdateAnimation(Vector2.Zero);
    }

    private void OnBodyEnteredSight(Node2D body)
    {
        if (_combatTriggered || body is not Player)
            return;

        _combatTriggered = true;
        GameManager.Instance.StartCombat(Data);
    }
}
