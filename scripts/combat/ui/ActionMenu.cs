using Godot;
using System.Collections.Generic;

public partial class ActionMenu : Control
{
    private const string EntryScenePath = "res://scenes/combat/ui/action_menu_entry.tscn";

    [Export] private VBoxContainer _container;

    private readonly List<CombatAction> _actions = new();
    private readonly List<ActionMenuEntry> _entries = new();
    private int _selectedIndex;

    public void Populate(List<CombatAction> actions)
    {
        foreach (var entry in _entries)
            entry.QueueFree();
        _entries.Clear();
        _actions.Clear();

        var entryScene = GD.Load<PackedScene>(EntryScenePath);
        foreach (var action in actions)
        {
            var entry = entryScene.Instantiate<ActionMenuEntry>();
            _container.AddChild(entry);
            entry.Setup(action);
            _entries.Add(entry);
            _actions.Add(action);
        }

        _selectedIndex = 0;
        UpdateHighlight();
        Visible = true;
        GD.Print($"ActionMenu populated with {_entries.Count} entries, container={_container}, visible={Visible}");
    }

    public void Hide()
    {
        Visible = false;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (!Visible || _actions.Count == 0)
            return;

        if (@event.IsActionPressed("ui_up"))
        {
            _selectedIndex = (_selectedIndex - 1 + _actions.Count) % _actions.Count;
            UpdateHighlight();
            GetViewport().SetInputAsHandled();
        }
        else if (@event.IsActionPressed("ui_down"))
        {
            _selectedIndex = (_selectedIndex + 1) % _actions.Count;
            UpdateHighlight();
            GetViewport().SetInputAsHandled();
        }
        else if (@event.IsActionPressed("ui_accept"))
        {
            _actions[_selectedIndex].Execute();
            GetViewport().SetInputAsHandled();
        }
    }

    private void UpdateHighlight()
    {
        for (int i = 0; i < _entries.Count; i++)
            _entries[i].SetHighlighted(i == _selectedIndex);
    }
}
