using Godot;
using System.Collections.Generic;

public partial class FarmingManager : Node
{
    [Export] private TileMapLayer _groundLayer;
    [Export] private Player _player;
    [Export] private Node2D _spriteContainer;
    [Export] private CropData[] _cropRegistry = [];
    [Export] private ItemData _wateringCan;

    private readonly Dictionary<Vector2I, CropState> _crops = new();

    public override void _UnhandledInput(InputEvent @event)
    {
        if (!@event.IsActionPressed("ui_accept"))
            return;

        var coords = GetPlayerTile();
        if (!IsFarmland(coords))
            return;

        var backpack = GameManager.Instance.PlayerBackpack;
        var heldItem = backpack.GetSlot(backpack.ActiveHotbarSlot)?.Data;

        if (_crops.TryGetValue(coords, out var state))
        {
            if (heldItem == _wateringCan && !state.Watered)
            {
                state.Watered = true;
                GetViewport().SetInputAsHandled();
            }
            else if (IsHarvestable(state))
            {
                Harvest(coords, state, backpack);
                GetViewport().SetInputAsHandled();
            }
        }
        else
        {
            var crop = FindCropForSeed(heldItem);
            if (crop != null)
            {
                Plant(coords, crop, backpack);
                GetViewport().SetInputAsHandled();
            }
        }
    }

    public void AdvanceDay()
    {
        foreach (var (_, state) in _crops)
        {
            if (state.Stage < state.Crop.GrowthStages)
            {
                if (state.Watered)
                    state.Stage++;
                state.Watered = false;
            }
            else if (state.Crop.Perennial)
                state.Fruiting = true;

            UpdateSprite(state);
        }
    }

    private void Plant(Vector2I coords, CropData crop, Backpack backpack)
    {
        backpack.TryRemove(crop.SeedItem, 1);

        var sprite = new Sprite2D();
        _spriteContainer.AddChild(sprite);
        sprite.GlobalPosition = _groundLayer.ToGlobal(_groundLayer.MapToLocal(coords));

        var state = new CropState { Crop = crop, Stage = 0, Sprite = sprite };
        _crops[coords] = state;
        UpdateSprite(state);
    }

    private void Harvest(Vector2I coords, CropState state, Backpack backpack)
    {
        for (int i = 0; i < state.Crop.HarvestYield.Length; i++)
            backpack.TryAdd(state.Crop.HarvestYield[i], state.Crop.HarvestQuantities[i]);

        if (state.Crop.Perennial)
        {
            state.Fruiting = false;
            UpdateSprite(state);
        }
        else
        {
            state.Sprite.QueueFree();
            _crops.Remove(coords);
        }
    }

    private void UpdateSprite(CropState state)
    {
        state.Sprite.Texture = state.Stage >= state.Crop.GrowthStages && state.Fruiting
            ? state.Crop.FruitingSprite
            : state.Crop.StageSprites[Mathf.Min(state.Stage, state.Crop.StageSprites.Length - 1)];
    }

    private static bool IsHarvestable(CropState state) =>
        state.Stage >= state.Crop.GrowthStages && (!state.Crop.Perennial || state.Fruiting);

    private CropData FindCropForSeed(ItemData seed)
    {
        if (seed == null) return null;
        foreach (var crop in _cropRegistry)
            if (crop.SeedItem == seed) return crop;
        return null;
    }

    private Vector2I GetPlayerTile() =>
        _groundLayer.LocalToMap(_groundLayer.ToLocal(_player.GlobalPosition));

    private bool IsFarmland(Vector2I coords) =>
        _groundLayer.GetCellTileData(coords)?.GetCustomData("is_farmland").AsBool() ?? false;
}
