using Godot;

public partial class OverworldEnemy : CharacterBody2D
{
    [Export] public CombatantData Data { get; set; }
    [Export] private Area2D _sightArea;

    private bool _combatTriggered;

    public override void _Ready()
    {
        _sightArea.BodyEntered += OnBodyEnteredSight;
    }

    private void OnBodyEnteredSight(Node2D body)
    {
        if (_combatTriggered || body is not Player)
            return;

        _combatTriggered = true;
        GameManager.Instance.StartCombat(Data);
    }
}
