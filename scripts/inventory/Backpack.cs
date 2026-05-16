using Godot;

public partial class Backpack : InventoryContainer
{
    public const int HotbarSize = 8;

    public int ActiveHotbarSlot { get; private set; }
    public ItemStack? ActiveItem => GetSlot(ActiveHotbarSlot);

    [Signal] public delegate void ActiveSlotChangedEventHandler(int slotIndex);

    public override void _Ready()
    {
        SlotCount = 24;
        base._Ready();
    }

    public void SetActiveHotbarSlot(int index)
    {
        if (index < 0 || index >= HotbarSize)
            return;

        ActiveHotbarSlot = index;
        EmitSignal(SignalName.ActiveSlotChanged, index);
    }

    public ItemStack?[] GetHotbarSlots()
    {
        var slots = new ItemStack?[HotbarSize];
        for (int i = 0; i < HotbarSize; i++)
            slots[i] = GetSlot(i);
        return slots;
    }
}
