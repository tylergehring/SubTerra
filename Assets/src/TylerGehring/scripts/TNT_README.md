# TNT Implementation Guide

## Overview
The TNT class has been implemented as a throwable explosive that destroys cave walls. It follows the same pattern as the Mushroom class, extending `NonReusableTools`.

## How It Works

1. **Throwing**: When the player uses/throws the TNT, it "lights" (changes color to red by default)
2. **Fuse Timer**: After being lit, it waits 3 seconds (configurable)
3. **Explosion**: After the timer expires, it:
   - Plays an explosion sound (if assigned)
   - Spawns a visual effect (if assigned)
   - Destroys all cave walls within the explosion radius using Matthew's `StaticChunk.DestroyInRadius()` method
   - Destroys itself

## Unity Setup Instructions

### 1. Create the TNT Prefab
1. Create a new GameObject in your scene
2. Rename it to "TNT"
3. Add the `TNT` component to it
4. Add a `SpriteRenderer` component (required for visual feedback when lit)
5. Assign a TNT sprite to the SpriteRenderer

### 2. Configure TNT Settings
In the Inspector, configure these fields:

**Tool Setup (inherited from NonReusableTools):**
- **Tool Name**: "TNT" (or whatever you want to call it)

**TNT Settings:**
- **Explosion Radius**: 30 (default, adjust as needed - this is in world units)
- **Fuse Time**: 3 (seconds before explosion)
- **Explosion Sound**: Optional AudioClip for explosion sound
- **Explosion Effect**: Optional GameObject for visual effect (particle system, etc.)
- **Lit Color**: Color.red (default - the color the TNT turns when lit)

### 3. Test with ToolSpawnTester
1. Add the `ToolSpawnTester` component to any GameObject in your scene
2. Configure it:
   - **Player**: Reference to your PlayerController
   - **Item Handler Prefab**: The ItemHandler prefab
   - **Tool Prefab**: The TNT prefab you just created
   - **Spawn Key**: KeyCode.P (or any key you prefer)

### 4. Usage in Game
1. Press your spawn key (default P) to spawn TNT
2. Walk to the TNT and pick it up (using your pickup key)
3. Press your use/throw key to throw the TNT
4. The TNT will turn red (or your chosen lit color)
5. After 3 seconds, it will explode and destroy nearby cave walls

## Key Features

- **Visual Feedback**: TNT changes color when lit
- **Configurable**: Explosion radius, fuse time, and effects are all adjustable
- **Terrain Destruction**: Uses Matthew's existing `StaticChunk.DestroyInRadius()` method
- **Sound & Effects**: Support for explosion audio and particle effects
- **Debug Visualization**: Explosion radius is shown as a red wire sphere in the Scene view when selected

## Technical Notes

- The TNT extends `NonReusableTools`, so it integrates with your existing tool system
- When thrown, `OnConsumed()` detaches the TNT from the player so it stays in the world
- The explosion happens via a coroutine (`ExplodeAfterDelay()`)
- All `StaticChunk` objects in the scene are affected by the explosion
- The TNT destroys itself after exploding

## Testing Recommendations

1. Test with different explosion radii to find the right balance
2. Create a simple explosion particle effect for visual feedback
3. Add an explosion sound for better game feel
4. Test on different terrain configurations to ensure it works properly

## Files Created
- `TNT.cs` - The TNT implementation in `Assets/src/TylerGehring/scripts/`
