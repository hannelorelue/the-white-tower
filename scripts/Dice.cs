using Godot;

public static class Dice
{
    public static int Roll(int sides) => (int)GD.RandRange(1, sides);

    public static int Roll(int count, int sides)
    {
        int total = 0;
        for (int i = 0; i < count; i++)
            total += Roll(sides);
        return total;
    }
}
