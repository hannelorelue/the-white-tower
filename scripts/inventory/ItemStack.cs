using System;
public class ItemStack
{
    public ItemData Data { get; }
    public int Quantity { get; private set; }

    public bool IsEmpty => Quantity <= 0;

    public ItemStack(ItemData data, int quantity = 1)
    {
        Data = data;
        Quantity = quantity;
    }

    // Adds as much as possible, returns leftover amount.
    public int Add(int amount)
    {
        int canAdd = Math.Min(Data.MaxStackSize - Quantity, amount);
        Quantity += canAdd;
        return amount - canAdd;
    }

    public bool Remove(int amount)
    {
        if (Quantity < amount)
            return false;
        Quantity -= amount;
        return true;
    }
}
