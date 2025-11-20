# Mikayla DeCuire's Code Directory

This directory contains code contributions by Mikayla DeCuire for the SubTerra project.

---

## Overview
This section contains code related to centralized audio management.  
The **SoundManager** prefab is responsible for handling all game sounds consistently in one place, including player actions, tools, enemy interactions, and background music.

---

## Components
### SoundManager Prefab
Handles audio playback for player actions, tools, enemies, and background music.

- Ties audio clips in the resources folder to player, enemy, or tool actions  
- Provides a default background track (`background_clip`) and an adjustable float value (`background_volume`) for initial volume  
- Uses two AudioSources:  
  - `backgroundSource` → looping background music  
  - `sfxSource` → one-shot sound effects  

#### Main Features
- Play, pause, stop, and fade audio clips  
- Crossfade between background tracks  
- Event-driven SFX playback (e.g., `OnEnemyDeath → Play("Explosion")`)  
- Adjustable volumes  
- Persistent settings across scenes  

---

## Usage
### 1. Setup
1. Drag the **SoundManager prefab** into your initial scene  
2. Mark it as `DontDestroyOnLoad` to persist across scenes  
3. Assign AudioSources and SoundLibrary in the Inspector as needed  

### 2. Trigger Events in Other Scripts
Example usage:
```csharp
SoundEvents.PlayerJump;
```

### 3. Make sure Event exists in SoundEvents and that SoundManager is subscribed
```csharp
public static event Action OnPlayerJump; 

public static void PlayerJump() => OnPlayerJump?.Invoke();

private void OnEnable() {
    SoundEvents.OnPlayerJump += PlayJumpSound;
}
