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

**Scene / Script structure**
Scripts mirror the `scenes/` tree under `scripts/`. A scene at `scenes/combat/BattleArena.tscn` has its script at `scripts/combat/BattleArena.cs`. Never co-locate `.cs` files next to `.tscn` files.

**Node access**
Use `[Export]` for all node references. Never use hardcoded `GetNode("../../...")` paths.

**Cross-system communication**
Systems must not call each other directly. Use Godot signals or a thin event-bus autoload to decouple subsystems (farming, combat, inventory, etc.).

**Data**
Item stats, crop growth, enemy definitions, etc. must be `[GlobalClass]` `Resource` subclasses saved as `.tres` files. No hardcoded data in scripts, no plain `Dictionary` blobs.

**Autoloads**
Autoloads (`GameManager`, `SaveSystem`, etc.) coordinate — they do not contain game logic. Keep them thin.

### Naming

* Classes and scene files: `PascalCase`
* Methods: `PascalCase` (C# standard)
* Signals: `PascalCase` past tense (`TurnEnded`, `ItemPickedUp`)
* Private fields: `_camelCase`

**Save system**
Each major system implements `GetSaveData()` / `LoadSaveData()` returning a typed save-data struct. Decide the save format before systems multiply.

## Agent behavior

* Do not commit by yourself
