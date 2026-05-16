using Godot;

public partial class Player : CharacterBody2D
{
    [Export] private float _speed = 100f;
    [Export] private Backpack _backpack;
    [Export] private Area2D _interactionArea;

    private IInteractable _currentInteractable;

    public override void _Ready()
    {
        GameManager.Instance.RegisterBackpack(_backpack);

        _interactionArea.BodyEntered += OnBodyEntered;
        _interactionArea.BodyExited += OnBodyExited;
    }

    public override void _PhysicsProcess(double delta)
    {
        var input = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
        Velocity = input * _speed;
        MoveAndSlide();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("ui_accept"))
            _currentInteractable?.Interact(this);
    }

    private void OnBodyEntered(Node2D body)
    {
        if (body is IInteractable interactable)
            _currentInteractable = interactable;
    }

    private void OnBodyExited(Node2D body)
    {
        if (body == _currentInteractable as Node2D)
            _currentInteractable = null;
    }
}
