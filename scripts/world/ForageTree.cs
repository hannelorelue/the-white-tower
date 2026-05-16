using Godot;

public partial class ForageTree : StaticBody2D, IInteractable
{
    [Export] public ItemData DropItem { get; set; }
    [Export] public int DropQuantity { get; set; } = 1;

    public void Interact(Player player)
    {
        var backpack = GameManager.Instance.PlayerBackpack;
        if (backpack == null)
            return;

        if (backpack.TryAdd(DropItem, DropQuantity))
            GD.Print($"Foraged {DropQuantity}x {DropItem.Name}");
        else
            GD.Print("Backpack is full.");
    }
}
