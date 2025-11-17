# TNT Prefab - Comprehensive Documentation

## Overview

**Target Audience**: Game designers, level designers, Unity developers, and team members integrating tools into SubTerra.

**Purpose**: This document explains the TNT prefab—a throwable, explosive tool that destroys terrain in a radius. Whether you're placing TNT in a level, modifying its behavior, or creating similar consumable items, this guide provides the answers you need.

## What is the TNT Prefab?

### Quick Summary
TNT is a **single-use explosive tool** that players can pick up, throw, and use to destroy cave walls within a configurable radius. It inherits from the `NonReusableTools` base class, making it part of SubTerra's consumable item system.

### Key Characteristics
- **Single-use**: Once thrown and detonated, the TNT is destroyed
- **Physics-based**: TNT bounces realistically when thrown
- **Timed fuse**: 3-second countdown after being lit (configurable)
- **Area damage**: Destroys terrain in a 30-unit radius (configurable)
- **Inventory compatible**: Works with the existing `ItemHandler` and `Inventory` systems

## How TNT Works in the Game

### Player Experience Flow

```
1. Player finds TNT in the world
   └─> TNT sprite is visible, physics enabled
   
2. Player picks up TNT (collision or input trigger)
   └─> TNT enters inventory
   └─> Sprite becomes invisible
   └─> Physics disabled (kinematic)
   
3. Player uses TNT (input action)
   └─> TNT is thrown toward mouse position
   └─> Fuse is automatically lit (3-second countdown)
   └─> Sprite turns red to indicate "lit" state
   └─> Physics re-enabled, TNT bounces naturally
   
4. TNT explodes after fuse timer
   └─> Destroys terrain in radius
   └─> Plays explosion sound/effect
   └─> TNT GameObject destroyed
```

### Design Intent
**Question Being Answered**: *"Why can't players cancel TNT once thrown?"*

**Answer**: Once lit, TNT cannot be picked up or defused. This creates **tension and risk** in gameplay—players must commit to their throw. This design decision encourages strategic placement and punishes reckless use.

---

## Implementation Guide

### Quick Start: Adding TNT to Your Scene

**Who asks this?** Level designers setting up a new cave area.

**What they need to know:**
1. The prefab location
2. How to configure it
3. How players interact with it

#### Step 1: Place the Prefab
```
Location: Assets/src/TylerGehring/prefabs/TNT.prefab
```

1. Drag `TNT.prefab` into your scene hierarchy
2. Position it where players can find it
3. TNT will automatically initialize with default settings

#### Step 2: Verify Required Components
The prefab should already have:
- `TNT.cs` script (inherits from `NonReusableTools`)
- `SpriteRenderer` (for visual representation)
- `Rigidbody2D` (auto-added by TNT script if missing)
- `CircleCollider2D` (auto-added by TNT script if missing)

**Why auto-add physics components?**  
Ensures TNT works even when instantiated from code or duplicated improperly. Prevents "invisible TNT" bugs during testing.


## Technical Architecture

### Class Hierarchy

**Question Being Answered**: *"How does TNT fit into the tool system?"*

```
MonoBehaviour
    └─> NonReusableTools (abstract base class)
            └─> TNT (concrete implementation)
            └─> Mushroom (another consumable)
            └─> [Your custom consumables here]
```

### Inheritance Design Decisions

**Who asks this?** Programmers creating new consumable tools.

**What they need to know:**
- Why TNT inherits from `NonReusableTools`
- What methods MUST be overridden
- What base class functionality they get for free

#### Base Class (`NonReusableTools`) Provides:
1. **Consumption tracking** (`_consumed` flag, `IsConsumed` property)
2. **Owner management** (tracks which player holds the tool)
3. **Pickup/drop lifecycle hooks** (`OnPickup`, `OnDropped`)
4. **Use method** (calls abstract `OnUse`, handles consumption)
5. **Debug logging** (standardized messages)

#### TNT Overrides:
```csharp
protected override bool OnUse(PlayerController player)
{
    // Custom logic: Light fuse, throw TNT
    _LightTNT(player);
    return true; // True = consume from inventory
}
```

**Why return `true` here?**  
Tells the base class to remove TNT from the player's inventory immediately. Returning `false` would keep it (used by reusable tools like flashlights).

---

## Inspector Configuration

**Who asks this?** Designers balancing gameplay or creating TNT variants.

### Serialized Fields Explained

Each `[SerializeField]` exposes a parameter in Unity's Inspector:

#### 1. `_explosionRadius` (default: 30f)
**Question**: *"How much terrain does TNT destroy?"*

- **Range**: Radius in Unity units from explosion center
- **Tip**: Test with `OnDrawGizmosSelected()` to visualize radius in Scene view
- **Balance Notes**: 
  - Too large = trivializes cave navigation
  - Too small = TNT feels useless
  - Default 30f works for medium cave passages

#### 2. `_fuseTime` (default: 3f)
**Question**: *"How long until TNT explodes?"*

- **Range**: Seconds after being thrown
- **Gameplay Impact**: 
  - Shorter fuse = less time to escape blast radius
  - Longer fuse = TNT might roll far from intended target
- **Why 3 seconds?** Gives players time to react but maintains urgency

#### 3. `_throwForce` (default: 10f)
**Question**: *"How far can players throw TNT?"*

- **Range**: Force applied to Rigidbody2D
- **Physics Note**: Actual distance depends on:
  - Gravity scale
  - Bounciness
  - Cave geometry
- **Tuning**: Increase if TNT doesn't reach distant walls

#### 4. `_bounciness` (default: 0.5f)
**Question**: *"Should TNT bounce off walls?"*

- **Range**: 0.0 (no bounce) to 1.0 (perfect bounce)
- **Feel**: 0.5 provides realistic grenade-like behavior
- **Technical**: Applied to `PhysicsMaterial2D` on Rigidbody2D

#### 5. `_explosionSound` (optional)
**Question**: *"What audio plays on detonation?"*

- **Type**: `AudioClip` asset reference
- **Implementation**: Uses `AudioSource.PlayClipAtPoint()` (no AudioSource component needed)
- **Missing**: If null, explosion is silent (not an error)

#### 6. `_explosionEffect` (optional)
**Question**: *"What particle effect spawns on explosion?"*

- **Type**: `GameObject` prefab (typically a particle system)
- **Implementation**: Instantiated at TNT position, plays once
- **Missing**: If null, no visual effect (not an error)

#### 7. `_litColor` (default: Red)
**Question**: *"How do players know TNT is live?"*

- **Visual Feedback**: SpriteRenderer color changes when fuse is lit
- **UX Purpose**: Clear danger indicator
- **Alternative Designs**: Could use sprite swap, animation, or shader effects

#### 8. `_desiredScale` (default: 0.1, 0.1, 1)
**Question**: *"Why is this exposed but never used?"*

- **Answer**: Legacy parameter for early prototyping
- **Current Behavior**: TNT uses sprite's native scale
- **Future**: Could be removed or implemented for dynamic sizing

---

## Integration with Game Systems

### 1. PlayerController Integration

**Who asks this?** Programmers connecting TNT to player input.

**Question**: *"How does the player trigger TNT?"*

#### Method Called by PlayerController:
```csharp
public void Use(PlayerController player)
```

**Call Flow:**
```
Player presses "Use Tool" button
    └─> PlayerController.UseTool() (input handling)
        └─> TNT.Use(playerInstance) (base class method)
            └─> TNT.OnUse(playerInstance) (override)
                └─> TNT._LightTNT(playerInstance)
                └─> TNT._ThrowTNT(playerInstance)
```

#### Consumption Notification:
```csharp
player.ConsumeCurrentTool(this, false);
```

**Why `false` parameter?**  
Tells PlayerController NOT to immediately destroy the GameObject—TNT needs to stay alive until explosion completes.

### 2. Inventory System Integration

**Who asks this?** UI programmers displaying tool status.

**Question**: *"How does inventory know TNT is consumed?"*

#### Public Properties:
```csharp
public bool IsConsumed => _consumed;
public PlayerController Owner => _owner;
public string ToolName => _toolName;
```

**Usage Example:**
```csharp
// Check if tool is still usable
if (!tool.IsConsumed)
{
    DisplayInUI(tool.ToolName);
}
```

### 3. Terrain Destruction System

**Who asks this?** Environment artists/procedural generation programmers.

**Question**: *"How does TNT know which terrain to destroy?"*

#### Integration Point:
```csharp
StaticChunk[] chunks = FindObjectsOfType<StaticChunk>();
foreach (StaticChunk chunk in chunks)
{
    chunk.DestroyInRadius(transform.position, _explosionRadius);
}
```

**Requirements:**
- Scene must contain `StaticChunk` objects (terrain manager)
- `StaticChunk` must implement `DestroyInRadius(Vector3, float)`
- **Warning**: If no StaticChunk found, explosion still occurs but terrain is unaffected

**Why FindObjectsOfType?**  
Simple and robust—works regardless of scene setup. Optimized version could cache chunks or use spatial partitioning.

---

## Common Use Cases

### Use Case 1: Placing TNT as Level Loot

**Actor**: Level Designer  
**Goal**: Add TNT as collectible in cave  
**Steps**:
1. Drag `TNT.prefab` into scene
2. Set position near valuable resources
3. Test: Player should see sprite, walk over to pick up
4. No code changes needed

**Edge Case**: If TNT falls through floor, check:
- Does TNT have Rigidbody2D? (should be auto-added)
- Is Rigidbody2D set to Dynamic (not Kinematic)?

---

### Use Case 2: Spawning TNT from Enemy Drops

**Actor**: Enemy AI Programmer  
**Goal**: Drop TNT when boss dies  
**Code Example**:
```csharp
public void OnBossDeath()
{
    GameObject tntInstance = Instantiate(tntPrefab, transform.position, Quaternion.identity);
    // TNT automatically initializes physics components
    // No additional setup needed
}
```

**Why this works**:
- TNT's `OnEnable()` calls `_CacheComponents()` and `_ConfigurePhysics()`
- Ensures Rigidbody2D/Collider2D exist even if prefab was modified

---

### Use Case 3: Creating TNT Variants

**Actor**: Game Designer  
**Goal**: Make "Mega TNT" with 2x radius  
**Steps**:
1. Duplicate `TNT.prefab` → `MegaTNT.prefab`
2. Select MegaTNT in Inspector
3. Change `Explosion Radius` to 60
4. Change `Tool Name` to "Mega TNT"
5. Test in play mode

**No code changes required**—serialized fields support this design pattern.

---

## Extending the TNT System

### Creating New Consumable Tools

**Who asks this?** Programmers adding medkits, throwable torches, etc.

**Pattern to Follow**:
```csharp
public class MyCustomTool : NonReusableTools
{
    [SerializeField] private float _customParameter;

    protected override bool OnUse(PlayerController player)
    {
        // Your custom logic here
        Debug.Log($"{player.name} used {ToolName}");
        
        // Return true to consume, false to keep in inventory
        return true;
    }
}
```

**Key Decisions**:
1. **Return `true` from `OnUse`**: Tool is consumed (single-use)
2. **Return `false` from `OnUse`**: Tool stays in inventory (reusable)
3. **Override `OnConsumed`**: Custom cleanup logic (like TNT's explosion)

---
