using Godot;

[GlobalClass]
public partial class CropData : Resource
{
    [Export] public string Name { get; set; } = "";
    [Export] public ItemData SeedItem { get; set; }
    [Export] public ItemData[] HarvestYield { get; set; } = [];
    [Export] public int[] HarvestQuantities { get; set; } = [];
    [Export] public int GrowthStages { get; set; } = 4;
    [Export] public bool Perennial { get; set; } = false;
    [Export] public Texture2D[] StageSprites { get; set; } = [];
    [Export] public Texture2D FruitingSprite { get; set; }
}
