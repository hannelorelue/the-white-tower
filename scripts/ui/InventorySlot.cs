using Godot;

public partial class InventorySlot : Control
{
    [Export] private TextureRect _icon;
    [Export] private Label _quantity;
    [Export] private TextureRect _highlight;

    public void SetItem(ItemStack? stack)
    {
        if (stack == null || stack.IsEmpty)
        {
            _icon.Texture = null;
            _quantity.Visible = false;
            return;
        }

        _icon.Texture = stack.Data.Icon;
        _quantity.Visible = stack.Quantity > 1;
        _quantity.Text = stack.Quantity.ToString();
    }

    public void SetHighlighted(bool highlighted)
    {
        _highlight.Visible = highlighted;
    }
}
