using Godot;

public partial class Player : WorldCharacter
{
    [Export] private float _speed = 100f;
    [Export] private Area2D _interactionArea;

    private IInteractable _currentInteractable;

    public override void _Ready()
    {
        _interactionArea.BodyEntered += OnBodyEntered;
        _interactionArea.BodyExited += OnBodyExited;
    }

    public override void _PhysicsProcess(double delta)
    {
        var input = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
        Velocity = input * _speed;
        MoveAndSlide();
        UpdateAnimation(input);
    }

    private void UpdateAnimation(Vector2 input)
    {
        if (input == Vector2.Zero)
        {
            _sprite.Play($"idle_{(_lastDirection == "left" ? "right" : _lastDirection)}");
            return;
        }

        if (Mathf.Abs(input.X) >= Mathf.Abs(input.Y))
        {
            _lastDirection = input.X > 0 ? "right" : "left";
            _sprite.FlipH = input.X < 0;
            _sprite.Play("walk_right");
        }
        else
        {
            _lastDirection = input.Y > 0 ? "down" : "up";
            _sprite.FlipH = false;
            _sprite.Play($"walk_{_lastDirection}");
        }
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
