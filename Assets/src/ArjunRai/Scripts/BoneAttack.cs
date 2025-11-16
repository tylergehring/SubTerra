using UnityEngine;

public class BoneAttack : MonoBehaviour
{
    public int damage = 10;
    private bool alreadyHit = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (alreadyHit) return;

        if (other.CompareTag("Player"))
        {
            alreadyHit = true;

            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.ChangeHealth(damage);  // ? DAMAGE PLAYER
            }

            Destroy(gameObject); // remove bone
        }
    }
}
