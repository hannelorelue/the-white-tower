using Godot;

public partial class Chest : InventoryContainer
{
    public override void _Ready()
    {
        SlotCount = 48;
        base._Ready();
    }
}
