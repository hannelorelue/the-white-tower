using Godot;

public partial class InventoryContainer : Node
{
    [Export] public int SlotCount { get; set; } = 24;

    private ItemStack?[] _slots;

    [Signal] public delegate void SlotChangedEventHandler(int slotIndex);

    public override void _Ready()
    {
        _slots = new ItemStack?[SlotCount];
    }

    public ItemStack? GetSlot(int index) => _slots[index];

    public bool TryAdd(ItemData data, int quantity = 1)
    {
        if (!HasSpace(data, quantity))
            return false;

        int remaining = quantity;

        for (int i = 0; i < SlotCount && remaining > 0; i++)
        {
            if (_slots[i] != null && _slots[i]!.Data == data)
            {
                remaining = _slots[i]!.Add(remaining);
                EmitSignal(SignalName.SlotChanged, i);
            }
        }

        for (int i = 0; i < SlotCount && remaining > 0; i++)
        {
            if (_slots[i] == null)
            {
                int toAdd = Mathf.Min(remaining, data.MaxStackSize);
                _slots[i] = new ItemStack(data, toAdd);
                remaining -= toAdd;
                EmitSignal(SignalName.SlotChanged, i);
            }
        }

        return true;
    }

    public bool TryRemove(ItemData data, int quantity = 1)
    {
        if (!HasItem(data, quantity))
            return false;

        int remaining = quantity;

        for (int i = 0; i < SlotCount && remaining > 0; i++)
        {
            if (_slots[i]?.Data == data)
            {
                int toRemove = Mathf.Min(remaining, _slots[i]!.Quantity);
                _slots[i]!.Remove(toRemove);
                remaining -= toRemove;

                if (_slots[i]!.IsEmpty)
                    _slots[i] = null;

                EmitSignal(SignalName.SlotChanged, i);
            }
        }

        return true;
    }

    public bool HasItem(ItemData data, int quantity = 1)
    {
        int total = 0;
        for (int i = 0; i < SlotCount; i++)
        {
            if (_slots[i]?.Data == data)
                total += _slots[i]!.Quantity;
        }
        return total >= quantity;
    }

    public bool HasSpace(ItemData data, int quantity = 1)
    {
        int space = 0;
        for (int i = 0; i < SlotCount; i++)
        {
            if (_slots[i] == null)
                space += data.MaxStackSize;
            else if (_slots[i]!.Data == data)
                space += data.MaxStackSize - _slots[i]!.Quantity;
        }
        return space >= quantity;
    }
}
