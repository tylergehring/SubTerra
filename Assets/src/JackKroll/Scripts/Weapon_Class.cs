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
        pos.y = _player.position.y + 10.1f;
        pos.z = -1f;

        transform.position = pos;
    }
}
