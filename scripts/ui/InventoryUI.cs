using Godot;

public partial class InventoryUI : CanvasLayer
{
    private const string SlotScenePath = "res://scenes/ui/inventory_slot.tscn";

    [Export] private Control _backpackPanel;
    [Export] private HBoxContainer _hotbarContainer;
    [Export] private GridContainer _backpackGrid;

    private Backpack _backpack;
    private InventorySlot[] _hotbarSlots;
    private InventorySlot[] _backpackSlots;

    public override void _Ready()
    {
        _backpack = GameManager.Instance.PlayerBackpack;

        BuildSlots();

        _backpack.SlotChanged += UpdateSlot;
        _backpack.ActiveSlotChanged += UpdateHighlight;

        _backpackPanel.Visible = false;

        for (int i = 0; i < _backpack.SlotCount; i++)
            UpdateSlot(i);

        UpdateHighlight(0);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventKey key && key.Pressed && !key.Echo)
        {
            if (key.Keycode == Key.E)
            {
                _backpackPanel.Visible = !_backpackPanel.Visible;
                return;
            }

            int hotbarIndex = (int)key.Keycode - (int)Key.Key1;
            if (hotbarIndex >= 0 && hotbarIndex < Backpack.HotbarSize)
                _backpack.SetActiveHotbarSlot(hotbarIndex);
        }

        if (@event is InputEventMouseButton mouse && mouse.Pressed)
        {
            if (mouse.ButtonIndex == MouseButton.WheelUp)
                _backpack.SetActiveHotbarSlot((_backpack.ActiveHotbarSlot - 1 + Backpack.HotbarSize) % Backpack.HotbarSize);
            else if (mouse.ButtonIndex == MouseButton.WheelDown)
                _backpack.SetActiveHotbarSlot((_backpack.ActiveHotbarSlot + 1) % Backpack.HotbarSize);
        }
    }

    private void BuildSlots()
    {
        var slotScene = GD.Load<PackedScene>(SlotScenePath);

        _hotbarSlots = new InventorySlot[Backpack.HotbarSize];
        for (int i = 0; i < Backpack.HotbarSize; i++)
        {
            var slot = slotScene.Instantiate<InventorySlot>();
            _hotbarContainer.AddChild(slot);
            _hotbarSlots[i] = slot;
        }

        _backpackSlots = new InventorySlot[_backpack.SlotCount];
        for (int i = 0; i < _backpack.SlotCount; i++)
        {
            var slot = slotScene.Instantiate<InventorySlot>();
            _backpackGrid.AddChild(slot);
            _backpackSlots[i] = slot;
        }
    }

    private void UpdateSlot(int slotIndex)
    {
        var stack = _backpack.GetSlot(slotIndex);

        if (slotIndex < Backpack.HotbarSize)
            _hotbarSlots[slotIndex].SetItem(stack);

        _backpackSlots[slotIndex].SetItem(stack);
    }

    private void UpdateHighlight(int activeIndex)
    {
        for (int i = 0; i < Backpack.HotbarSize; i++)
            _hotbarSlots[i].SetHighlighted(i == activeIndex);
    }
}
