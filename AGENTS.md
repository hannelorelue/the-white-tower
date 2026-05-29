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

* Classes: `PascalCase`
* Scene and resource files: `snake_case` (Godot default — `battle_arena.tscn`, `goblin.tres`)
* Methods: `PascalCase` (C# standard)
* Signals: `PascalCase` past tense (`TurnEnded`, `ItemPickedUp`)
* Private fields: `_camelCase`

Scene file paths hardcoded in scripts (e.g. `const string ScenePath = "res://..."`) must use `snake_case` to match the actual file names Godot creates.

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
* `ActionMenu` (NinePatchRect) — keyboard-driven action menu. `Populate(List<CombatAction>)` / `Hide()`. Entries are instantiated `ActionMenuEntry` children of its inner VBoxContainer
* `Dice` — static utility at `scripts/Dice.cs`

PF2e rules in scope: Strike, multiple-attack penalty (0 / -5 / -10), Raise Shield (+2 AC), Shield Block (hardness reduces damage), Retributive Strike (reaction, -2 to hit).

PF2e rules not yet implemented: conditions, saving throws, dying state, feats, spells.

Scene: `scenes/combat/battle_arena.tscn`.

---

### World system — complete (MVP)

**Scripts:** `scripts/world/`, `scripts/autoloads/GameManager.cs`

Key classes:

* `WorldCharacter : CharacterBody2D` — base for animated world characters. `UpdateAnimation(Vector2)` drives 4-directional walk/idle sprites via `AnimatedSprite2D`
* `Player : WorldCharacter` — WASD/arrow movement, `Area2D` interaction zone. Press confirm to call `IInteractable.Interact(player)` on the nearest interactable
* `OverworldEnemy : WorldCharacter` — stationary enemy with `Area2D` sight zone. Triggers `GameManager.StartCombat(Data)` when player enters
* `ForageTree : StaticBody2D, IInteractable` — exports `DropItem` (ItemData) and `DropQuantity`. On interact: calls `backpack.TryAdd()`; logs success or "Backpack full"
* `IInteractable` — interface: `void Interact(Player player)`
* `GameManager` (autoload) — holds `PlayerData`, `PlayerBackpack`, `PendingEnemyData`. `StartCombat()` stores return scene path and switches to `battle_arena.tscn`; `EndCombat()` returns to it

Scene: `scenes/world/region_01.tscn`.

---

### Inventory system — complete (MVP)

**Scripts:** `scripts/inventory/`, `scripts/ui/`

Key classes:

* `ItemData` — `[GlobalClass]` Resource: Name, Icon, MaxStackSize (1 for tools, >1 for stackables)
* `ItemStack` — plain C# class: holds ItemData reference + Quantity. `Add()` returns leftover
* `InventoryContainer` (Node) — base class with slot array, `TryAdd` / `TryRemove` / `HasItem` / `HasSpace`. `TryAdd` is all-or-nothing. Emits `SlotChanged(int)`
* `Backpack : InventoryContainer` — 24 slots. Hotbar = slots 0–7. Tracks `ActiveHotbarSlot`. Emits `ActiveSlotChanged(int)`
* `Chest : InventoryContainer` — 48 slots, stationary
* `InventoryUI : CanvasLayer` — hotbar always visible; backpack grid toggled with E. 1–8 selects hotbar slot; scroll wheel cycles it. Listens to `SlotChanged` / `ActiveSlotChanged`
* `InventorySlot` — single slot visual (icon + quantity label + highlight)

Design rules:

* Hotbar is a window into the backpack (slots 0–7), not a separate container
* Full backpack blocks pickup — `TryAdd` returns false, caller handles the failure
* Crafting pulls from backpack only, not chests

Item resources defined: `resources/items/wood.tres`, `resources/items/watering_can.tres`, `resources/crops/peach.tres`, `resources/crops/peach_sapling.tres`.

`GameManager._Ready()` seeds the player backpack with 3 peach saplings and 1 watering can on startup (temporary, until a proper starting-inventory system exists).

Not yet built: chest placement in world, additional item `.tres` files.

---

### Farming system — complete (MVP)

**Scripts:** `scripts/farming/`, `scripts/data/CropData.cs`

Key classes:

* `CropData` — `[GlobalClass]` Resource: `SeedItem` (ItemData), `HarvestYield` (ItemData[]), `HarvestQuantities` (int[]), `GrowthStages` (int), `Perennial` (bool), `StageSprites` (Texture2D[]), `FruitingSprite` (Texture2D). Perennial crops stay at max stage and re-fruit each day instead of being removed on harvest
* `CropState` — plain C# class (not a Node): runtime state per planted tile. Fields: `Crop`, `Stage`, `Fruiting`, `Watered`, `Sprite` (Sprite2D spawned at tile world position)
* `FarmingManager` (Node) — lives in the region scene. Holds `Dictionary<Vector2I, CropState>`. Exports: `_groundLayer` (TileMapLayer), `_player`, `_spriteContainer` (Node2D for crop sprites), `_cropRegistry` (CropData[]), `_wateringCan` (ItemData), `_notification` (Label)

Farmland tiles are defined via a custom data layer `is_farmland` (bool) on the Ground TileMapLayer's TileSet. `FarmingManager` checks this on interact.

**Input (ui_accept on a farmland tile):**

* No crop + seed in hand → plant, consume 1 seed
* Crop present + watering can in hand → water (required for growth)
* Crop present + mature + fruiting → harvest, add yield to backpack

**`AdvanceDay()`:** advances stage on watered crops, resets `Watered`, sets `Fruiting = true` on perennials at max stage. Currently triggered by Escape key (temporary — no day system yet).

**Sprite logic:**

* `stage < GrowthStages` → `StageSprites[stage]`
* `stage == GrowthStages`, `Fruiting = true` → `FruitingSprite`
* `stage == GrowthStages`, `Fruiting = false` → `StageSprites[last]` (bare mature tree after harvest)

Resources defined: `resources/crops/peach_crop.tres` (CropData, 3 growth stages, perennial, peach sapling as seed), `resources/crops/peach_sapling.tres` (ItemData, max stack 3).

Not yet built: day/time system to drive `AdvanceDay()`, watering visual on tile, save/load of crop state.

---

### Crafting system — not yet built

`scripts/crafting/` exists but is empty.

---

### Save system — not yet built

`scripts/save/` exists but is empty.
