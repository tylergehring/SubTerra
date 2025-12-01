using UnityEngine;
using System;

public static class SoundEvents
{
    /* Observer Pattern -   Define a one-to-many dependency between objects 
    Game Objects (subjects) notify the SoundManager (observer) when events occur
    This makes it easy to add new sounds or modify existing onces without changing other scripts
    Allows multiple listeners to subscribe so the UI for example can share the information
    */

    // Declare Events that other scripts can subscribe to (for Observer Pattern)
    // Action is a built-in type that represents a method with no parameters and no return value

    public static event Action OnPlayerJump;
    public static event Action OnToolUse;
    public static event Action OnToolPickup;
    public static event Action OnFootstep;
    public static event Action OnEnemyDamage;
    public static event Action OnEnemyThrow;
    public static event Action OnPlayerDeath;

    /* Methods to trigger events
    OnPlayerJump?.Invoke();  means "if OnPlayerJump is NOT null -> call Invoke(); else do nothing"
    I need this null reference check because events in C# are null unless there is a listener

    (?) Replaces this VERY long statement:
        if (OnPlayerJump != null) { OnPlayerJump.Invoke();}
    */
    // Technically Dynamic Binding  -   Actual methods called depend on which event is subscribed @ runtime (Not a good example because I do not use inheritences)
    public static void PlayerJump() => OnPlayerJump?.Invoke();
    public static void ToolUse() => OnToolUse?.Invoke();
    public static void ToolPickup() => OnToolPickup?.Invoke();
    public static void Footstep() => OnFootstep?.Invoke();
    public static void EnemyDamage() => OnEnemyDamage?.Invoke();
    public static void EnemyThrow() => OnEnemyThrow?.Invoke();
    public static void PlayerDeath() => OnPlayerDeath?.Invoke();

    /* --------------------------------
        STATIC vs DYNAMIC BINDING DEMO
    ------------------------------------- */

    // SUPER CLASS
    /*
    public class SoundAction {
        public void PlayStatic() { Debug.Log("Super Static"); }
        public virtual void PlayDynamic() { Debug.Log("Super Dynamic"); }
    }

    // SUB CLASSES
    public class JumpSoundAction : SoundAction {
        public override void PlayDynamic() { Debug.Log("Dynamic Jump!");}
    }

    public class ToolSoundAction : SoundAction {
        public override void PlayDynamic() { Debug.Log("Dynamic Tool!");}
    }

    public static void RunBindingDemo() {
        SoundAction action = new SoundAction();
        SoundAction jumpAction = new JumpSoundAction();
        SoundAction toolAction = new ToolSoundAction();

        /* Static calls are resolved @ COMPILE TIME
        Static method is always called
        Even though jumpAction/toolAction have overriding methods (Look VS Code even uses different colors thats cool)
        */
    /* baseAction.PlayStatic();            // "Super Static"            
        jumpAction.PlayStatic();            // "Super Static"       
        toolAction.PlayStatic();            // "Super Static" 

        /* Dynamic calls are resolved @ RUNTIME 
        Changing the object's type now changes which function gets called
        */
    /*
    action.PlayDynamic();               // "Super Dynamic" 
    jumpAction.PlayDynamic();           // "Dynamic Jump!"
    toolAction.PlayDynamic();           // "Dynamic Tool!" 
}
*/
    //--------------------------------------------------------------------------------DemoDone
}
