//jack kroll


/*
    This class serves as a base for weapons and tools, keeping them positioned in front of
    the player. It updates their location each frame based on the player's transform, and
    automatically retrieves the player reference if missing. Other weapon/tool types can
    inherit from this class to share the same positional behavior.
*/


/*
    These two classes use dynamic binding through C# polymorphism. The base class (Weapon_Class)
    defines UpdatePosition as a virtual method, which allows the subclass (ABetterGame) to override
    it with its own version. When UpdatePosition is called at runtime, the program automatically
    chooses the correct method based on the actual object type, not the variable type. This means
    that even if a weapon is referenced as the base class, the overridden method in ABetterGame will
    execute instead, enabling different tools to share a common interface while still providing
    customized behavior.
*/

using UnityEngine;

public class Weapon_Class : ReusableToolClass
{
    protected Transform _player;


   
    protected virtual void UpdatePosition()
    {
        

        if (_player == null)
            _player = PlayerController.Instance.GetObject().transform;
       // _player = GameObject.FindGameObjectWithTag("Player").transform;

        Vector3 pos = _player.position + _player.forward * 1f;
        pos.y = _player.position.y + 2.1f;
        pos.z = -1f;

        transform.position = pos;
    }
}
