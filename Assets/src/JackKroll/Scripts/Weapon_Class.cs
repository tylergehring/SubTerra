using UnityEngine;

public class Weapon_Class : ReusableToolClass
{
    protected Transform player;


   
    protected virtual void UpdatePosition()
    {
        

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        Vector3 pos = player.position + player.forward * 1f;
        pos.y = player.position.y + 10.1f;
        pos.z = -1f;

        transform.position = pos;
    }
}
