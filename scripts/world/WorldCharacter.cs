using Godot;

public partial class WorldCharacter : CharacterBody2D
{
    [Export] protected AnimatedSprite2D _sprite;

    protected string _lastDirection = "down";

    protected void UpdateAnimation(Vector2 input)
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
}
