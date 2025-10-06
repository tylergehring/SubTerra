# Tyler Gehring's Code Directory

This folder holds Tyler’s gameplay systems, centered around the new **non-reusable tool** pipeline (mushroom consumables, future TNT, etc.).

## Overview
- `NonReusableTools` – Base MonoBehaviour for pick-up-and-use tools. Handles inventory integration, logging, and optional test spawning.
- `Food` – Shared behaviour for consumables that modify player health.
- `Mushroom` – Concrete food item that restores health when used.
- `ToolSpawnTester` – Editor-only helper for spawning tools during playtests.

## Unity Setup
1. **Prefabs**
	- Create a mushroom prefab with the `Mushroom` script, `SpriteRenderer`, collider, and optional audio.
	- In the prefab Inspector, set the **Item Handler Prefab** reference on the `NonReusableTools` component.
2. **Player Controller**
	- Ensure `PlayerController.useKey` is set (default `F`).
	- Assign the same key in menus/documentation so players know how to consume tools.
3. **Spawner/Test Harness (optional)**
	- Drop `ToolSpawnTester` onto an empty GameObject.
	- Populate `Player` with the scene’s `PlayerController` and `Tool Prefab` with the mushroom prefab.
	- Press the configured spawn key (default `P`) in Play Mode to drop mushrooms in front of the player.

## Gameplay Loop
1. Walk up to a mushroom pickup (spawned via handler or level design) and press **E** to add it to the quick access inventory.
2. Use **Tab** or number hotkeys to switch slots.
3. Press **F** (or the configured `useKey`) to consume the mushroom:
	- Player health increases by the value on the `Food` component.
	- The tool logs usage/consumption in the Console and removes itself from the inventory.
4. Press **Q** to drop the currently selected tool back into the world (colliders/renderers re-enable automatically).

## Notes
- The `NonReusableTools.PlaceForTesting` method can drop a handler + tool prefab in front of the player—handy for rapid iteration.
- When picking up a tool, world colliders/rigidbodies renderers are disabled so it no longer blocks movement; they’re restored if the tool is dropped.
- To add new single-use items (e.g., TNT), inherit from `NonReusableTools` or `Food` and implement `OnUse`/`ApplyFoodEffect` as needed.