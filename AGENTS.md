# Agents

## Project Goal

The goal of the project is to develop a retro pixel art rpg game with includes the following mechanics:

* RPG with turn based combat
* Farming
* Foraging
* Planting Trees that bear fruit
* crafting
* inventory
* serialize and deserialize the game state

## Technologies

* Godot 4
* C#
* Combat based on Pathfinder 2 the only player Class is Champion

## Project Structure

```text
res://
├── addons/               # Third-party plugins
├── assets/
│   ├── audio/music/
│   ├── audio/sfx/
│   ├── fonts/
│   └── sprites/          # characters, environment, items, ui
├── scenes/
│   ├── combat/
│   ├── farming/
│   ├── world/
│   ├── ui/
│   └── autoloads/        # Singletons (GameManager, SaveSystem, etc.)
├── scripts/
│   ├── combat/
│   │   ├── actions/      # PF2e actions (Strike, Raise Shield, etc.)
│   │   ├── conditions/   # Frightened, Grabbed, etc.
│   │   └── champion/     # Champion feats and reactions
│   ├── farming/
│   ├── foraging/
│   ├── crafting/
│   ├── inventory/
│   ├── data/             # Resource definitions and loaders
│   └── save/             # Serialization / deserialization
├── resources/            # .tres data files (items, crops, enemies)
└── tests/
```

## Coding Conventions

### Scene / Script structure

Scripts mirror the `scenes/` tree under `scripts/`. A scene at `scenes/combat/BattleArena.tscn` has its script at `scripts/combat/BattleArena.cs`. Never co-locate `.cs` files next to `.tscn` files.

### Node access

Use `[Export]` for all node references. Never use hardcoded `GetNode("../../...")` paths.

### Cross-system communication

Systems must not call each other directly. Use Godot signals or a thin event-bus autoload to decouple subsystems (farming, combat, inventory, etc.).

### Data

Item stats, crop growth, enemy definitions, etc. must be `[GlobalClass]` `Resource` subclasses saved as `.tres` files. No hardcoded data in scripts, no plain `Dictionary` blobs.

### Autoloads

Autoloads (`GameManager`, `SaveSystem`, etc.) coordinate — they do not contain game logic. Keep them thin.

### Naming

* Classes and scene files: `PascalCase`
* Methods: `PascalCase` (C# standard)
* Signals: `PascalCase` past tense (`TurnEnded`, `ItemPickedUp`)
* Private fields: `_camelCase`

### Save system

Each major system implements `GetSaveData()` / `LoadSaveData()` returning a typed save-data struct. Decide the save format before systems multiply.

## Agent behavior

* Do not commit by yourself

## Current State

### Combat system — complete (MVP)

**Scripts:** `scripts/combat/`, `scripts/data/`

Key classes:

* `CombatantData` / `ChampionData` — `[GlobalClass]` Resources defining stats (HP, AC, attack bonus, damage dice, Perception, speed, shield)
* `Combatant` (Node2D) — runtime state: CurrentHp, ActionsRemaining, ReactionAvailable, MultipleAttackPenalty, AcBonus. Holds `List<Reaction>`
* `Champion : Combatant` — adds shield state (IsShieldRaised, ShieldCurrentHp). Registers RetributiveStrike and ShieldBlock reactions on Ready
* `CombatManager` (autoload) — owns turn order, broadcasts `ReactionTrigger` events, emits `TurnChanged` / `CombatEnded`
* `StrikeAction` — static class, executes PF2e Strike (d20 + AttackBonus + MAP + situationalBonus vs AC + AcBonus). Crits double dice count, not total
* `Reaction` / `ReactionTrigger` — generic reaction system. `BeforeDamageTaken` triggers reduce damage before it is applied (ShieldBlock); `DamageTaken` triggers fire after (RetributiveStrike)
* `Dice` — static utility at `scripts/Dice.cs`

PF2e rules in scope: Strike, multiple-attack penalty (0 / -5 / -10), Raise Shield (+2 AC), Shield Block (hardness reduces damage), Retributive Strike (reaction, -2 to hit).

PF2e rules not yet implemented: conditions, saving throws, dying state, feats, spells.

Test scene: `scenes/combat/BattleArena.tscn`. Controls: Space = Strike, ↑ = Raise Shield, Enter = End Turn.

---

### Inventory system — scaffolded, not yet wired to UI or player

**Scripts:** `scripts/inventory/`

Key classes:

* `ItemData` — `[GlobalClass]` Resource: Name, Icon, MaxStackSize (1 for tools, >1 for stackables)
* `ItemStack` — plain C# class: holds ItemData reference + Quantity. `Add()` returns leftover
* `InventoryContainer` (Node) — base class with slot array, `TryAdd` / `TryRemove` / `HasItem` / `HasSpace`. `TryAdd` is all-or-nothing (checks space before committing). Emits `SlotChanged(int)`
* `Backpack : InventoryContainer` — 24 slots. Hotbar = slots 0–7 (no item type restriction). Tracks `ActiveHotbarSlot`. Emits `ActiveSlotChanged(int)`
* `Chest : InventoryContainer` — 48 slots, stationary

Design rules:

* Hotbar is a window into the backpack (slots 0–7), not a separate container
* Full backpack blocks pickup — `TryAdd` returns false, caller handles the failure
* Crafting pulls from backpack only, not chests

Not yet built: inventory UI, item `.tres` files, player pickup logic, chest placement in world.
