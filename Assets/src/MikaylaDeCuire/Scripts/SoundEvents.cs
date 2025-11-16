using UnityEngine;
using System;

public static class SoundEvents {
    /* Observer Pattern -   Define a one-to-many dependency between objects 
    Game Objects (subjects) notify the SoundManager (observer) when events occur
    This makes it easy to add new sounds or modify existing onces without changing other scripts
    Allows multiple listeners to subscribe so the UI for example can share the information
    */

    // Declare Events that other scripts can subscribe to (for Observer Pattern)
    // Action is a built-in type that represents a method with no parameters and no return value

    public static event Action OnPlayerJump;
    public static event Action OnPlayerLand;
    public static event Action OnToolUse;
    public static event Action OnToolPickup;
    public static event Action OnFootstep;

    /* Methods to trigger events
    OnPlayerJump?.Invoke();  means "if OnPlayerJump is NOT null -> call Invoke(); else do nothing"
    I need this null reference check because events in C# are null unless there is a listener
    So this line would throw a Null Reference Exception. Unless I check it first?
    (?) Replaces this VERY long statement:
        if (OnPlayerJump != null) { OnPlayerJump.Invoke();}
    */
    // Technically Dynamic Binding  -   Actual methods called depend on which event is subscribed @ runtime (Not a good example because I do not use inheritences)
    public static void PlayerJump() => OnPlayerJump?.Invoke();
    public static void PlayerLand() => OnPlayerLand?.Invoke();
    public static void ToolUse() => OnToolUse?.Invoke();
    public static void ToolPickup() => OnToolPickup?.Invoke();
    public static void Footstep() => OnFootstep?.Invoke();
    
}
